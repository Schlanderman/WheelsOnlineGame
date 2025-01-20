using UnityEngine;

public class HeroActionsLengthManager : MonoBehaviour
{
    [SerializeField] private Hero selfOtherHero;  //Dieser Held

    [SerializeField] private BulwarkMover enemyBulwark;  //Bulwark des Gegners

    [SerializeField] private ActionRodAnimManager playerRodAnimations;   //Spieleranimationen
    [SerializeField] private ActionRodAnimManager enemyRodAnimations;    //Gegneranimationen

    private string thisHeroSide = "Square";
    private string thisUserSide = "Player";

    //Methode um alle Felder zu befüllen
    public void SetManagers(
        Hero otherHero,
        BulwarkMover enemyBM,
        ActionRodAnimManager playerARAM, ActionRodAnimManager enemyARAM,
        string heroSideString, string userSideString
        )
    {
        selfOtherHero = otherHero;

        enemyBulwark = enemyBM;

        playerRodAnimations = playerARAM;
        enemyRodAnimations = enemyARAM;

        thisHeroSide = heroSideString;
        thisUserSide = userSideString;
    }

    //Länge an Hero übergeben
    public float GetAnimationLength(Hero hero)
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

        return finalDelay;
    }

    private float WarriorAction()
    {
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 0;
        string attackSideBulwark = "ActionRodAttackRightBulwark";
        string attackSideCrown = "ActionRodAttackRightCrown";
        if ((thisHeroSide == "Diamond" && thisUserSide == "Player") || (thisHeroSide == "Square" && thisUserSide == "Enemy"))
        {
            rodNumber = 3;
            attackSideBulwark = "ActionRodAttackLeftBulwark";
            attackSideCrown = "ActionRodAttackLeftCrown";
        }

        float animationLength = 0f;
        if (enemyBulwark.GetBulwarkLevel() >= 1)
        {
            animationLength = playerRodAnimations.GetAnimationLength(rodNumber, attackSideBulwark);
        }
        else
        {
            animationLength = playerRodAnimations.GetAnimationLength(rodNumber, attackSideCrown);
        }

        return animationLength;
    }

    private float MageAction()
    {
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 0;
        string attackSideBulwark = "ActionRodAttackRightBulwark";
        string attackSideCrown = "ActionRodAttackRightCrown";
        string attackSideCrownHigh = "ActionRodFireballHighRightCrown";
        if ((thisHeroSide == "Diamond" && thisUserSide == "Player") || (thisHeroSide == "Square" && thisUserSide == "Enemy"))
        {
            rodNumber = 3;
            attackSideBulwark = "ActionRodAttackLeftBulwark";
            attackSideCrown = "ActionRodAttackLeftCrown";
            attackSideCrownHigh = "ActionRodFireballHighLeftCrown";
        }

        //Erster Angriff
        float animationLength = 0f;
        if (enemyBulwark.GetBulwarkLevel() >= 1)
        {
            animationLength = playerRodAnimations.GetAnimationLength(rodNumber, attackSideBulwark);
        }
        else
        {
            animationLength = playerRodAnimations.GetAnimationLength(rodNumber, attackSideCrown);
        }

        //Zweiter Angriff
        animationLength += playerRodAnimations.GetAnimationLength(rodNumber, attackSideCrownHigh);

        return animationLength;
    }

    private float ArcherAction()
    {
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 0;
        string attackSideBulwark = "ActionRodArrowRightBulwark";
        string attackSideCrown = "ActionRodArrowRightCrown";
        if ((thisHeroSide == "Diamond" && thisUserSide == "Player") || (thisHeroSide == "Square" && thisUserSide == "Enemy"))
        {
            rodNumber = 3;
            attackSideBulwark = "ActionRodArrowLeftBulwark";
            attackSideCrown = "ActionRodArrowLeftCrown";
        }

        float animationLength = 0f;
        if (enemyBulwark.GetBulwarkLevel() >= 3)
        {
            animationLength = playerRodAnimations.GetAnimationLength(rodNumber, attackSideBulwark);
        }
        else
        {
            animationLength = playerRodAnimations.GetAnimationLength(rodNumber, attackSideCrown);
        }

        return animationLength;
    }

    private float EngineerAction()
    {
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 0;
        int repairSide = 1;
        string attackSideBulwark = "ActionRodAttackRightBulwark";
        string attackSideCrown = "ActionRodAttackRightCrown";
        if ((thisHeroSide == "Diamond" && thisUserSide == "Player") || (thisHeroSide == "Square" && thisUserSide == "Enemy"))
        {
            rodNumber = 3;
            repairSide = 2;
            attackSideBulwark = "ActionRodAttackLeftBulwark";
            attackSideCrown = "ActionRodAttackLeftCrown";
        }

        //Erste Aktion
        float animationLength = 0f;
        if (enemyBulwark.GetBulwarkLevel() >= 1)
        {
            animationLength = playerRodAnimations.GetAnimationLength(rodNumber, attackSideBulwark);
        }
        else
        {
            animationLength = playerRodAnimations.GetAnimationLength(rodNumber, attackSideCrown);
        }

        //Zweite Aktion
        animationLength += playerRodAnimations.GetAnimationLength(repairSide, "ActionRodPopUp");

        return animationLength;
    }

    private float AssassinAction()
    {
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 1;
        if (thisHeroSide == "Diamond")
        {
            rodNumber = 2;
        }

        //Erste und zweite Aktion
        return enemyRodAnimations.GetAnimationLength(0, "ActionRodPopUp") + enemyRodAnimations.GetAnimationLength(rodNumber, "ActionRodPopUp");
    }

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
        float animationLength = playerRodAnimations.GetAnimationLength(crownSide, "ActionRodPopUp");

        //Zweite Aktion
        if (selfOtherHero.PriestChecksAction())
        {
            animationLength += playerRodAnimations.GetAnimationLength(rodNumber, "ActionRodPopUp");
        }

        return animationLength;
    }

    public float GetPriestSecondAnimationLength()
    {
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 3;
        if ((thisHeroSide == "Diamond" && thisUserSide == "Player") || (thisHeroSide == "Square" && thisUserSide == "Enemy"))
        {
            rodNumber = 0;
        }

        return playerRodAnimations.GetAnimationLength(rodNumber, "ActionRodPopUp");
    }

    public float GetBombAnimationLength()
    {
        //Auswerten, wo die Animation Stattfinden soll (Standard ist Square)
        int rodNumber = 1;
        if (thisHeroSide == "Diamond")
        {
            rodNumber = 2;
        }

        return playerRodAnimations.GetAnimationLength(rodNumber, "ActionRodPopUp");
    }
}
