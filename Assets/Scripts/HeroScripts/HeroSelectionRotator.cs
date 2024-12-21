using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelectionRotator : NetworkBehaviour
{
    [SerializeField] private EnemyScript enemyScript;  //Offline only

    [SerializeField] private GameObject[] availableHeroes;      //Liste der verfügbaren Helden

    //Referenzen auf die UI-Updater
    [SerializeField] private HeroUIUpdater playerSquareHeroUIUpdater;
    [SerializeField] private HeroUIUpdater playerDiamondHeroUIUpdater;

    //Referenzen auf die Energy Bars
    [SerializeField] private EnergyBar playerSquareEnergyBar;
    [SerializeField] private EnergyBar playerDiamondEnergyBar;

    //Referenzen auf die XP Lichter
    [SerializeField] private XPLightManager playerSquareXPLightManager;
    [SerializeField] private XPLightManager playerDiamondXPLightManager;

    //Referenzen auf die Spawnpositionen der Helden
    //[SerializeField] private Transform playerSquareHeroSpawnPoint;      //Position, an der der Spieler-Square-Held erscheinen soll
    //[SerializeField] private Transform playerDiamondHeroSpawnPoint;     //Position, an der der Spieler-Diamond-Held erscheinen soll

    //Referenzen, um die Parents zu setzen
    [SerializeField] private GameObject playerHeroObject;
    private FixedString128Bytes pathToSquareHeroSpawn = "FigureCurbSelfSquareSpawn/FigureCurbSelfSquare/FigureStandSquare/SquareHeroSpawn";
    private FixedString128Bytes pathToDiamondHeroSpawn = "FigureCurbSelfDiamondSpawn/FigureCurbSelfDiamond/FigureStandDiamond/DiamondHeroSpawn";

    //Welcher Held in der Liste ist ausgewählt
    private int currentPlayerSquareHeroIndex = 0;               //Aktuelle Position des Spieler Square Helden in der Liste
    private int currentPlayerDiamondHeroIndex = 0;              //Aktuelle Position des Spieler Diamond Helden in der Liste

    //Referenz auf den ausgewählten Helden
    private Hero currentPlayerSquareHero;
    private Hero currentPlayerDiamondHero;

    //Referenz auf das aktive Objekt des Helden
    [SerializeField] private GameObject activePlayerSquareHero;      //Referenz zum Objekt des Spieler-Square-Helden
    [SerializeField] private GameObject activePlayerDiamondHero;     //Referenz zum Objekt des Spieler-Diamond-Helden

    //Events
    public event Action<int, int, string> OnActivateChangeHero;

    //Initialisierung
    private void Start()
    {
        UpdateHero("Square");
        UpdateHero("Diamond");

        InitialHeroSetting.Instance.OnGetHeroes += InitialHeroSetting_OnGetHeroes;
        InitialHeroSetting.Instance.OnGetPlayerComponents += InitialHeroSetting_OnGetPlayerComponents;
    }

    private void InitialHeroSetting_OnGetHeroes(object sender, System.EventArgs e)
    {
        if (enemyScript != null)
        {
            InitialHeroSetting.Instance.SetPlayerHeroes(enemyScript.playerId, currentPlayerSquareHero, currentPlayerDiamondHero);
        }
        else
        {
            //InitialHeroSetting.Instance.SetPlayerHeroes(PlayerScript.Instance.playerId, currentPlayerSquareHero, currentPlayerDiamondHero);
        }
    }

    private void InitialHeroSetting_OnGetPlayerComponents(object sender, System.EventArgs e)
    {
        if (enemyScript != null)
        {
            InitialHeroSetting.Instance.SetPlayerComponents(
                enemyScript.playerId,
                enemyScript.GetSquareHeroActions(),
                enemyScript.GetDiamondHeroActions(),
                enemyScript.GetEvaluationManager()
                );
        }
        else
        {
            //InitialHeroSetting.Instance.SetPlayerComponents(
            //    PlayerScript.Instance.playerId,
            //    PlayerScript.Instance.GetSquareHeroActions(),
            //    PlayerScript.Instance.GetDiamondHeroActions(),
            //    PlayerScript.Instance.GetEvaluationManager()
            //    );
        }
    }

    //Methode zum Rotieren nach rechts
    public void RotateRight(string heroType)
    {
        if (heroType == "Square")
        {
            currentPlayerSquareHeroIndex = (currentPlayerSquareHeroIndex + 1) % availableHeroes.Length;
        }
        else if (heroType == "Diamond")
        {
            currentPlayerDiamondHeroIndex = (currentPlayerDiamondHeroIndex + 1) % availableHeroes.Length;
        }
        else
        {
            Debug.LogError("Der HeroType " + heroType + " existiert nicht!");
            return;
        }

        UpdateHero(heroType);
    }

    //Methode zum Rotieren nach links
    public void RotateLeft(string heroType)
    {
        if (heroType == "Square")
        {
            currentPlayerSquareHeroIndex = (currentPlayerSquareHeroIndex - 1 + availableHeroes.Length) % availableHeroes.Length;
        }
        else if (heroType == "Diamond")
        {
            currentPlayerDiamondHeroIndex = (currentPlayerDiamondHeroIndex - 1 + availableHeroes.Length) % availableHeroes.Length;
        }
        else
        {
            Debug.LogError("Der HeroType " + heroType + " existiert nicht!");
            return;
        }

        UpdateHero(heroType);
    }

    //Methode zum Aktualisieren der 3D-Helden in der Szene
    private void UpdateHero(string heroType)
    {
        OnActivateChangeHero?.Invoke(currentPlayerSquareHeroIndex, currentPlayerDiamondHeroIndex, heroType);

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

            activePlayerSquareHero.GetComponent<Hero>().SetHeroParent(playerHeroObject, HeroSpawnDummy.PlayerSideKey.Player, HeroSpawnDummy.HeroSideKey.Square);

            currentPlayerSquareHero = activePlayerSquareHero.GetComponent<Hero>();

            if (currentPlayerSquareHero != null)
            {
                currentPlayerSquareHero.SetUIUpdater(playerSquareHeroUIUpdater);
                playerSquareHeroUIUpdater.UpdateHeroDisplay(currentPlayerSquareHero);
                currentPlayerSquareHero.SetEnergyBar(playerSquareEnergyBar);
                playerSquareEnergyBar.UpdateEnergieDisplay(0, currentPlayerSquareHero.GetMaxEnergy());
                currentPlayerSquareHero.SetXPLightManager(playerSquareXPLightManager);
            }
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

            activePlayerDiamondHero.GetComponent<Hero>().SetHeroParent(playerHeroObject, HeroSpawnDummy.PlayerSideKey.Player, HeroSpawnDummy.HeroSideKey.Diamond);

            currentPlayerDiamondHero = activePlayerDiamondHero.GetComponent<Hero>();

            if (currentPlayerDiamondHero != null)
            {
                currentPlayerDiamondHero.SetUIUpdater(playerDiamondHeroUIUpdater);
                playerDiamondHeroUIUpdater.UpdateHeroDisplay(currentPlayerDiamondHero);
                currentPlayerDiamondHero.SetEnergyBar(playerDiamondEnergyBar);
                playerDiamondEnergyBar.UpdateEnergieDisplay(0, currentPlayerDiamondHero.GetMaxEnergy());
                currentPlayerDiamondHero.SetXPLightManager(playerDiamondXPLightManager);
            }
        }
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
            SetHeroToAllClientRpc(newHeroNetworkObject, heroType);
        }
        else
        {
            Debug.LogError("Das Objekt hat keine NetworkObject-Komponente und kann nicht gespawnt werden.");
        }
    }

    [ClientRpc]
    private void SetHeroToAllClientRpc(NetworkObjectReference heroNetworkObjectReference, string heroType)
    {
        heroNetworkObjectReference.TryGet(out NetworkObject heroNetworkObject);

        if (heroType == "Square")
        {
            activePlayerSquareHero = heroNetworkObject.gameObject;
        }
        else
        {
            activePlayerDiamondHero = heroNetworkObject.gameObject;
        }
    }
}
