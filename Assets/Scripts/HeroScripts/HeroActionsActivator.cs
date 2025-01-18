using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class HeroActionsActivator : NetworkBehaviour
{
    [SerializeField] private CrownManager playerCrown;       //Spielerkrone
    [SerializeField] private CrownManager enemyCrown;        //Gegnerkrone
    
    [SerializeField] private BulwarkMover playerBulwark;     //Spielerbulwark
    [SerializeField] private BulwarkMover enemyBulwark;      //Gegnerbulwark
    
    [SerializeField] private ActionRodAnimManager playerRodAnimations;   //Spieleranimationen
    [SerializeField] private ActionRodAnimManager enemyRodAnimations;    //Gegneranimationen
    
    [SerializeField] private Hero selfThisHero;          //Der Aktive Held
    [SerializeField] private Hero selfOtherHero;         //Der andere Held auf Spielerseite
    [SerializeField] private Hero enemySquareHero;       //Square Held auf Gegnerseite
    [SerializeField] private Hero enemyDiamondHero;      //Diamond Held auf Gegnerseite
    
    [SerializeField] private HeroAnimationManager heroAnimationManager;      //Der Animationsmanager des Helden

    private string thisHeroSide = "Square";
    private string thisUserSide = "Player";

    private bool priestBoostedOtherHero = false;     //Wert, ob ein Priester den anderen Helden Energie gegeben hat

    //Methode um alle Felder zu befüllen
    public void SetManagers(
        CrownManager playerCM, CrownManager enemyCM,
        BulwarkMover playerBM, BulwarkMover enemyBM,
        ActionRodAnimManager playerARAM, ActionRodAnimManager enemyARAM,
        Hero selfHeroOne, Hero selfHeroTwo, Hero enemyHeroOne, Hero enemyHeroTwo,
        HeroAnimationManager heroAM,
        string heroSideString, string userSideString
        )
    {
        playerCrown = playerCM;
        enemyCrown = enemyCM;

        playerBulwark = playerBM;
        enemyBulwark = enemyBM;

        playerRodAnimations = playerARAM;
        enemyRodAnimations = enemyARAM;

        selfThisHero = selfHeroOne;
        selfOtherHero = selfHeroTwo;
        enemySquareHero = enemyHeroOne;
        enemyDiamondHero = enemyHeroTwo;

        heroAnimationManager = heroAM;

        thisHeroSide = heroSideString;
        thisUserSide = userSideString;
    }

    //Auswählen, welcher Held die Aktion ausführt
    public float ActivateHeroAction(Hero hero)
    {
        HeroType hType = hero.GetHeroType();
        float finalDelay = 0;

        switch (hType)
        {
            case HeroType.Warrior:
                finalDelay = WarriorAction();
                break;

            case HeroType.Mage:
                finalDelay = MageAction();
                break;

            case HeroType.Archer:
                finalDelay = ArcherAction();
                break;

            case HeroType.Engineer:
                finalDelay = EngineerAction();
                break;

            case HeroType.Assassin:
                finalDelay = AssassinAction();
                break;

            case HeroType.Priest:
                finalDelay = PriestAction();
                break;

            default:
                Debug.LogError(hType + " ist kein valider HeroType!");
                break;
        }

        //Jeder Held, der eine Aktion ausführt bekommt 2 XP
        DelayedFunctionCallCoroutine(hero.AddXP, 2, finalDelay);
        return finalDelay;
    }

    public float SendBomb()
    {
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 1;
        if (thisHeroSide == "Diamond")
        {
            rodNumber = 2;
        }

        //Animation
        heroAnimationManager.TriggerHeroAction();
        return playerRodAnimations.ActivateRodAnimation(rodNumber, "Bomb", "PopUp");
    }

    //Aktionen für die einzelnen Helden
    //Warrior
    private float WarriorAction()
    {
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 0;
        string attackSideBulwark = "AttackRightBulwark";
        string attackSideCrown = "AttackRightCrown";
        if ((thisHeroSide == "Diamond" && thisUserSide == "Player") || (thisHeroSide == "Square" && thisUserSide == "Enemy"))
        {
            rodNumber = 3;
            attackSideBulwark = "AttackLeftBulwark";
            attackSideCrown = "AttackLeftCrown";
        }

        float finalDelay = 0f;
        if (enemyBulwark.GetBulwarkLevel() >= 1)
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            finalDelay = playerRodAnimations.ActivateRodAnimation(rodNumber, "Sword", attackSideBulwark);

            //Schaden am Bulwark
            DelayedFunctionCallCoroutine(enemyBulwark.decreaseBulwark, selfThisHero.GetBulwarkDamage(), finalDelay);
        }
        else
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            finalDelay = playerRodAnimations.ActivateRodAnimation(rodNumber, "Sword", attackSideCrown);

            //Schaden an der Krone
            if (IsServer) { DelayedFunctionCallCoroutine(enemyCrown.DecreaseHPRpc, selfThisHero.GetCrownDamage(), finalDelay); }
        }

        return finalDelay;
    }

    //Mage
    private float MageAction()
    {
        Debug.Log("MageAction wird ausgeführt!");
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 0;
        string attackSideBulwark = "AttackRightBulwark";
        string attackSideCrown = "AttackRightCrown";
        string attackSideCrownHigh = "FireballRightHigh";
        if ((thisHeroSide == "Diamond" && thisUserSide == "Player") || (thisHeroSide == "Square" && thisUserSide == "Enemy"))
        {
            rodNumber = 3;
            attackSideBulwark = "AttackLeftBulwark";
            attackSideCrown = "AttackLeftCrown";
            attackSideCrownHigh = "FireballLeftHigh";
        }

        //Erster Angriff
        float finalDelay = 0f;
        if (enemyBulwark.GetBulwarkLevel() >= 1)
        {
            //Animation
            Debug.Log("Erster Angriff mit Bulwarklevel: " + enemyBulwark);
            heroAnimationManager.TriggerHeroAction();
            finalDelay = playerRodAnimations.ActivateRodAnimation(rodNumber, "Fireball", attackSideBulwark);

            //Schaden am Bulwark
            DelayedFunctionCallCoroutine(enemyBulwark.decreaseBulwark, selfThisHero.GetBulwarkDamage(), finalDelay);
        }
        else
        {
            //Animation
            Debug.Log("Erster Angriff ohne Bulwark");
            heroAnimationManager.TriggerHeroAction();
            finalDelay = playerRodAnimations.ActivateRodAnimation(rodNumber, "Fireball", attackSideCrown);

            //Schaden an der Krone
            if (IsServer) { DelayedFunctionCallCoroutine(enemyCrown.DecreaseHPRpc, selfThisHero.GetCrownDamage(), finalDelay); }
        }

        //Zweiter Angriff
        //Animation
        Debug.Log("Zweiter Angriff!");
        heroAnimationManager.TriggerHeroAction();
        finalDelay += playerRodAnimations.ActivateRodAnimation(rodNumber, "Fireball", attackSideCrownHigh);

        //Schaden an der Krone
        if (IsServer) { DelayedFunctionCallCoroutine(enemyCrown.DecreaseHPRpc, selfThisHero.GetCrownDamage(), finalDelay); }

        return finalDelay;
    }

    //Archer
    private float ArcherAction()
    {
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 0;
        string attackSideBulwark = "ArrowRightBulwark";
        string attackSideCrown = "ArrowRightCrown";
        if ((thisHeroSide == "Diamond" && thisUserSide == "Player") || (thisHeroSide == "Square" && thisUserSide == "Enemy"))
        {
            rodNumber = 3;
            attackSideBulwark = "ArrowLeftBulwark";
            attackSideCrown = "ArrowLeftCrown";
        }

        float finalDelay = 0f;
        if (enemyBulwark.GetBulwarkLevel() >= 3)
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            finalDelay = playerRodAnimations.ActivateRodAnimation(rodNumber, "Arrow", attackSideBulwark);

            //Schaden am Bulwark
            DelayedFunctionCallCoroutine(enemyBulwark.decreaseBulwark, selfThisHero.GetBulwarkDamage(), finalDelay);
        }
        else
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            finalDelay = playerRodAnimations.ActivateRodAnimation(rodNumber, "Arrow", attackSideCrown);

            //Schaden an der Krone
            if (IsServer) { DelayedFunctionCallCoroutine(enemyCrown.DecreaseHPRpc, selfThisHero.GetCrownDamage(), finalDelay); }
        }

        return finalDelay;
    }

    //Engineer
    private float EngineerAction()
    {
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 0;
        int repairSide = 1;
        string attackSideBulwark = "AttackRightBulwark";
        string attackSideCrown = "AttackRightCrown";
        if ((thisHeroSide == "Diamond" && thisUserSide == "Player") || (thisHeroSide == "Square" && thisUserSide == "Enemy"))
        {
            rodNumber = 3;
            repairSide = 2;
            attackSideBulwark = "AttackLeftBulwark";
            attackSideCrown = "AttackLeftCrown";
        }

        //Erste Aktion
        float finalDelay = 0f;
        if (enemyBulwark.GetBulwarkLevel() >= 1)
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            finalDelay = playerRodAnimations.ActivateRodAnimation(rodNumber, "Hammer", attackSideBulwark);

            //Schaden am Bulwark
            DelayedFunctionCallCoroutine(enemyBulwark.decreaseBulwark, selfThisHero.GetBulwarkDamage(), finalDelay);
        }
        else
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            finalDelay = playerRodAnimations.ActivateRodAnimation(rodNumber, "Hammer", attackSideCrown);

            //Schaden an der Krone
            if (IsServer) { DelayedFunctionCallCoroutine(enemyCrown.DecreaseHPRpc, selfThisHero.GetCrownDamage(), finalDelay); }
        }

        //Zweite Aktion
        //Animation
        heroAnimationManager.TriggerHeroAction();
        finalDelay += playerRodAnimations.ActivateRodAnimation(repairSide, "Hammer", "PopUp");

        //Spieler Bulwark erhöhen
        DelayedFunctionCallCoroutine(playerBulwark.increaseBulwark, 2, finalDelay);
        return finalDelay;
    }

    //Assassin
    private float AssassinAction()
    {
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 1;
        if (thisHeroSide == "Diamond")
        {
            rodNumber = 2;
        }

        //Erste Aktion
        float finalDelay = 0f;
        if (enemySquareHero.GetCurrentEnergy() > enemyDiamondHero.GetCurrentEnergy())
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            finalDelay = enemyRodAnimations.ActivateRodAnimation(0, "Dagger", "PopUp");

            //Square Held Energie abziehen
            DelayedFunctionCallCoroutine(enemySquareHero.DecreaseEnergy, selfThisHero.GetDelayAdding(), finalDelay);
            DelayedFunctionCallCoroutine(enemySquareHero.SetCanMakeAction, false, finalDelay);
        }
        else
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            finalDelay = enemyRodAnimations.ActivateRodAnimation(3, "Dagger", "PopUp");

            //Diamond Held Energie abziehen
            DelayedFunctionCallCoroutine(enemyDiamondHero.DecreaseEnergy, selfThisHero.GetDelayAdding(), finalDelay);
            DelayedFunctionCallCoroutine(enemyDiamondHero.SetCanMakeAction, false, finalDelay);
        }

        //Zweite Aktion
        //Animation
        heroAnimationManager.TriggerHeroAction();
        finalDelay += enemyRodAnimations.ActivateRodAnimation(rodNumber, "Dagger", "PopUp");

        //Schaden an der Krone
        if (IsServer) { DelayedFunctionCallCoroutine(enemyCrown.DecreaseHPRpc, selfThisHero.GetCrownDamage(), finalDelay); }

        return finalDelay;
    }

    //Priest
    private float PriestAction()
    {
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 3;
        int crownSide = 1;
        if ((thisHeroSide == "Diamond" && thisUserSide == "Player") || (thisHeroSide == "Square" && thisUserSide == "Enemy"))
        {
            rodNumber = 0;
            crownSide = 2;
        }

        //Erste Aktion
        //Animation
        float finalDelay = 0f;
        heroAnimationManager.TriggerHeroAction();
        finalDelay = playerRodAnimations.ActivateRodAnimation(crownSide, "Book", "PopUp");

        //Spieler-Krone heilen
        if (IsServer) { DelayedFunctionCallCoroutine(playerCrown.IncreaseHPRpc, selfThisHero.GetHealingAdding(), finalDelay); }

        //Zweite Aktion
        if (selfOtherHero.PriestChecksAction())
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            finalDelay += playerRodAnimations.ActivateRodAnimation(rodNumber, "Book", "PopUp");

            //Anderen Helden Energie geben
            DelayedFunctionCallCoroutine(selfOtherHero.AddEnergy, selfThisHero.GetEnergyAdding(), finalDelay);
            DelayedFunctionCallCoroutine(SetPriestBoostedOtherHero, true, finalDelay);
        }

        return finalDelay;
    }

    //Priest #2
    public float PriestSecondAction()
    {
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 3;
        if ((thisHeroSide == "Diamond" && thisUserSide == "Player") || (thisHeroSide == "Square" && thisUserSide == "Enemy"))
        {
            rodNumber = 0;
        }

        //Animation
        float finalDelay = 0f;
        heroAnimationManager.TriggerHeroAction();
        finalDelay = playerRodAnimations.ActivateRodAnimation(rodNumber, "Book", "PopUp");

        //Anderen Helden Energie geben
        DelayedFunctionCallCoroutine(selfOtherHero.AddEnergy, selfThisHero.GetEnergyAdding(), finalDelay);

        return finalDelay;
    }

    public bool GetPriestBoosted()
    {
        bool temp = priestBoostedOtherHero;
        priestBoostedOtherHero = false;
        return temp;
    }

    private void SetPriestBoostedOtherHero(bool state)
    {
        priestBoostedOtherHero = state;
    }



    //Aktionen wie BulwarkDamage oder AddEnergy erst nach ablauf der RodAnimation ausführen
    private void DelayedFunctionCallCoroutine<T>(Action<T> functionToCall, T parameter, float delayInSeconds)
    {
        StartCoroutine(ExecuteWithDelay(functionToCall, parameter, delayInSeconds));
    }

    private IEnumerator ExecuteWithDelay<T>(Action<T> functionToCall, T parameter, float delayInSeconds)
    {
        //Wartezeit
        yield return new WaitForSeconds(delayInSeconds);

        //Aufruf der übergebenen Funktion
        functionToCall?.Invoke(parameter);
    }
}
