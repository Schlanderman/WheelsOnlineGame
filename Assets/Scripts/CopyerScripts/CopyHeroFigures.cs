using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class CopyHeroFigures : ManagerCopiesHandler<HeroSelectionRotator>
{
    [SerializeField] private GameObject[] availableHeroes;      //Liste der verfügbaren Helden

    //Referenzen, um die Parents zu setzen
    [SerializeField] private GameObject playerHeroObject;

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
            InstantiateNewHeroRpc(heroType);

            activePlayerSquareHero.GetComponent<Hero>().SetHeroParent(playerHeroObject, HeroSpawnDummy.PlayerSideKey.Enemy, HeroSpawnDummy.HeroSideKey.Square);
            activeSquareHeroAnimations = activePlayerSquareHero.GetComponentInChildren<HeroAnimationManager>();
        }

        else if (heroType == "Diamond")
        {
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
        GameObject newHeroObject = null;
        NetworkObject newHeroNetworkObject = null;
        if (heroType == "Square")
        {
            newHeroObject = Instantiate(availableHeroes[currentPlayerSquareHeroIndex]);
            newHeroNetworkObject = newHeroObject.GetComponent<NetworkObject>();
        }
        else
        {
            newHeroObject = Instantiate(availableHeroes[currentPlayerDiamondHeroIndex]);
            newHeroNetworkObject = newHeroObject.GetComponent<NetworkObject>();
        }

        //Spawn auf dem Server
        if (newHeroNetworkObject != null)
        {
            newHeroNetworkObject.Spawn();
            SetActiveHeroRpc(heroType, newHeroNetworkObject);
        }
        else
        {
            Debug.LogError("Das Objekt hat keine NetworkObject-Komponente und kann nicht gespawnt werden.");
        }
    }

    [Rpc(SendTo.Everyone)]
    private void SetActiveHeroRpc(string heroType, NetworkObjectReference heroNetworkObjectReference)
    {
        if (heroNetworkObjectReference.TryGet(out NetworkObject heroNetworkObject))
        {
            GameObject heroObject = heroNetworkObject.gameObject;
            if (heroType == "Square")
            {
                activePlayerSquareHero = heroObject;
            }
            else
            {
                activePlayerDiamondHero = heroObject;
            }
        }
    }
}
