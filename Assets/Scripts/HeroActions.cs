using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroActions : MonoBehaviour
{
    [SerializeField] private CrownManager playerCrown;      //Spielerkrone
    [SerializeField] private CrownManager enemyCrown;       //Gegnerkrone

    [SerializeField] private BulwarkMover playerBulwark;    //Spielerbulwark
    [SerializeField] private BulwarkMover enemyBulwark;     //Gegnerbulwark

    [SerializeField] private ActionRodAnimManager rodAnimations;        //Spieleranimationen
    [SerializeField] private ActionRodAnimManager enemyRodAnimations;   //Gegneranimationen

    [SerializeField] private Hero selfThisHero;          //Der Aktive Held
    [SerializeField] private Hero selfOtherHero;         //Der andere Held auf Spielerseite
    [SerializeField] private Hero enemySquareHero;       //Square Held auf Gegnerseite
    [SerializeField] private Hero enemyDiamondHero;      //Diamond Held auf Gegnerseite

    private HeroAnimationManager heroAnimationManager;      //Der Animationsmanager des Helden

    private string thisHeroSide = "Square";
    //private string otherHeroSide = "Diamond";

    private bool priestBoostedOtherHero = false;     //Wert, ob ein Priester den anderen Helden Energie gegeben hat

    //Methode um Spielerhelden zuzuweisen
    public void SetPlayerHeroes(Hero thisHero, Hero otherHero)
    {
        selfThisHero = thisHero;
        selfOtherHero = otherHero;

        heroAnimationManager = this.GetComponent<HeroAnimationManager>();
        //Debug.Log("AnimationManager in " + this + " ist " + heroAnimationManager);
    }

    //Methode um die Gegnerhelden zuzuweisen
    public void SetEnemyHeroes(Hero square, Hero diamond)
    {
        enemySquareHero = square;
        enemyDiamondHero = diamond;
    }

    //Methode, um die Seiten zuzuweisen
    public void SetSquareSideMain()
    {
        thisHeroSide = "Square";
        //otherHeroSide = "Diamond";
    }

    public void SetDiamondSideMain()
    {
        thisHeroSide = "Diamond";
        //otherHeroSide = "Square";
    }

    //Auswählen, welcher Held die Aktion ausführt
    public IEnumerator ActivateHeroAction(Hero hero)
    {
        HeroType hType = hero.getHeroType();
        Debug.Log(hero + " führt seine Aktion aus in HeroActions und ist Type: " + hType);

        switch (hType)
        {
            case HeroType.Warrior:
                yield return StartCoroutine(WarriorAction());
                break;

            case HeroType.Mage:
                yield return StartCoroutine(MageAction());
                break;

            case HeroType.Archer:
                yield return StartCoroutine(ArcherAction());
                break;

            case HeroType.Engineer:
                yield return StartCoroutine(EngineerAction());
                break;

            case HeroType.Assassin:
                yield return StartCoroutine(AssassinAction());
                break;

            case HeroType.Priest:
                yield return StartCoroutine(PriestAction());
                break;

            default:
                Debug.LogError(hType + " ist kein valider HeroType!");
                yield return null;
                break;
        }

        //Jeder Held, der eine Aktion ausführt bekommt 2 XP
        hero.AddXP(2);
    }

    public IEnumerator SendBomb()
    {
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 1;
        if (thisHeroSide == "Diamond")
        {
            rodNumber = 2;
        }

        //Animation
        heroAnimationManager.TriggerHeroAction(thisHeroSide);
        yield return StartCoroutine(rodAnimations.ActivateRodAnimation(rodNumber, "Bomb", "PopUp"));
    }

    //Aktionen für die einzelnen Helden
    //Warrior
    private IEnumerator WarriorAction()
    {
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 0;
        string attackSideBulwark = "AttackRightBulwark";
        string attackSideCrown = "AttackRightCrown";
        if (thisHeroSide == "Diamond")
        {
            rodNumber = 3;
            attackSideBulwark = "AttackLeftBulwark";
            attackSideCrown = "AttackLeftCrown";
        }

        if (enemyBulwark.GetBulwarkLevel() >= 1)
        {
            //Animation
            heroAnimationManager.TriggerHeroAction(thisHeroSide);
            yield return StartCoroutine(rodAnimations.ActivateRodAnimation(rodNumber, "Sword", attackSideBulwark));

            //Schaden am Bulwark
            enemyBulwark.decreaseBulwark(selfThisHero.getBulwarkDamage());
        }
        else
        {
            //Animation
            heroAnimationManager.TriggerHeroAction(thisHeroSide);
            yield return StartCoroutine(rodAnimations.ActivateRodAnimation(rodNumber, "Sword", attackSideCrown));

            //Schaden an der Krone
            enemyCrown.DecreaseHP(selfThisHero.getCrownDamage());
        }
    }

    //Mage
    private IEnumerator MageAction()
    {
        Debug.Log("MageAction wird ausgeführt!");
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 0;
        string attackSideBulwark = "AttackRightBulwark";
        string attackSideCrown = "AttackRightCrown";
        string attackSideCrownHigh = "FireBallRightHigh";
        if (thisHeroSide == "Diamond")
        {
            rodNumber = 3;
            attackSideBulwark = "AttackLeftBulwark";
            attackSideCrown = "AttackLeftCrown";
            attackSideCrownHigh = "FireBallLeftHigh";
        }

        //Erster Angriff
        if (enemyBulwark.GetBulwarkLevel() >= 1)
        {
            //Animation
            Debug.Log("Erster Angriff mit Bulwarklevel: " + enemyBulwark);
            heroAnimationManager.TriggerHeroAction(thisHeroSide);
            yield return StartCoroutine(rodAnimations.ActivateRodAnimation(rodNumber, "Fireball", attackSideBulwark));

            //Schaden am Bulwark
            enemyBulwark.decreaseBulwark(selfThisHero.getBulwarkDamage());
        }
        else
        {
            //Animation
            Debug.Log("Erster Angriff ohne Bulwark");
            heroAnimationManager.TriggerHeroAction(thisHeroSide);
            yield return StartCoroutine(rodAnimations.ActivateRodAnimation(rodNumber, "Fireball", attackSideCrown));

            //Schaden an der Krone
            enemyCrown.DecreaseHP(selfThisHero.getCrownDamage());
        }

        //Zweiter Angriff
        //Animation
        Debug.Log("Zweiter Angriff!");
        heroAnimationManager.TriggerHeroAction(thisHeroSide);
        yield return StartCoroutine(rodAnimations.ActivateRodAnimation(rodNumber, "Fireball", attackSideCrownHigh));

        //Schaden an der Krone
        enemyCrown.DecreaseHP(selfThisHero.getCrownDamage());
    }

    //Archer
    private IEnumerator ArcherAction()
    {
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 0;
        string attackSideBulwark = "ArrowRightBulwark";
        string attackSideCrown = "ArrowRightCrown";
        if (thisHeroSide == "Diamond")
        {
            rodNumber = 3;
            attackSideBulwark = "ArrowLeftBulwark";
            attackSideCrown = "ArrowLeftCrown";
        }

        if (enemyBulwark.GetBulwarkLevel() >= 3)
        {
            //Animation
            heroAnimationManager.TriggerHeroAction(thisHeroSide);
            yield return StartCoroutine(rodAnimations.ActivateRodAnimation(rodNumber, "Arrow", attackSideBulwark));

            //Schaden am Bulwark
            enemyBulwark.decreaseBulwark(selfThisHero.getBulwarkDamage());
        }
        else
        {
            //Animation
            heroAnimationManager.TriggerHeroAction(thisHeroSide);
            yield return StartCoroutine(rodAnimations.ActivateRodAnimation(rodNumber, "Arrow", attackSideCrown));

            //Schaden an der Krone
            enemyCrown.DecreaseHP(selfThisHero.getCrownDamage());
        }
    }

    //Engineer
    private IEnumerator EngineerAction()
    {
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 0;
        int repairSide = 1;
        string attackSideBulwark = "AttackRightBulwark";
        string attackSideCrown = "AttackRightCrown";
        if (thisHeroSide == "Diamond")
        {
            rodNumber = 3;
            repairSide = 2;
            attackSideBulwark = "AttackLeftBulwark";
            attackSideCrown = "AttackLeftCrown";
        }

        //Erste Aktion
        if (enemyBulwark.GetBulwarkLevel() >= 1)
        {
            //Animation
            heroAnimationManager.TriggerHeroAction(thisHeroSide);
            yield return StartCoroutine(rodAnimations.ActivateRodAnimation(rodNumber, "Hammer", attackSideBulwark));

            //Schaden am Bulwark
            enemyBulwark.decreaseBulwark(selfThisHero.getBulwarkDamage());
        }
        else
        {
            //Animation
            heroAnimationManager.TriggerHeroAction(thisHeroSide);
            yield return StartCoroutine(rodAnimations.ActivateRodAnimation(rodNumber, "Hammer", attackSideCrown));

            //Schaden an der Krone
            enemyCrown.DecreaseHP(selfThisHero.getCrownDamage());
        }

        //Zweite Aktion
        //Animation
        heroAnimationManager.TriggerHeroAction(thisHeroSide);
        yield return StartCoroutine(rodAnimations.ActivateRodAnimation(repairSide, "Hammer", "PopUp"));

        //Spieler Bulwark erhöhen
        playerBulwark.increaseBulwark(2);
    }

    //Assassin
    private IEnumerator AssassinAction()
    {
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 1;
        if (thisHeroSide == "Diamond")
        {
            rodNumber = 2;
        }

        //Erste Aktion
        if (enemySquareHero.getCurrentEnergy() > enemyDiamondHero.getCurrentEnergy())
        {
            //Animation
            heroAnimationManager.TriggerHeroAction(thisHeroSide);
            yield return StartCoroutine(enemyRodAnimations.ActivateRodAnimation(0, "Dagger", "PopUp"));

            //Square Held Energie abziehen
            enemySquareHero.DecreaseEnergy(selfThisHero.getDelayAdding());
        }
        else
        {
            //Animation
            heroAnimationManager.TriggerHeroAction(thisHeroSide);
            yield return StartCoroutine(enemyRodAnimations.ActivateRodAnimation(3, "Dagger", "PopUp"));

            //Diamond Held Energie abziehen
            enemyDiamondHero.DecreaseEnergy(selfThisHero.getDelayAdding());
        }

        //Zweite Aktion
        //Animation
        heroAnimationManager.TriggerHeroAction(thisHeroSide);
        yield return StartCoroutine(enemyRodAnimations.ActivateRodAnimation(rodNumber, "Dagger", "PopUp"));

        //Schaden an der Krone
        enemyCrown.DecreaseHP(selfThisHero.getCrownDamage());
    }

    //Priest
    private IEnumerator PriestAction()
    {
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 3;
        int crownSide = 1;
        if (thisHeroSide == "Diamond")
        {
            rodNumber = 0;
            crownSide = 2;
        }

        //Erste Aktion
        //Animation
        heroAnimationManager.TriggerHeroAction(thisHeroSide);
        yield return StartCoroutine(rodAnimations.ActivateRodAnimation(crownSide, "Book", "PopUp"));

        //Spieler-Krone heilen
        playerCrown.IncreaseHP(selfThisHero.getHealingAdding());

        //Zweite Aktion
        if (selfOtherHero.priestChecksAction())
        {
            //Animation
            heroAnimationManager.TriggerHeroAction(thisHeroSide);
            yield return StartCoroutine(rodAnimations.ActivateRodAnimation(rodNumber, "Book", "PopUp"));

            //Anderen Helden Energie geben
            selfOtherHero.AddEnergy(selfThisHero.getEnergyAdding());
            priestBoostedOtherHero = true;
        }
    }

    //Priest #2
    public IEnumerator PriestSecondAction()
    {
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 3;
        if (thisHeroSide == "Diamond")
        {
            rodNumber = 0;
        }

        //Animation
        heroAnimationManager.TriggerHeroAction(thisHeroSide);
        yield return StartCoroutine(rodAnimations.ActivateRodAnimation(rodNumber, "Book", "PopUp"));

        //Anderen Helden Energie geben
        selfOtherHero.AddEnergy(selfThisHero.getEnergyAdding());
    }

    public bool GetPriestBoosted()
    {
        bool temp = priestBoostedOtherHero;
        priestBoostedOtherHero = false;
        return temp;
    }
}
