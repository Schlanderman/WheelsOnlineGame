using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class HeroActionsActivator : NetworkBehaviour
{
    private CrownManager playerCrown;       //Spielerkrone
    private CrownManager enemyCrown;        //Gegnerkrone
    
    private BulwarkMover playerBulwark;     //Spielerbulwark
    private BulwarkMover enemyBulwark;      //Gegnerbulwark
    
    private ActionRodAnimManager playerRodAnimations;   //Spieleranimationen
    private ActionRodAnimManager enemyRodAnimations;    //Gegneranimationen
    
    private Hero selfThisHero;          //Der Aktive Held
    private Hero selfOtherHero;         //Der andere Held auf Spielerseite
    private Hero enemySquareHero;       //Square Held auf Gegnerseite
    private Hero enemyDiamondHero;      //Diamond Held auf Gegnerseite
    
    private HeroAnimationManager heroAnimationManager;      //Der Animationsmanager des Helden

    private string thisHeroSide = "Square";
    private string thisUserSide = "Player";

    private NetworkVariable<bool> priestBoostedOtherHero = new NetworkVariable<bool>(   //Wert, ob ein Priester den anderen Helden Energie gegeben hat
        false,  //Startwert
        NetworkVariableReadPermission.Everyone,     //Clients dürfen den Wert lesen
        NetworkVariableWritePermission.Server       //Nur der Owner darf den Wert schreiben
        );

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
    public IEnumerator ActivateHeroAction(Hero hero)
    {
        HeroType hType = hero.GetHeroType();

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
                yield break;
        }

        //Jeder Held, der eine Aktion ausführt bekommt 2 XP
        if (IsServer) { hero.AddXP(2); }
    }

    public void SendBomb()
    {
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 1;
        if (thisHeroSide == "Diamond")
        {
            rodNumber = 2;
        }

        //Animation
        heroAnimationManager.TriggerHeroAction();
        StartCoroutine(enemyRodAnimations.ActivateRodAnimation(rodNumber, "Bomb", "PopUp"));
        enemyCrown.DecreaseHPRpc(2);
    }

    //Aktionen für die einzelnen Helden
    //Warrior
    private IEnumerator WarriorAction()
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

        if (enemyBulwark.GetBulwarkLevel() >= 1)
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            yield return StartCoroutine(playerRodAnimations.ActivateRodAnimation(rodNumber, "Sword", attackSideBulwark));

            //Schaden am Bulwark
            enemyBulwark.decreaseBulwark(selfThisHero.GetBulwarkDamage());
        }
        else
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            yield return StartCoroutine(playerRodAnimations.ActivateRodAnimation(rodNumber, "Sword", attackSideCrown));

            //Schaden an der Krone
            if (IsServer) { enemyCrown.DecreaseHPRpc(selfThisHero.GetCrownDamage()); }
        }
    }

    //Mage
    private IEnumerator MageAction()
    {
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
        if (enemyBulwark.GetBulwarkLevel() >= 1)
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            yield return StartCoroutine(playerRodAnimations.ActivateRodAnimation(rodNumber, "Fireball", attackSideBulwark));

            //Schaden am Bulwark
            enemyBulwark.decreaseBulwark(selfThisHero.GetBulwarkDamage());
        }
        else
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            yield return StartCoroutine(playerRodAnimations.ActivateRodAnimation(rodNumber, "Fireball", attackSideCrown));

            //Schaden an der Krone
            if (IsServer) { enemyCrown.DecreaseHPRpc(selfThisHero.GetCrownDamage()); }
        }

        //Kurz warten, damit der nächste Sprite nicht ausversehen deaktiviert wird
        yield return new WaitForSeconds(0.2f);

        //Zweiter Angriff
        //Animation
        heroAnimationManager.TriggerHeroAction();
        yield return StartCoroutine(playerRodAnimations.ActivateRodAnimation(rodNumber, "Fireball", attackSideCrownHigh));

        //Schaden an der Krone
        if (IsServer) { enemyCrown.DecreaseHPRpc(selfThisHero.GetCrownDamage()); }
    }

    //Archer
    private IEnumerator ArcherAction()
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

        if (enemyBulwark.GetBulwarkLevel() >= 3)
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            yield return StartCoroutine(playerRodAnimations.ActivateRodAnimation(rodNumber, "Arrow", attackSideBulwark));

            //Schaden am Bulwark
            enemyBulwark.decreaseBulwark(selfThisHero.GetBulwarkDamage());
        }
        else
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            yield return StartCoroutine(playerRodAnimations.ActivateRodAnimation(rodNumber, "Arrow", attackSideCrown));

            //Schaden an der Krone
            if (IsServer) { enemyCrown.DecreaseHPRpc(selfThisHero.GetCrownDamage()); }
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
        if ((thisHeroSide == "Diamond" && thisUserSide == "Player") || (thisHeroSide == "Square" && thisUserSide == "Enemy"))
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
            heroAnimationManager.TriggerHeroAction();
            yield return StartCoroutine(playerRodAnimations.ActivateRodAnimation(rodNumber, "Hammer", attackSideBulwark));

            //Schaden am Bulwark
            enemyBulwark.decreaseBulwark(selfThisHero.GetBulwarkDamage());
        }
        else
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            yield return StartCoroutine(playerRodAnimations.ActivateRodAnimation(rodNumber, "Hammer", attackSideCrown));

            //Schaden an der Krone
            if (IsServer) { enemyCrown.DecreaseHPRpc(selfThisHero.GetCrownDamage()); }
        }

        //Kurz warten, damit der nächste Sprite nicht ausversehen deaktiviert wird
        yield return new WaitForSeconds(0.2f);

        //Zweite Aktion
        //Animation
        heroAnimationManager.TriggerHeroAction();
        yield return StartCoroutine(playerRodAnimations.ActivateRodAnimation(repairSide, "Hammer", "PopUp"));

        //Spieler Bulwark erhöhen
        if (IsServer) { playerBulwark.increaseBulwark(2); }
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
        if (enemySquareHero.GetCurrentEnergy() > enemyDiamondHero.GetCurrentEnergy())
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            yield return StartCoroutine(enemyRodAnimations.ActivateRodAnimation(0, "Dagger", "PopUp"));

            //Square Held Energie abziehen
            if (IsServer) { enemySquareHero.DecreaseEnergy(selfThisHero.GetDelayAdding()); }
            enemySquareHero.SetCanMakeAction(false);
        }
        else
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            yield return StartCoroutine(enemyRodAnimations.ActivateRodAnimation(3, "Dagger", "PopUp"));

            //Diamond Held Energie abziehen
            if (IsServer) { enemyDiamondHero.DecreaseEnergy(selfThisHero.GetDelayAdding()); }
            enemyDiamondHero.SetCanMakeAction(false);
        }

        //Kurz warten, damit der nächste Sprite nicht ausversehen deaktiviert wird
        yield return new WaitForSeconds(0.2f);

        //Zweite Aktion
        //Animation
        heroAnimationManager.TriggerHeroAction();
        yield return StartCoroutine(enemyRodAnimations.ActivateRodAnimation(rodNumber, "Dagger", "PopUp"));

        //Schaden an der Krone
        if (IsServer) { enemyCrown.DecreaseHPRpc(selfThisHero.GetCrownDamage()); }
    }

    //Priest
    private IEnumerator PriestAction()
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
        heroAnimationManager.TriggerHeroAction();
        yield return StartCoroutine(playerRodAnimations.ActivateRodAnimation(crownSide, "Book", "PopUp"));

        //Spieler-Krone heilen
        if (IsServer) { playerCrown.IncreaseHPRpc(selfThisHero.GetHealingAdding()); }

        //Kurz warten, damit der nächste Sprite nicht ausversehen deaktiviert wird
        yield return new WaitForSeconds(0.2f);

        //Zweite Aktion
        if (selfOtherHero.PriestChecksAction())
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            yield return StartCoroutine(playerRodAnimations.ActivateRodAnimation(rodNumber, "Book", "PopUp"));

            //Anderen Helden Energie geben
            if (IsServer)
            {
                selfOtherHero.AddEnergy(selfThisHero.GetEnergyAdding());
                SetPriestBoostedOtherHero(true);
            }
        }
    }

    //Priest #2
    public IEnumerator PriestSecondAction()
    {
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 3;
        if ((thisHeroSide == "Diamond" && thisUserSide == "Player") || (thisHeroSide == "Square" && thisUserSide == "Enemy"))
        {
            rodNumber = 0;
        }

        //Animation
        heroAnimationManager.TriggerHeroAction();
        yield return StartCoroutine(playerRodAnimations.ActivateRodAnimation(rodNumber, "Book", "PopUp"));

        //Anderen Helden Energie geben
        if (IsServer) { selfOtherHero.AddEnergy(selfThisHero.GetEnergyAdding()); }
    }

    public bool GetPriestBoosted()
    {
            bool temp = priestBoostedOtherHero.Value;
            priestBoostedOtherHero.Value = false;
            return temp;
    }

    private void SetPriestBoostedOtherHero(bool state)
    {
        priestBoostedOtherHero.Value = state;
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
