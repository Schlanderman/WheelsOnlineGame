using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class CopyHeroFigures : ManagerCopiesHandler<HeroSelectionRotator>
{
    [SerializeField] private GameObject[] availableHeroes;      //Liste der verfügbaren Helden

    //Referenzen auf die Spawnpositionen der Helden
    //[SerializeField] private Transform playerSquareHeroSpawnPoint;
    //[SerializeField] private Transform playerDiamondHeroSpawnPoint;

    //Referenzen, um die Parents zu setzen
    [SerializeField] private GameObject playerHeroObject;
    private FixedString128Bytes pathToSquareHeroSpawn = "FigureCurbCopySquareSpawn/FigureCurbCopySquare/FigureStandSquare/SquareHeroSpawn";
    private FixedString128Bytes pathToDiamondHeroSpawn = "FigureCurbCopyDiamondSpawn/FigureCurbCopyDiamond/FigureStandDiamond/DiamondHeroSpawn";

    //Referenz auf das aktive Objekt des Helden
    private GameObject activePlayerSquareHero;
    private GameObject activePlayerDiamondHero;

    //Referenz auf HeroAnimations
    private HeroAnimationManager activeSquareHeroAnimations;
    private HeroAnimationManager activeDiamondHeroAnimations;

    private ActionRodAnimManager actionRodAnimManager;

    //Welcher Held in der Liste ist ausgewählt
    private int currentPlayerSquareHeroIndex = 0;
    private int currentPlayerDiamondHeroIndex = 0;

    protected override void SetEvents()
    {
        originalManager.OnActivateChangeHero += HeroSelectionRotator_OnActivateChangeHero;
    }

    private void HeroSelectionRotator_OnActivateChangeHero(int squareHeroIndex, int diamondHeroIndex, string heroType)
    {
        currentPlayerSquareHeroIndex = squareHeroIndex;
        currentPlayerDiamondHeroIndex = diamondHeroIndex;

        UpdateHero(heroType);
    }



    //Methode zum Aktualisieren der 3D-Helden in der Szene
    private void UpdateHero(string heroType)
    {
        if (heroType == "Square")
        {
            ////Lösche den aktuellen Spieler-Square-Held, wenn einer existiert
            //if (activePlayerSquareHero != null)
            //{
            //    Destroy(activePlayerSquareHero);
            //}

            ////Instanziere den neuen Spieler-Square-Held an der vorgesehenen Position
            //activePlayerSquareHero = Instantiate(availableHeroes[currentPlayerSquareHeroIndex], playerSquareHeroSpawnPoint.position, playerSquareHeroSpawnPoint.rotation);

            InstantiateNewHeroRpc(heroType);

            activePlayerSquareHero.GetComponent<Hero>().SetHeroParent(playerHeroObject, HeroSpawnDummy.PlayerSideKey.Enemy, HeroSpawnDummy.HeroSideKey.Square);
            activeSquareHeroAnimations = activePlayerSquareHero.GetComponentInChildren<HeroAnimationManager>();
        }

        else if (heroType == "Diamond")
        {
            ////Lösche den aktuellen Spieler-Diamond-Held, wenn einer existiert
            //if (activePlayerDiamondHero != null)
            //{
            //    Destroy(activePlayerDiamondHero);
            //}

            ////Instanziere den neuen Spieler-Diamond-Held an der vorgesehenen Position
            //activePlayerDiamondHero = Instantiate(availableHeroes[currentPlayerDiamondHeroIndex], playerDiamondHeroSpawnPoint.position, playerDiamondHeroSpawnPoint.rotation);

            InstantiateNewHeroRpc(heroType);

            activePlayerDiamondHero.GetComponent<Hero>().SetHeroParent(playerHeroObject, HeroSpawnDummy.PlayerSideKey.Enemy, HeroSpawnDummy.HeroSideKey.Diamond);
            activeDiamondHeroAnimations = activePlayerDiamondHero.GetComponentInChildren<HeroAnimationManager>();
        }
    }

    public void SetActionRodManager(ActionRodAnimManager actionRodManager)
    {
        actionRodAnimManager = actionRodManager;
        actionRodAnimManager.OnActivateActionRodAnimation += ActionRodAnimManager_OnActivateActionRodAnimation;
    }

    private void ActionRodAnimManager_OnActivateActionRodAnimation(int rodIndex, string sprite, string animation)
    {
        string heroSide = ChangeAnimationString(rodIndex, animation);

        if (heroSide == "Square")
        {
            activeSquareHeroAnimations.TriggerHeroAction();
        }
        else
        {
            activeDiamondHeroAnimations.TriggerHeroAction();
        }
    }

    private string ChangeAnimationString(int rodIndex, string animation)
    {
        if (animation == "AttackRightBulwark" ||
            animation == "AttackRightCrown" ||
            animation == "FireballRightHigh" ||
            animation == "ArrowRightBulwark" ||
            animation == "ArrowRightCrown") { return "Square"; }
        else if (animation == "AttackLeftBulwark"||
            animation == "AttackLeftCrown" ||
            animation == "FireballLeftHigh" ||
            animation == "ArrowLeftBulwark" ||
            animation == "ArrowLeftCrown") { return "Diamond"; }
        else if (animation == "PopUp" && rodIndex <= 1) { return "Square"; }
        else if (animation == "PopUp" && rodIndex >= 2) { return "Diamond"; }

        return "Diamond";
    }


    //Rpcs
    [Rpc(SendTo.Server)]
    public void InstantiateNewHeroRpc(string heroType)
    {
        //Nur ausführen, wenn es der Server ist
        if (!IsServer) { return; }

        NetworkObject currentlyActiveHero = null;
        if (heroType == "Square")
        {
            if (activePlayerSquareHero != null)
            {
                currentlyActiveHero = activePlayerSquareHero.GetComponent<NetworkObject>();
            }
        }
        else
        {
            if (activePlayerDiamondHero != null)
            {
                currentlyActiveHero = activePlayerDiamondHero.GetComponent<NetworkObject>();
            }
        }

        //Altes Objekt zerstören
        if (currentlyActiveHero != null && currentlyActiveHero.IsSpawned)
        {
            currentlyActiveHero.Despawn(true);
        }

        //Instanziieren und Networkobjekt abrufen
        NetworkObject newHeroNetworkObject = null;
        if (heroType == "Square")
        {
            activePlayerSquareHero = Instantiate(availableHeroes[currentPlayerSquareHeroIndex]);
            newHeroNetworkObject = activePlayerSquareHero.GetComponent<NetworkObject>();

        }
        else
        {
            activePlayerDiamondHero = Instantiate(availableHeroes[currentPlayerDiamondHeroIndex]);
            newHeroNetworkObject = activePlayerDiamondHero.GetComponent<NetworkObject>();
        }

        //Spawn auf dem Server
        if (newHeroNetworkObject != null)
        {
            newHeroNetworkObject.Spawn();
        }
        else
        {
            Debug.LogError("Das Objekt hat keine NetworkObject-Komponente und kann nicht gespawnt werden.");
        }
    }
}
