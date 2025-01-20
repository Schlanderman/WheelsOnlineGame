using System.Collections;
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
        selfThisHero.SetHeroActions(gameObject.GetComponent<HeroActionsActivator>(), gameObject.GetComponent<HeroActionsLengthManager>());
    }

    private void InitialHeroSetting_OnSetEnemyManagers(ulong playerId, CrownManager enemyCM, BulwarkMover enemyBM, ActionRodAnimManager enemyARM)
    {
        //Testen, ob die playerId mit der OwnerId übereinstimmt. Wenn ja, dann werden keine Managers gesetzt
        if (playerId == playerObject.GetComponent<NetworkObject>().OwnerClientId) { return; }

        enemyCrown = enemyCM;
        enemyBulwark = enemyBM;
        enemyRodAnimations = enemyARM;

        //Zu diesem Zeitpunkt sollten alle Manager vorhanden sein
        StartCoroutine(SetManagersToActivator());
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

    private IEnumerator SetManagersToActivator()
    {
        //Warten, bis alle Manager vorhanden sind
        while (
            playerCrown == null || enemyCrown == null ||
            playerBulwark == null || enemyBulwark == null ||
            rodAnimations == null || enemyRodAnimations == null ||
            selfThisHero == null || selfOtherHero == null ||
            enemySquareHero == null || enemyDiamondHero == null ||
            heroAnimationManager == null
            )
        { yield return null; }

        //Dem Activator alle Manager übertragen
        gameObject.GetComponent<HeroActionsActivator>().SetManagers(
            playerCrown, enemyCrown,
            playerBulwark, enemyBulwark,
            rodAnimations, enemyRodAnimations,
            selfThisHero, selfOtherHero, enemySquareHero, enemyDiamondHero,
            heroAnimationManager,
            thisHeroSide, thisUserSide
            );

        //Dem LengthManager alle Manager übertragen
        gameObject.GetComponent<HeroActionsLengthManager>().SetManagers(
            selfOtherHero,
            enemyBulwark,
            rodAnimations, enemyRodAnimations,
            thisHeroSide, thisUserSide
            );
    }
}
