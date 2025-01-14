using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public void BeginTurn()
    {
        //Nur ausführen, wenn es der Server ist
        if (!IsServer) { return; }
        Debug.Log("Der Server startet die Turnreihenfolge.");

        //Resette den aktuellen Schritt und starte den Zugzyklus
        currentTurnStep = 1;
        StartCoroutine(ProcessTurnStep());
    }

    private IEnumerator ProcessTurnStep()
    {
        switch (currentTurnStep)
        {
            case 1:
                //Kurz warten vor der ersten Aktion
                yield return new WaitForSeconds(1f);

                //Cover herunterfahren
                OnSetCoverDown?.Invoke(this, EventArgs.Empty);
                yield return new WaitForSeconds(1f);
                break;

            case 2:
                //XP Panel, Level ups
                yield return StartCoroutine(ApplyPanelXPAndLevelUps());
                break;

            case 3:
                //Hammer panels added
                yield return StartCoroutine(ApplyHammerPanels());
                break;

            case 4:
                //Energy panels added
                yield return StartCoroutine(ApplyEnergyPanels());
                break;

            case 5:
                //Assassin acts
                yield return StartCoroutine(ActingAssassin(playerHeroes));
                yield return StartCoroutine(ActingAssassin(enemyHeroes));
                break;

            case 6:
                //Priest acts
                yield return StartCoroutine(ActingPriest1(playerHeroes));
                yield return StartCoroutine(ActingPriest1(enemyHeroes));
                break;

            case 7:
                //Engineer acts
                yield return StartCoroutine(ActingEngineer(playerHeroes));
                yield return StartCoroutine(ActingEngineer(enemyHeroes));
                break;

            case 8:
                //Bombs
                yield return StartCoroutine(DeplayingBombs(playerHeroes));
                yield return StartCoroutine(DeplayingBombs(enemyHeroes));
                break;

            case 9:
                //Rest od heroes act
                yield return StartCoroutine(ActingRestHeroes(playerHeroes));
                yield return StartCoroutine(ActingRestHeroes(enemyHeroes));
                break;

            case 10:
                //Priest acts again
                yield return StartCoroutine(ActingPriest2(playerHeroes));
                yield return StartCoroutine(ActingPriest2(enemyHeroes));
                break;

            case 11:
                //Heroes acting from Priest
                yield return StartCoroutine(ActingHeroesAgain(playerHeroes));
                yield return StartCoroutine(ActingHeroesAgain(enemyHeroes));
                break;

            case 12:
                //Bombs again
                yield return StartCoroutine(DeployingBombsAgain(playerHeroes));
                yield return StartCoroutine(DeployingBombsAgain(enemyHeroes));
                break;

            case 13:
                //0 HP Crown check
                yield return StartCoroutine(CheckCrownHP());
                break;

            default:
                Debug.LogError("Beim Turnmanager ist irgendwas schief gelaufen! " + currentTurnStep + " ist kein valides Argument!");
                yield return null;
                break;
        }

        currentTurnStep++;
        if (currentTurnStep <= 13)
        {
            StartCoroutine(ProcessTurnStep());
        }
        else
        {
            yield return new WaitForSeconds(2f);
            OnSetCoverUp?.Invoke(this, EventArgs.Empty);
            yield return new WaitForSeconds(1f);
            EndTurn();
        }
    }

    private void EndTurn()
    {
        Debug.Log("Runde beendet. Nächste Runde wird vorbereitet!");

        OnResetRound?.Invoke(this, EventArgs.Empty);
    }

    //Methoden für Heldenaktionen mit Animation
    // 1) Panel XP, Level ups
    private IEnumerator ApplyPanelXPAndLevelUps()
    {
        OnApplyXPandLevelUps?.Invoke(this, EventArgs.Empty);

        yield return new WaitForSeconds(0.8f);
    }

    // 2) Hammer panels added
    private IEnumerator ApplyHammerPanels()
    {
        OnApplyHammerPanels?.Invoke(this, EventArgs.Empty);

        yield return new WaitForSeconds(0.8f);
    }

    // 3) Energy panels added
    private IEnumerator ApplyEnergyPanels()
    {
        OnApplyEnergyPanels?.Invoke(this, EventArgs.Empty);

        yield return new WaitForSeconds(0.8f);
    }

    // 4) Assassin acts
    private IEnumerator ActingAssassin(Hero[] heros)
    {
        foreach (var hero in heros)
        {
            if (hero.GetHeroType() == HeroType.Assassin)
            {
                if (hero.GetCanMakeAction())
                {
                    yield return StartCoroutine(hero.ActivateAction(hero.GetHeroType()));
                }
                else { yield return null; }
            }
            else { yield return null; }
        }
    }

    // 5) Priest acts 1
    private IEnumerator ActingPriest1(Hero[] heroes)
    {
        foreach (var hero in heroes)
        {
            if (hero.GetHeroType() == HeroType.Priest)
            {
                if (hero.GetCanMakeAction())
                {
                    yield return StartCoroutine(hero.ActivateAction(hero.GetHeroType()));
                }
                else { yield return null; }
            }
            else { yield return null; }
        }
    }

    // 6) Engineer Acts
    private IEnumerator ActingEngineer(Hero[] heroes)
    {
        foreach (var hero in heroes)
        {
            if (hero.GetHeroType() == HeroType.Engineer)
            {
                if (hero.GetCanMakeAction())
                {
                    yield return StartCoroutine(hero.ActivateAction(hero.GetHeroType()));
                }
                else { yield return null; }
            }
            else { yield return null; }
        }
    }

    // 7) Bomben senden
    private IEnumerator DeplayingBombs(Hero[] heroes)
    {
        foreach (var hero in heroes)
        {
            if (hero.GetCanSendBomb())
            {
                yield return StartCoroutine(hero.SendBomb());
            }
            else { yield return null; }
        }
    }

    // 8) Rest of Heroes act
    private IEnumerator ActingRestHeroes(Hero[] heroes)
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
                yield return StartCoroutine(hero.ActivateAction(hero.GetHeroType()));
            }
            else { yield return null; }
        }
    }

    // 9) (If the second hero had enough energy from energy panels to act: Priest grants energy)
    private IEnumerator ActingPriest2(Hero[] heroes)
    {
        //Debug.Log("Das hier sind die Heroes, die angekommen sind: " + heroes);
        foreach (var hero in heroes)
        {
            //Debug.Log("Das ist der Hero der aus " + heroes + " kommt: " + hero);
            foreach (var held in heroes)
            {
                if (hero.GetHeroType() == HeroType.Priest)
                {
                    if ((held != hero) && !hero.GetPriestBoosted())
                    {
                        yield return StartCoroutine(hero.ActivateSecondPriest(hero.GetHeroType()));
                    }
                    else { yield return null; }
                }
            }
        }
    }

    // 10) Hero acts from priest energy
    private IEnumerator ActingHeroesAgain(Hero[] heroes)
    {
        foreach (var hero in heroes)
        {
            if (hero.GetCanMakeAction())
            {
                yield return StartCoroutine(hero.ActivateAction(hero.GetHeroType()));
            }
            else { yield return null; }
        }
    }

    // 11) Bombs again
    private IEnumerator DeployingBombsAgain(Hero[] heroes)
    {
        foreach (var hero in heroes)
        {
            if (hero.GetCanSendBomb())
            {
                yield return StartCoroutine(hero.SendBomb());
            }
            else { yield return null; }
        }
    }

    // 12) 0 HP Crown check
    private IEnumerator CheckCrownHP()
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

        yield return null;
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
}
