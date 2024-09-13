using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private Hero[] playerHeroes;   //Helden des Spielers
    [SerializeField] private Hero[] enemyHeroes;    //Helden des Gegners

    [SerializeField] private WheelManager playerWheelManager;   //WheelManager des Spielers
    [SerializeField] private WheelManager enemyWheelManager;    //WheelManager des Gegners

    [SerializeField] private CrownManager playerCrownManager;   //Crownmanager des Spielers
    [SerializeField] private CrownManager enemyCrownManager;    //CrownManager des Gegners

    [SerializeField] private BulwarkMover playerBulwarkMover;   //BulwarkManager des Spielers
    [SerializeField] private BulwarkMover enemyBulwarkMover;    //BulwarkManager des Gegners

    //Anzeigen für Gewinnen / Verlieren / Unentschienden
    [SerializeField] private GameObject WinScreen;
    [SerializeField] private GameObject LoseScreen;
    [SerializeField] private GameObject DrawScreen;

    private int currentTurnStep = 1;

    //Schauen, ob die Spieler bereit sind
    //private bool playerReady = false;
    //private bool enemyReady = false;

    private void Start()
    {
        WinScreen.SetActive(false);
        LoseScreen.SetActive(false);
        DrawScreen.SetActive(false);
    }

    public void TestForReadyness()
    {
        if (playerWheelManager.GetStatusFinished() && enemyWheelManager.GetStatusFinished())
        {
            playerWheelManager.ResetStatusFinshed();
            enemyWheelManager.ResetStatusFinshed();
            BeginTurn();
        }
    }

    public void BeginTurn()
    {
        //Resette den aktuellen Schritt und starte den Zugzyklus
        currentTurnStep = 1;
        StartCoroutine(ProcessTurnStep());
    }

    private IEnumerator ProcessTurnStep()
    {
        switch (currentTurnStep)
        {
            case 1:
                //XP Panel, Level ups
                yield return StartCoroutine(ApplyPanelXPAndLevelUps(playerWheelManager));
                yield return StartCoroutine(ApplyPanelXPAndLevelUps(enemyWheelManager));
                break;

            case 2:
                //Hammer panels added
                yield return StartCoroutine(ApplyHammerPanels(playerWheelManager));
                yield return StartCoroutine(ApplyHammerPanels(enemyWheelManager));
                break;

            case 3:
                //Energy panels added
                yield return StartCoroutine(ApplyEnergyPanels(playerWheelManager));
                yield return StartCoroutine(ApplyEnergyPanels(enemyWheelManager));
                break;

            case 4:
                //Assassin acts
                yield return StartCoroutine(ActingAssassin(playerHeroes));
                yield return StartCoroutine(ActingAssassin(enemyHeroes));
                break;

            case 5:
                //Priest acts
                yield return StartCoroutine(ActingPriest1(playerHeroes));
                yield return StartCoroutine(ActingPriest1(enemyHeroes));
                break;

            case 6:
                //Engineer acts
                yield return StartCoroutine(ActingEngineer(playerHeroes));
                yield return StartCoroutine(ActingEngineer(enemyHeroes));
                break;

            case 7:
                //Bombs
                yield return StartCoroutine(DeplayingBombs(playerHeroes));
                yield return StartCoroutine(DeplayingBombs(enemyHeroes));
                break;

            case 8:
                //Rest od heroes act
                yield return StartCoroutine(ActingRestHeroes(playerHeroes));
                yield return StartCoroutine(ActingRestHeroes(enemyHeroes));
                break;

            case 9:
                //Priest acts again
                yield return StartCoroutine(ActingPriest2(playerHeroes));
                yield return StartCoroutine(ActingPriest2(enemyHeroes));
                break;

            case 10:
                //Heroes acting from Priest
                yield return StartCoroutine(ActingHeroesAgain(playerHeroes));
                yield return StartCoroutine(ActingHeroesAgain(enemyHeroes));
                break;

            case 11:
                //Bombs again
                yield return StartCoroutine(DeployingBombsAgain(playerHeroes));
                yield return StartCoroutine(DeployingBombsAgain(enemyHeroes));
                break;

            case 12:
                //0 HP Crown check
                yield return StartCoroutine(CheckCrownHP());
                break;

            default:
                Debug.LogError("Beim Turnmanager ist irgendwas schief gelaufen! " + currentTurnStep + " ist kein valides Argument!");
                yield return null;
                break;
        }

        currentTurnStep++;
        if (currentTurnStep <= 12)
        {
            StartCoroutine(ProcessTurnStep());
        }
        else
        {
            EndTurn();
        }
    }

    private void EndTurn()
    {
        Debug.Log("Runde beendet. Nächste Runde wird vorbereitet!");
        playerWheelManager.ResetRound();
        enemyWheelManager.ResetRound();
    }

    //Methoden für Heldenaktionen mit Animation
    // 1) Panel XP, Level ups
    private IEnumerator ApplyPanelXPAndLevelUps(WheelManager wheels)
    {
        wheels.EvaluateXPGained();

        yield return new WaitForSeconds(0.8f);
    }

    // 2) Hammer panels added
    private IEnumerator ApplyHammerPanels(WheelManager wheels)
    {
        wheels.EvaluateHammerCount();

        yield return new WaitForSeconds(0.8f);
    }

    // 3) Energy panels added
    private IEnumerator ApplyEnergyPanels(WheelManager wheels)
    {
        wheels.EvaluateEnergyCount();

        yield return new WaitForSeconds(0.8f);
    }

    // 4) Assassin acts
    private IEnumerator ActingAssassin(Hero[] heros)
    {
        foreach (var hero in heros)
        {
            if (hero.getHeroType() == HeroType.Assassin)
            {
                if (hero.getCanMakeAction())
                {
                    yield return StartCoroutine(hero.ActivateAction(hero.getHeroType()));
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
            if (hero.getHeroType() == HeroType.Priest)
            {
                if (hero.getCanMakeAction())
                {
                    yield return StartCoroutine(hero.ActivateAction(hero.getHeroType()));
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
            if (hero.getHeroType() == HeroType.Engineer)
            {
                if (hero.getCanMakeAction())
                {
                    yield return StartCoroutine(hero.ActivateAction(hero.getHeroType()));
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
            if (hero.getCanSendBomb())
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
            if (hero.getHeroType() == HeroType.Assassin) { validHero = false; }
            else if (hero.getHeroType() == HeroType.Priest) { validHero = false; }
            else if (hero.getHeroType() == HeroType.Engineer) { validHero = false; }

            if (validHero && hero.getCanMakeAction())
            {
                yield return StartCoroutine(hero.ActivateAction(hero.getHeroType()));
            }
            else { yield return null; }
        }
    }

    // 9) (If the second hero had enough energy from energy panels to act: Priest grants energy)
    private IEnumerator ActingPriest2(Hero[] heroes)
    {
        foreach (var hero in heroes)
        {
            foreach (var held in heroes)
            {
                if ((held != hero) && !hero.getPriestBoosted())
                {
                    yield return StartCoroutine(hero.ActivateSecondPriest(hero.getHeroType()));
                }
                else { yield return null; }
            }
        }
    }

    // 10) Hero acts from priest energy
    private IEnumerator ActingHeroesAgain(Hero[] heroes)
    {
        foreach (var hero in heroes)
        {
            if (hero.getCanMakeAction())
            {
                yield return StartCoroutine(hero.ActivateAction(hero.getHeroType()));
            }
            else { yield return null; }
        }
    }

    // 11) Bombs again
    private IEnumerator DeployingBombsAgain(Hero[] heroes)
    {
        foreach (var hero in heroes)
        {
            if (hero.getCanSendBomb())
            {
                yield return StartCoroutine(hero.SendBomb());
            }
            else { yield return null; }
        }
    }

    // 12) 0 HP Crown check
    private IEnumerator CheckCrownHP()
    {
        //Checken, ob beide Seiten verloren haben
        if ((playerCrownManager.GetCurrentHP() == 0) && (enemyCrownManager.GetCurrentHP() == 0))
        {
            DrawScreen.SetActive(true);
        }
        else if (enemyCrownManager.GetCurrentHP() == 0)
        {
            WinScreen.SetActive(true);
        }
        else
        {
            LoseScreen.SetActive(true);
        }

        yield return null;
    }
}
