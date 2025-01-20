using System.Collections;
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

    //CopyManager um zugriff auf die OriginalHelden zu haben
    private CopyManager copyManager;

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
            SetAnimationManagersRpc(heroType, activePlayerSquareHero.GetComponent<NetworkObject>());
        }

        else if (heroType == "Diamond")
        {
            InstantiateNewHeroRpc(heroType);

            activePlayerDiamondHero.GetComponent<Hero>().SetHeroParent(playerHeroObject, HeroSpawnDummy.PlayerSideKey.Enemy, HeroSpawnDummy.HeroSideKey.Diamond);
            SetAnimationManagersRpc(heroType, activePlayerDiamondHero.GetComponent<NetworkObject>());
        }
    }

    [Rpc(SendTo.Everyone)]
    private void SetAnimationManagersRpc(string heroType, NetworkObjectReference heroObjectReference)
    {
        if (!heroObjectReference.TryGet(out NetworkObject heroNetworkObject))
        {
            Debug.LogError($"{heroObjectReference} hat keine NetworkObject Komponente!");
        }

        GameObject heroObject = heroNetworkObject.gameObject;

        StartCoroutine(SetAnimationManager(heroType, heroObject));
    }

    //HeroAnimations übergeben, sobald sie vorhanden sind
    private IEnumerator SetAnimationManager(string heroType, GameObject hero)
    {
        while (hero.GetComponentInChildren<HeroAnimationManager>() == null)
        {
            yield return null;
        }

        if (heroType == "Square")
        {
            activeSquareHeroAnimations = hero.GetComponentInChildren<HeroAnimationManager>();
        }
        else
        {
            activeDiamondHeroAnimations = hero.GetComponentInChildren<HeroAnimationManager>();
        }
    }

    public void SetActionRodManager(ActionRodAnimManager actionRodManager)
    {
        actionRodAnimManager = actionRodManager;
        actionRodAnimManager.OnActivateActionRodAnimation += ActionRodAnimManager_OnActivateActionRodAnimation;
    }

    public void SetCopyManager(CopyManager copy)
    {
        copyManager = copy;

        //Events, die damit einhergehen aktivieren
        copyManager.GetHeroSelectionRotator().OnSquareHeroChanged += CopyHeroFigures_OnSquareHeroChanged;
        copyManager.GetHeroSelectionRotator().OnDiamondHeroChanged += CopyHeroFigures_OnDiamondHeroChanged;
    }

    private void CopyHeroFigures_OnSquareHeroChanged(Hero hero)
    {
        hero.SetCopyHero(activePlayerSquareHero.GetComponent<Hero>());
    }

    private void CopyHeroFigures_OnDiamondHeroChanged(Hero hero)
    {
        hero.SetCopyHero(activePlayerDiamondHero.GetComponent<Hero>());
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
