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
    public event EventHandler OnInitializeCrownHP;
    public event Action<bool, ulong> OnSetEndscreen;

    //Herostuff Events
    public event EventHandler OnApplyXPandLevelUps;
    public event EventHandler OnApplyHammerPanels;
    public event EventHandler OnApplyEnergyPanels;

    //Dictionaries
    private Dictionary<ulong, int> crownHPDictionary;
    //Nur Debug
    [SerializeField] private ulong[] playerIds = new ulong[2];
    [SerializeField] private int[] playerHps = new int[2];

    //Heldenzuweisung
    private Hero[] playerHeroes = new Hero[2];     //Helden des Spielers
    private Hero[] enemyHeroes = new Hero[2];      //Helden des Gegners

    private int currentTurnStep = 1;

    private void Awake()
    {
        Instance = this;

        crownHPDictionary = new Dictionary<ulong, int>();
    }

    private void Start()
    {
        MultiplayerGameManager.Instance.OnPlayersRoundIsFinished += MultiplayerGameManager_OnPlayersRoundIsFinished;
        //StartCoroutine(InitializeReadynessLate());
        //StartCoroutine(InitializeCrownHPLate());
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
                OnSetCoverDown?.Invoke(this, EventArgs.Empty);
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
            OnSetCoverUp?.Invoke(this, EventArgs.Empty);
            //SetCoverUpRpc();
            await WaitForSecondsAsync(1f);
            EndTurn();
        }
    }

    private void EndTurn()
    {
        Debug.Log("Runde beendet. Nächste Runde wird vorbereitet!");

        ResetRoundRpc();
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
            }
        }
    }

    // 9) (If the second hero had enough energy from energy panels to act: Priest grants energy)
    private async Task ActingPriest2(Hero[] heroes)
    {
        ////Debug.Log("Das hier sind die Heroes, die angekommen sind: " + heroes);
        //foreach (var hero in heroes)
        //{
        //    //Debug.Log("Das ist der Hero der aus " + heroes + " kommt: " + hero);
        //    foreach (var held in heroes)
        //    {
        //        if (hero.GetHeroType() == HeroType.Priest)
        //        {
        //            if (IsServer)   //Hier nur ausführen, wenn es der Server ist, damit die NetworkVariable nur vom Server geändert wird
        //            {
        //                if ((held != hero) && !hero.GetPriestBoosted())
        //                {
        //                    //Animation
        //                    float animationLength = hero.GetPriestSecondAnimationLength();
        //                    hero.ActivateSecondPriestRpc();

        //                    //Wartezeit
        //                    await WaitForSecondsAsync(animationLength); 
        //                }
        //            }
        //        }
        //    }
        //}

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
        ulong[] playerId = crownHPDictionary.Keys.ToArray();
        int[] crownsHp = crownHPDictionary.Values.ToArray();

        Debug.Log($"Spieler {playerId[0]} hat noch {crownsHp[0]} HP!");
        Debug.Log($"Spieler {playerId[1]} hat noch {crownsHp[1]} HP!");

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
    public void ChangeCrownHP(ulong playerId, int hp)
    {
        Debug.Log($"Spieler {playerId} hat nun {hp} HP!");

        ChangeCrownHPRpc(playerId, hp);
    }

    [Rpc(SendTo.Everyone)]
    private void ChangeCrownHPRpc(ulong playerId, int hp)
    {
        crownHPDictionary[playerId] = hp;

        OnCrownHPChanged();
    }

    [Rpc(SendTo.Everyone)]
    private void ResetRoundRpc()
    {
        OnResetRound?.Invoke(this, EventArgs.Empty);
    }

    public IEnumerator InitializeReadynessLate()
    {
        yield return new WaitForEndOfFrame();
    }

    public IEnumerator InitializeCrownHPLate()
    {
        yield return new WaitForEndOfFrame();

        Debug.Log("Hier sollten die Ursprungs HP initialisiert werden.");
        OnInitializeCrownHP?.Invoke(this, EventArgs.Empty);
    }

    private void OnCrownHPChanged()
    {
        //Debug.Log("HP haben sich geändert");
        if (crownHPDictionary.Count == 2)
        {
            playerIds = crownHPDictionary.Keys.ToArray();
            playerHps = crownHPDictionary.Values.ToArray();
        }
    }

    //Zeitverzögerung
    private async Task WaitForSecondsAsync(float seconds)
    {
        await Task.Delay((int)(seconds * 1000));
    }
}
