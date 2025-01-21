using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class TurnManager : NetworkBehaviour
{
    public static TurnManager Instance { get; private set; }

    //Overall Events
    public event EventHandler OnResetRound;
    public event EventHandler OnSetCoverUp;
    public event EventHandler OnSetCoverDown;
    public event Action<bool, ulong> OnSetEndscreen;

    //Herostuff Events
    public event EventHandler OnApplyXPandLevelUps;
    public event EventHandler OnApplyHammerPanels;
    public event EventHandler OnApplyEnergyPanels;
    public event EventHandler OnGetCrownHP;

    //Heldenzuweisung
    private Hero[] playerHeroes = new Hero[2];     //Helden des Spielers
    private Hero[] enemyHeroes = new Hero[2];      //Helden des Gegners

    private Dictionary<ulong, int> CrownHPDictionary;

    private int currentTurnStep = 1;

    private bool matchHasEnded = false;     //Wird Wahr, wenn mindestens ein Spieler auf 0 HP gesunken ist und somit das Match beendet

    private void Awake()
    {
        Instance = this;

        CrownHPDictionary = new Dictionary<ulong, int>();
    }

    private void Start()
    {
        MultiplayerGameManager.Instance.OnPlayersRoundIsFinished += MultiplayerGameManager_OnPlayersRoundIsFinished;
    }

    private void MultiplayerGameManager_OnPlayersRoundIsFinished(object sender, EventArgs e)
    {
        BeginTurn();
    }

    public void SetHeroes(Hero playerSquare, Hero playerDiamond, Hero enemySquare, Hero enemyDiamond)
    {
        playerHeroes[0] = playerSquare;
        playerHeroes[1] = playerDiamond;

        enemyHeroes[0] = enemySquare;
        enemyHeroes[1] = enemyDiamond;
    }





    //Logic um den Spielzyklus auszuwerten
    private async void BeginTurn()
    {
        //Nur ausführen, wenn es der Server ist
        if (!IsServer) { return; }
        Debug.Log("Der Server startet die Turnreihenfolge.");

        //Resette den aktuellen Schritt und starte den Zugzyklus
        currentTurnStep = 1;
        await ProcessTurnStep();
    }

    private async Task ProcessTurnStep()
    {
        switch (currentTurnStep)
        {
            case 1:
                //Kurz warten vor der ersten Aktion
                await WaitForSecondsAsync(1f);

                //Cover herunterfahren
                ChangeCoverStateRpc(false);
                await WaitForSecondsAsync(1f);
                break;

            case 2:
                //XP Panel, Level ups
                await ApplyPanelXPAndLevelUps();
                break;

            case 3:
                //Hammer panels added
                await ApplyHammerPanels();
                break;

            case 4:
                //Energy panels added
                await ApplyEnergyPanels();
                break;

            case 5:
                //Assassin acts
                await ActingAssassin(playerHeroes);
                await ActingAssassin(enemyHeroes);
                break;

            case 6:
                //Priest acts
                await ActingPriest1(playerHeroes);
                await ActingPriest1(enemyHeroes);
                break;

            case 7:
                //Engineer acts
                await ActingEngineer(playerHeroes);
                await ActingEngineer(enemyHeroes);
                break;

            case 8:
                //Bombs
                await DeplayingBombs(playerHeroes);
                await DeplayingBombs(enemyHeroes);
                break;

            case 9:
                //Rest od heroes act
                await ActingRestHeroes(playerHeroes);
                await ActingRestHeroes(enemyHeroes);
                break;

            case 10:
                //Priest acts again
                await ActingPriest2(playerHeroes);
                await ActingPriest2(enemyHeroes);
                break;

            case 11:
                //Heroes acting from Priest
                await ActingHeroesAgain(playerHeroes);
                await ActingHeroesAgain(enemyHeroes);
                break;

            case 12:
                //Bombs again
                await DeployingBombsAgain(playerHeroes);
                await DeployingBombsAgain(enemyHeroes);
                break;

            case 13:
                //0 HP Crown check
                CheckCrownHP();
                break;

            default:
                Debug.LogError("Beim Turnmanager ist irgendwas schief gelaufen! " + currentTurnStep + " ist kein valides Argument!");
                await Task.Yield();
                break;
        }

        currentTurnStep++;
        if (currentTurnStep <= 13)
        {
            await ProcessTurnStep();
        }
        else
        {
            await WaitForSecondsAsync(2f);
            ChangeCoverStateRpc(true);     //Cover hochfahren
            await WaitForSecondsAsync(1f);
            EndTurn();
        }
    }

    private void EndTurn()
    {
        if (!matchHasEnded)
        {
            Debug.Log("Runde beendet. Nächste Runde wird vorbereitet!");

            ResetRoundRpc();
        }
    }

    //Methoden für Heldenaktionen mit Animation
    // 1) Panel XP, Level ups
    private async Task ApplyPanelXPAndLevelUps()
    {
        OnApplyXPandLevelUps?.Invoke(this, EventArgs.Empty);

        await WaitForSecondsAsync(0.8f);
    }

    // 2) Hammer panels added
    private async Task ApplyHammerPanels()
    {
        OnApplyHammerPanels?.Invoke(this, EventArgs.Empty);

        await WaitForSecondsAsync(0.8f);
    }

    // 3) Energy panels added
    private async Task ApplyEnergyPanels()
    {
        OnApplyEnergyPanels?.Invoke(this, EventArgs.Empty);

        await WaitForSecondsAsync(0.8f);
    }

    // 4) Assassin acts
    private async Task ActingAssassin(Hero[] heros)
    {
        foreach (var hero in heros)
        {
            if (hero.GetHeroType() == HeroType.Assassin && hero.GetCanMakeAction())
            {
                //Animation
                float animationLength = hero.GetAnimationLength();
                hero.ActivateActionRpc();
                
                //Wartezeit
                await WaitForSecondsAsync(animationLength);

                //Energy Bar wieder zurücksetzen
                hero.ResetEnergyBar();

                //Wartezeit 2, um den Spielfluss etwas übersichtlicher zu halten
                await WaitForSecondsAsync(0.2f);
            }
        }
    }

    // 5) Priest acts 1
    private async Task ActingPriest1(Hero[] heroes)
    {
        foreach (var hero in heroes)
        {
            if (hero.GetHeroType() == HeroType.Priest && hero.PriestChecksAction())
            {
                //Animation
                float animationLength = hero.GetAnimationLength();
                hero.ActivateActionRpc();

                //Wartezeit
                await WaitForSecondsAsync(animationLength);

                //Energy Bar wieder zurücksetzen
                hero.ResetEnergyBar();

                //Wartezeit 2, um den Spielfluss etwas übersichtlicher zu halten
                await WaitForSecondsAsync(0.2f);
            }
        }
    }

    // 6) Engineer Acts
    private async Task ActingEngineer(Hero[] heroes)
    {
        foreach (var hero in heroes)
        {
            if (hero.GetHeroType() == HeroType.Engineer && hero.GetCanMakeAction())
            {
                //Animation
                float animationLength = hero.GetAnimationLength();
                hero.ActivateActionRpc();

                //Wartezeit
                await WaitForSecondsAsync(animationLength);

                //Energy Bar wieder zurücksetzen
                hero.ResetEnergyBar();

                //Wartezeit 2, um den Spielfluss etwas übersichtlicher zu halten
                await WaitForSecondsAsync(0.2f);
            }
        }
    }

    // 7) Bomben senden
    private async Task DeplayingBombs(Hero[] heroes)
    {
        foreach (var hero in heroes)
        {
            if (hero.GetCanSendBomb())
            {
                //Animation
                float animationLength = hero.GetBombAnimationLength();
                hero.SendBombRpc();

                //Wartezeit
                await WaitForSecondsAsync(animationLength);
            }
        }
    }

    // 8) Rest of Heroes act
    private async Task ActingRestHeroes(Hero[] heroes)
    {
        foreach (var hero in heroes)
        {
            //Test ob der Held schon dran war
            bool validHero = true;
            if (hero.GetHeroType() == HeroType.Assassin) { validHero = false; }
            else if (hero.GetHeroType() == HeroType.Priest) { validHero = false; }
            else if (hero.GetHeroType() == HeroType.Engineer) { validHero = false; }

            if (validHero && hero.GetCanMakeAction())
            {
                //Animation
                float animationLength = hero.GetAnimationLength();
                hero.ActivateActionRpc();

                //Wartezeit
                await WaitForSecondsAsync(animationLength);

                //Energy Bar wieder zurücksetzen
                hero.ResetEnergyBar();

                //Wartezeit 2, um den Spielfluss etwas übersichtlicher zu halten
                await WaitForSecondsAsync(0.2f);
            }
        }
    }

    // 9) (If the second hero had enough energy from energy panels to act: Priest grants energy)
    private async Task ActingPriest2(Hero[] heroes)
    {
        foreach (var hero in heroes)
        {
            if (hero.GetHeroType() == HeroType.Priest && IsServer && !hero.GetPriestBoosted() && hero.GetCanMakeAction())
            {
                foreach (var held in heroes.Where(h => h != hero))
                {
                    float animationLength = hero.GetPriestSecondAnimationLength();
                    hero.ActivateSecondPriestRpc();

                    //Wartezeit
                    await WaitForSecondsAsync(animationLength);
                }
            }
        }
    }

    // 10) Hero acts from priest energy
    private async Task ActingHeroesAgain(Hero[] heroes)
    {
        foreach (var hero in heroes)
        {
            if (hero.GetCanMakeAction())
            {
                //Animation
                float animationLength = hero.GetAnimationLength();
                hero.ActivateActionRpc();

                //Wartezeit
                await WaitForSecondsAsync(animationLength);

                //Energy Bar wieder zurücksetzen
                hero.ResetEnergyBar();

                //Wartezeit 2, um den Spielfluss etwas übersichtlicher zu halten
                await WaitForSecondsAsync(0.2f);
            }
        }
    }

    // 11) Bombs again
    private async Task DeployingBombsAgain(Hero[] heroes)
    {
        foreach (var hero in heroes)
        {
            if (hero.GetCanSendBomb())
            {
                //Animation
                float animationLength = hero.GetBombAnimationLength();
                hero.SendBombRpc();

                //Wartezeit
                await WaitForSecondsAsync(animationLength);
            }
        }
    }

    // 12) 0 HP Crown check
    private void CheckCrownHP()
    {
        OnGetCrownHP?.Invoke(this, EventArgs.Empty);

        ulong[] playerId = CrownHPDictionary.Keys.ToArray();
        int[] crownsHp = CrownHPDictionary.Values.ToArray();

        Debug.Log($"Spieler {playerId[0]} hat noch {crownsHp[0]} HP!");
        Debug.Log($"Spieler {playerId[1]} hat noch {crownsHp[1]} HP!");

        //Nur ausführen, wenn mindestens ein Spieler auf 0 HP ist.
        if (Array.Exists(crownsHp, value => value == 0))
        {
            matchHasEnded = true;
            InvokeEndscreensRpc(playerId, crownsHp);
        }
    }

    [Rpc(SendTo.Everyone)]
    private void InvokeEndscreensRpc(ulong[] playerId, int[] crownsHp)
    {
        //Checken, ob beide Seiten verloren haben
        if ((crownsHp[0] == 0) && (crownsHp[1] == 0))
        {
            //Unentschieden Screen bei beiden aktivieren
            OnSetEndscreen?.Invoke(true, (ulong)0);
        }
        else if (crownsHp[0] == 0)
        {
            //Win Screen bei Spieler 1 aktivieren
            //Lose Screen bei Spieler 0 aktivieren
            OnSetEndscreen?.Invoke(false, playerId[1]);
        }
        else if (crownsHp[1] == 0)
        {
            //Lose Screen bei Spieler 1 aktivieren
            //Win Screen bei Spieler 0 aktivieren
            OnSetEndscreen?.Invoke(false, playerId[0]);
        }
    }




    //Hilfsfunktionen
    [Rpc(SendTo.Everyone)]
    private void ResetRoundRpc()
    {
        OnResetRound?.Invoke(this, EventArgs.Empty);
    }

    //Zeitverzögerung
    private async Task WaitForSecondsAsync(float seconds)
    {
        await Task.Delay((int)(seconds * 1000));
    }

    //Empfangen der CrownHP
    public void SetCrownHPForPlayer(ulong playerId, int hp)
    {
        CrownHPDictionary[playerId] = hp;
    }

    //Cover triggern
    [Rpc(SendTo.Everyone)]
    private void ChangeCoverStateRpc(bool setCoverUp)
    {
        if (setCoverUp)
        {
            OnSetCoverUp?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            OnSetCoverDown?.Invoke(this, EventArgs.Empty);
        }
    }
}
