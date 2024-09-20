using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private WheelManager playerWheelManager;   //WheelManager des Spielers
    [SerializeField] private WheelManager enemyWheelManager;    //WheelManager des Gegners

    [SerializeField] private CrownManager playerCrownManager;   //Crownmanager des Spielers
    [SerializeField] private CrownManager enemyCrownManager;    //CrownManager des Gegners

    [SerializeField] private BulwarkMover playerBulwarkMover;   //BulwarkManager des Spielers
    [SerializeField] private BulwarkMover enemyBulwarkMover;    //BulwarkManager des Gegners

    [SerializeField] private CoverManager coverManager;         //CoverManager des Gegners

    [SerializeField] private EvaluationManager playerEvaluationManager;     //EvaluationManager des Spielers
    [SerializeField] private EvaluationManager enemyEvaluationManager;      //EvaluationManager des Gegners

    //Anzeigen für Gewinnen / Verlieren / Unentschienden
    [SerializeField] private GameObject WinScreen;
    [SerializeField] private GameObject LoseScreen;
    [SerializeField] private GameObject DrawScreen;

    private readonly Hero[] playerHeroes = new Hero[2];     //Helden des Spielers
    private readonly Hero[] enemyHeroes = new Hero[2];      //Helden des Gegners

    private int currentTurnStep = 1;

    private void Start()
    {
        WinScreen.SetActive(false);
        LoseScreen.SetActive(false);
        DrawScreen.SetActive(false);
    }

    public void SetHeroes(Hero playerSquare, Hero playerDiamond, Hero enemySquare, Hero enemyDiamond)
    {
        playerHeroes[0] = playerSquare;
        playerHeroes[1] = playerDiamond;

        enemyHeroes[0] = enemySquare;
        enemyHeroes[1] = enemyDiamond;
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
                //Kurz warten vor der ersten Aktion
                yield return new WaitForSeconds(1f);

                //Cover herunterfahren
                yield return StartCoroutine(coverManager.SetCoverDown());
                break;

            case 2:
                //XP Panel, Level ups
                yield return StartCoroutine(ApplyPanelXPAndLevelUps(playerWheelManager, playerEvaluationManager));
                yield return StartCoroutine(ApplyPanelXPAndLevelUps(enemyWheelManager, enemyEvaluationManager));
                break;

            case 3:
                //Hammer panels added
                yield return StartCoroutine(ApplyHammerPanels(playerWheelManager, playerEvaluationManager));
                yield return StartCoroutine(ApplyHammerPanels(enemyWheelManager, enemyEvaluationManager));
                break;

            case 4:
                //Energy panels added
                yield return StartCoroutine(ApplyEnergyPanels(playerWheelManager, playerEvaluationManager));
                yield return StartCoroutine(ApplyEnergyPanels(enemyWheelManager, enemyEvaluationManager));
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
            yield return StartCoroutine(coverManager.SetCoverUp());
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
    private IEnumerator ApplyPanelXPAndLevelUps(WheelManager wheels, EvaluationManager evalManager)
    {
        evalManager.EvaluateXPGained(wheels.GetWheels());

        yield return new WaitForSeconds(0.8f);
    }

    // 2) Hammer panels added
    private IEnumerator ApplyHammerPanels(WheelManager wheels, EvaluationManager evalManager)
    {
        evalManager.EvaluateHammerCount(wheels.GetWheels());

        yield return new WaitForSeconds(0.8f);
    }

    // 3) Energy panels added
    private IEnumerator ApplyEnergyPanels(WheelManager wheels, EvaluationManager evalManager)
    {
        evalManager.EvaluateEnergyCount(wheels.GetWheels());

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
        //Checken, ob beide Seiten verloren haben
        if ((playerCrownManager.GetCurrentHP() == 0) && (enemyCrownManager.GetCurrentHP() == 0))
        {
            DrawScreen.SetActive(true);
        }
        else if (enemyCrownManager.GetCurrentHP() == 0)
        {
            WinScreen.SetActive(true);
        }
        else if (playerCrownManager.GetCurrentHP() == 0)
        {
            LoseScreen.SetActive(true);
        }

        yield return null;
    }
}
