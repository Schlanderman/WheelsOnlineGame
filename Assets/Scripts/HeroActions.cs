using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroActions : MonoBehaviour
{
    [SerializeField] private CrownManager playerCrown;      //Spielerkrone
    [SerializeField] private CrownManager enemyCrown;       //Gegnerkrone

    [SerializeField] private BulwarkMover playerBulwark;    //Spielerbulwark
    [SerializeField] private BulwarkMover enemyBulwark;     //Gegnerbulwark

    private Hero selfThisHero;          //Der Aktive Held
    private Hero selfOtherHero;         //Der andere Held auf Spielerseite
    private Hero enemySquareHero;       //Square Held auf Gegnerseite
    private Hero enemyDiamondHero;      //Diamond Held auf Gegnerseite

    //Methode um Spielerhelden zuzuweisen
    public void SetPlayerHeroes(Hero thisHero, Hero otherHero)
    {
        selfThisHero = thisHero;
        selfOtherHero = otherHero;
    }

    //Methode um die Gegnerhelden zuzuweisen
    public void SetEnemyHeroes(Hero square, Hero diamond)
    {
        enemySquareHero = square;
        enemyDiamondHero = diamond;
    }

    //Auswählen, welcher Held die Aktion ausführt
    public void ActivateHeroAction(Hero hero)
    {
        HeroType hType = hero.getHeroType();

        switch (hType)
        {
            case HeroType.Warrior:
                StartCoroutine(WarriorAction());
                break;

            case HeroType.Mage:
                StartCoroutine(MageAction());
                break;

            case HeroType.Archer:
                StartCoroutine(ArcherAction());
                break;

            case HeroType.Engineer:
                StartCoroutine(EngineerAction());
                break;

            case HeroType.Assassin:
                StartCoroutine(AssassinAction());
                break;

            case HeroType.Priest:
                StartCoroutine(PriestAction());
                break;

            default:
                Debug.LogError(hType + " ist kein valider HeroType!");
                break;
        }
    }

    //Aktionen für die einzelnen Helden
    //TODO Animationen einfügen!

    //Warrior
    private IEnumerator WarriorAction()
    {
        if (enemyBulwark.GetBulwarkLevel() >= 1)
        {
            //Animation
            yield return null;

            //Schaden am Bulwark
            enemyBulwark.decreaseBulwark(selfThisHero.getBulwarkDamage());
        }
        else
        {
            //Animation
            yield return null;

            //Schaden an der Krone
            enemyCrown.DecreaseHP(selfThisHero.getCrownDamage());
        }
    }

    //Mage
    private IEnumerator MageAction()
    {
        //Erster Angriff
        if (enemyBulwark.GetBulwarkLevel() >= 1)
        {
            //Animation
            yield return null;

            //Schaden am Bulwark
            enemyBulwark.decreaseBulwark(selfThisHero.getBulwarkDamage());
        }
        else
        {
            //Animation
            yield return null;

            //Schaden an der Krone
            enemyCrown.DecreaseHP(selfThisHero.getCrownDamage());
        }

        //Zweiter Angriff
        //Animation
        yield return null;

        //Schaden an der Krone
        enemyCrown.DecreaseHP(selfThisHero.getCrownDamage());
    }

    //Archer
    private IEnumerator ArcherAction()
    {
        if (enemyBulwark.GetBulwarkLevel() >= 3)
        {
            //Animation
            yield return null;

            //Schaden am Bulwark
            enemyBulwark.decreaseBulwark(selfThisHero.getBulwarkDamage());
        }
        else
        {
            //Animation
            yield return null;

            //Schaden an der Krone
            enemyCrown.DecreaseHP(selfThisHero.getCrownDamage());
        }
    }

    //Engineer
    private IEnumerator EngineerAction()
    {
        //Erste Aktion
        if (enemyBulwark.GetBulwarkLevel() >= 1)
        {
            //Animation
            yield return null;

            //Schaden am Bulwark
            enemyBulwark.decreaseBulwark(selfThisHero.getBulwarkDamage());
        }
        else
        {
            //Animation
            yield return null;

            //Schaden an der Krone
            enemyCrown.DecreaseHP(selfThisHero.getCrownDamage());
        }

        //Zweite Aktion
        //Animation
        yield return null;

        //Spieler Bulwark erhöhen
        playerBulwark.increaseBulwark(2);
    }

    //Assassin
    private IEnumerator AssassinAction()
    {
        //Erste Aktion
        if (enemySquareHero.getCurrentEnergy() > enemyDiamondHero.getCurrentEnergy())
        {
            //Animation
            yield return null;

            //Square Held Energie abziehen
            enemySquareHero.DecreaseEnergy(selfThisHero.getDelayAdding());
        }
        else
        {
            //Animation
            yield return null;

            //Diamond Held Energie abziehen
            enemyDiamondHero.DecreaseEnergy(selfThisHero.getDelayAdding());
        }

        //Zweite Aktion
        //Animation
        yield return null;

        //Schaden an der Krone
        enemyCrown.DecreaseHP(selfThisHero.getCrownDamage());
    }

    //Priest
    private IEnumerator PriestAction()
    {
        //Erste Aktion
        //Animation
        yield return null;

        //Spieler-Krone heilen
        playerCrown.IncreaseHP(selfThisHero.getHealingAdding());

        //Zweite Aktion
        //Animation
        yield return null;

        //Anderen Helden Energie geben
        selfOtherHero.AddEnergy(selfThisHero.getEnergyAdding());
    }
}
