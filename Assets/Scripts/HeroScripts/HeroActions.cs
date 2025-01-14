using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HeroActions : NetworkBehaviour
{
    [SerializeField] private EnemyScript enemyScript;  //Offline only

    [SerializeField] private GameObject playerObject;

    [SerializeField] private CrownManager playerCrown;      //Spielerkrone
    private CrownManager enemyCrown;       //Gegnerkrone

    [SerializeField] private BulwarkMover playerBulwark;    //Spielerbulwark
    private BulwarkMover enemyBulwark;     //Gegnerbulwark

    [SerializeField] private ActionRodAnimManager rodAnimations;        //Spieleranimationen
    private ActionRodAnimManager enemyRodAnimations;   //Gegneranimationen

    private Hero selfThisHero;          //Der Aktive Held
    private Hero selfOtherHero;         //Der andere Held auf Spielerseite
    private Hero enemySquareHero;       //Square Held auf Gegnerseite
    private Hero enemyDiamondHero;      //Diamond Held auf Gegnerseite

    private HeroAnimationManager heroAnimationManager;      //Der Animationsmanager des Helden

    private string thisHeroSide = "Square";
    private string thisUserSide = "Player";

    private bool priestBoostedOtherHero = false;     //Wert, ob ein Priester den anderen Helden Energie gegeben hat

    [SerializeField] private bool isSquareSide;


    private void Start()
    {
        InitialHeroSetting.Instance.OnSetEnemyManagers += InitialHeroSetting_OnSetEnemyManagers;
        InitialHeroSetting.Instance.OnSetMultiplayerHeroesInitially += InitialHeroSetting_OnSetMultiplayerHeroesInitially;
    }

    private void InitialHeroSetting_OnSetMultiplayerHeroesInitially(Hero playerOneSquare, Hero playerOneDiamond, Hero playerTwoSquare, Hero playerTwoDiamond)
    {
        SetMultiplayerHeroesRpc(
            playerOneSquare.GetComponent<NetworkObject>(),
            playerOneDiamond.GetComponent<NetworkObject>(),
            playerTwoSquare.GetComponent<NetworkObject>(),
            playerTwoDiamond.GetComponent<NetworkObject>()
            );
    }

    [Rpc(SendTo.Everyone)]
    private void SetMultiplayerHeroesRpc(NetworkObjectReference playerOneSquareNetworkObjectReference, NetworkObjectReference playerOneDiamondNetworkObjectReference,
        NetworkObjectReference playerTwoSquareNetworkObjectReference, NetworkObjectReference playerTwoDiamondNetworkObjectReference)
    {
        //Alle networkObjectReferences in NetworkObjects wandeln und dann die Heros holen
        if (!playerOneSquareNetworkObjectReference.TryGet(out NetworkObject playerOneSquareNetworkObject))
        {
            Debug.LogWarning($"'{playerOneSquareNetworkObjectReference}' hat kein NetworkObject.");
            return;
        }
        if (!playerOneDiamondNetworkObjectReference.TryGet(out NetworkObject playerOneDiamondNetworkObject))
        {
            Debug.LogWarning($"'{playerOneDiamondNetworkObjectReference}' hat kein NetworkObject.");
            return;
        }
        if (!playerTwoSquareNetworkObjectReference.TryGet(out NetworkObject playerTwoSquareNetworkObject))
        {
            Debug.LogWarning($"'{playerTwoSquareNetworkObjectReference}' hat kein NetworkObject.");
            return;
        }
        if (!playerTwoDiamondNetworkObjectReference.TryGet(out NetworkObject playerTwoDiamondNetworkObject))
        {
            Debug.LogWarning($"'{playerTwoDiamondNetworkObjectReference}' hat kein NetworkObject.");
            return;
        }

        Hero playerOneSquare = playerOneSquareNetworkObject.GetComponent<Hero>();
        Hero playerOneDiamond = playerOneDiamondNetworkObject.GetComponent<Hero>();
        Hero playerTwoSquare = playerTwoSquareNetworkObject.GetComponent<Hero>();
        Hero playerTwoDiamond = playerTwoDiamondNetworkObject.GetComponent<Hero>();

        //Zuweisung für die Helden
        bool isPlayerOneObject = playerObject.GetComponent<NetworkObject>().OwnerClientId == PlayerScript.LocalInstance.OwnerClientId;
        bool isHostPlayerOne = IsHost == isPlayerOneObject;

        //Bestimme die Helden für den Spieler und den Gegner basierend auf den Bedingungen
        var playerSquare = isHostPlayerOne ? playerOneSquare : playerTwoSquare;
        var playerDiamond = isHostPlayerOne ? playerOneDiamond : playerTwoDiamond;
        var enemySquare = isHostPlayerOne ? playerTwoSquare : playerOneSquare;
        var enemyDiamond = isHostPlayerOne ? playerTwoDiamond : playerOneDiamond;

        //Überprüfung, ob Square oder Diamond
        if (isSquareSide)
        {
            SetPlayerHeroes(playerSquare, playerDiamond);
            SetSquareSideMain();
            SetEnemyManagers();
        }
        else
        {
            SetPlayerHeroes(playerDiamond, playerSquare);
            SetDiamondSideMain();
        }

        //Setze die Helden für den Gegner
        SetEnemyHeroes(enemySquare, enemyDiamond);

        //Dieses Script dem jeweiligen Helden übergeben
        selfThisHero.SetHeroActions(this);
    }

    private void InitialHeroSetting_OnSetEnemyManagers(ulong playerId, CrownManager enemyCM, BulwarkMover enemyBM, ActionRodAnimManager enemyARM)
    {
        //Testen, ob die playerId mit der OwnerId übereinstimmt. Wenn ja, dann werden keine Managers gesetzt
        if (playerId == playerObject.GetComponent<NetworkObject>().OwnerClientId) {  return; }

        enemyCrown = enemyCM;
        enemyBulwark = enemyBM;
        enemyRodAnimations = enemyARM;
    }




    //Methode um Spielerhelden zuzuweisen
    private void SetPlayerHeroes(Hero thisHero, Hero otherHero)
    {
        selfThisHero = thisHero;
        selfOtherHero = otherHero;

        heroAnimationManager = thisHero.GetComponent<HeroAnimationManager>();
        //Debug.Log("AnimationManager in " + this + " ist " + heroAnimationManager);
    }

    //Methode um die Gegnerhelden zuzuweisen
    private void SetEnemyHeroes(Hero square, Hero diamond)
    {
        enemySquareHero = square;
        enemyDiamondHero = diamond;
    }

    //Methoden, um die Seiten zuzuweisen
    private void SetSquareSideMain()
    {
        thisHeroSide = "Square";
    }

    private void SetDiamondSideMain()
    {
        thisHeroSide = "Diamond";
    }

    private void SetEnemyManagers()
    {
        InitialHeroSetting.Instance.SetEnemyManagers(playerObject.GetComponent<NetworkObject>().OwnerClientId, playerCrown, playerBulwark, rodAnimations);
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
        heroAnimationManager.TriggerHeroAction();
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
            yield return StartCoroutine(rodAnimations.ActivateRodAnimation(rodNumber, "Sword", attackSideBulwark));

            //Schaden am Bulwark
            enemyBulwark.decreaseBulwark(selfThisHero.GetBulwarkDamage());
        }
        else
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            yield return StartCoroutine(rodAnimations.ActivateRodAnimation(rodNumber, "Sword", attackSideCrown));

            //Schaden an der Krone
            if (IsServer) { enemyCrown.DecreaseHPRpc(selfThisHero.GetCrownDamage()); }
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
            Debug.Log("Erster Angriff mit Bulwarklevel: " + enemyBulwark);
            heroAnimationManager.TriggerHeroAction();
            yield return StartCoroutine(rodAnimations.ActivateRodAnimation(rodNumber, "Fireball", attackSideBulwark));

            //Schaden am Bulwark
            enemyBulwark.decreaseBulwark(selfThisHero.GetBulwarkDamage());
        }
        else
        {
            //Animation
            Debug.Log("Erster Angriff ohne Bulwark");
            heroAnimationManager.TriggerHeroAction();
            yield return StartCoroutine(rodAnimations.ActivateRodAnimation(rodNumber, "Fireball", attackSideCrown));

            //Schaden an der Krone
            if (IsServer) { enemyCrown.DecreaseHPRpc(selfThisHero.GetCrownDamage()); }
        }

        //Zweiter Angriff
        //Animation
        Debug.Log("Zweiter Angriff!");
        heroAnimationManager.TriggerHeroAction();
        yield return StartCoroutine(rodAnimations.ActivateRodAnimation(rodNumber, "Fireball", attackSideCrownHigh));

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
            yield return StartCoroutine(rodAnimations.ActivateRodAnimation(rodNumber, "Arrow", attackSideBulwark));

            //Schaden am Bulwark
            enemyBulwark.decreaseBulwark(selfThisHero.GetBulwarkDamage());
        }
        else
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            yield return StartCoroutine(rodAnimations.ActivateRodAnimation(rodNumber, "Arrow", attackSideCrown));

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
            yield return StartCoroutine(rodAnimations.ActivateRodAnimation(rodNumber, "Hammer", attackSideBulwark));

            //Schaden am Bulwark
            enemyBulwark.decreaseBulwark(selfThisHero.GetBulwarkDamage());
        }
        else
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            yield return StartCoroutine(rodAnimations.ActivateRodAnimation(rodNumber, "Hammer", attackSideCrown));

            //Schaden an der Krone
            if (IsServer) { enemyCrown.DecreaseHPRpc(selfThisHero.GetCrownDamage()); }
        }

        //Zweite Aktion
        //Animation
        heroAnimationManager.TriggerHeroAction();
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
        if (enemySquareHero.GetCurrentEnergy() > enemyDiamondHero.GetCurrentEnergy())
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            yield return StartCoroutine(enemyRodAnimations.ActivateRodAnimation(0, "Dagger", "PopUp"));

            //Square Held Energie abziehen
            enemySquareHero.DecreaseEnergy(selfThisHero.GetDelayAdding());
            enemySquareHero.SetCanMakeAction(false);
        }
        else
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            yield return StartCoroutine(enemyRodAnimations.ActivateRodAnimation(3, "Dagger", "PopUp"));

            //Diamond Held Energie abziehen
            enemyDiamondHero.DecreaseEnergy(selfThisHero.GetDelayAdding());
            enemyDiamondHero.SetCanMakeAction(false);
        }

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
        yield return StartCoroutine(rodAnimations.ActivateRodAnimation(crownSide, "Book", "PopUp"));

        //Spieler-Krone heilen
        if (IsServer) { playerCrown.IncreaseHPRpc(selfThisHero.GetHealingAdding()); }

        //Zweite Aktion
        if (selfOtherHero.PriestChecksAction())
        {
            //Animation
            heroAnimationManager.TriggerHeroAction();
            yield return StartCoroutine(rodAnimations.ActivateRodAnimation(rodNumber, "Book", "PopUp"));

            //Anderen Helden Energie geben
            selfOtherHero.AddEnergy(selfThisHero.GetEnergyAdding());
            priestBoostedOtherHero = true;
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
        yield return StartCoroutine(rodAnimations.ActivateRodAnimation(rodNumber, "Book", "PopUp"));

        //Anderen Helden Energie geben
        selfOtherHero.AddEnergy(selfThisHero.GetEnergyAdding());
    }

    public bool GetPriestBoosted()
    {
        bool temp = priestBoostedOtherHero;
        priestBoostedOtherHero = false;
        return temp;
    }
}
