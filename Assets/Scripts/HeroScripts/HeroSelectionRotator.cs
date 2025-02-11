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

    //Referenzen, um die Parents zu setzen
    [SerializeField] private GameObject playerHeroObject;

    //Welcher Held in der Liste ist ausgewählt
    private int currentPlayerSquareHeroIndex = 0;               //Aktuelle Position des Spieler Square Helden in der Liste
    private int currentPlayerDiamondHeroIndex = 1;              //Aktuelle Position des Spieler Diamond Helden in der Liste

    //Referenz auf den ausgewählten Helden
    [SerializeField] private Hero currentPlayerSquareHero;
    [SerializeField] private Hero currentPlayerDiamondHero;

    //Referenz auf das aktive Objekt des Helden
    [SerializeField] private GameObject activePlayerSquareHero;      //Referenz zum Objekt des Spieler-Square-Helden
    [SerializeField] private GameObject activePlayerDiamondHero;     //Referenz zum Objekt des Spieler-Diamond-Helden

    //Events
    public event Action<int, int, string> OnActivateChangeHero;
    public event Action<Hero> OnSquareHeroChanged;
    public event Action<Hero> OnDiamondHeroChanged;

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
        //Damit die Helden nicht überschrieben werden, da zu häufig gefeuert wird, nur aufrufen, wenn die Id vom Gameobject nicht mit der lokalen Id übereinstimmt
        if (this.NetworkObject.OwnerClientId != NetworkManager.Singleton.LocalClientId)
        {
            InitialHeroSetting.Instance.SetPlayerHeroes(currentPlayerSquareHero, currentPlayerDiamondHero);
        }
    }

    private void InitialHeroSetting_OnGetPlayerComponents(object sender, System.EventArgs e)
    {
        InitialHeroSetting.Instance.SetMultiplayerPlayerComponents(PlayerScript.LocalInstance);
    }

    //Methode zum Rotieren nach rechts
    public void RotateRight(string heroType)
    {
        UpdateHeroIndexRpc(heroType, true);

        UpdateHero(heroType);
    }

    //Methode zum Rotieren nach links
    public void RotateLeft(string heroType)
    {
        UpdateHeroIndexRpc(heroType, false);

        UpdateHero(heroType);
    }

    //Methode zum Aktualisieren der 3D-Helden in der Szene
    private void UpdateHero(string heroType)
    {
        UpdateHeroRpc(heroType);
    }




    //Rpcs
    [Rpc(SendTo.Everyone)]
    private void UpdateHeroIndexRpc(string heroType, bool rotateRight)
    {
        if (rotateRight)
        {
            if (heroType == "Square")
            {
                currentPlayerSquareHeroIndex = (currentPlayerSquareHeroIndex + 1) % availableHeroes.Length;

                //Verhindern, dass Square und Diamond gleich sind, indem ein index übersprungen wird
                if (currentPlayerSquareHeroIndex == currentPlayerDiamondHeroIndex)
                {
                    currentPlayerSquareHeroIndex = (currentPlayerSquareHeroIndex + 1) % availableHeroes.Length;
                }
            }
            else if (heroType == "Diamond")
            {
                currentPlayerDiamondHeroIndex = (currentPlayerDiamondHeroIndex + 1) % availableHeroes.Length;

                //Verhindern, dass Square und Diamond gleich sind, indem ein index übersprungen wird
                if (currentPlayerDiamondHeroIndex == currentPlayerSquareHeroIndex)
                {
                    currentPlayerDiamondHeroIndex = (currentPlayerDiamondHeroIndex + 1) % availableHeroes.Length;
                }
            }
            else
            {
                Debug.LogError("Der HeroType " + heroType + " existiert nicht!");
                return;
            }
        }
        else
        {
            if (heroType == "Square")
            {
                currentPlayerSquareHeroIndex = (currentPlayerSquareHeroIndex - 1 + availableHeroes.Length) % availableHeroes.Length;

                //Verhindern, dass Square und Diamond gleich sind, indem ein index übersprungen wird
                if (currentPlayerSquareHeroIndex == currentPlayerDiamondHeroIndex)
                {
                    currentPlayerSquareHeroIndex = (currentPlayerSquareHeroIndex - 1 + availableHeroes.Length) % availableHeroes.Length;
                }
            }
            else if (heroType == "Diamond")
            {
                currentPlayerDiamondHeroIndex = (currentPlayerDiamondHeroIndex - 1 + availableHeroes.Length) % availableHeroes.Length;

                //Verhindern, dass Square und Diamond gleich sind, indem ein index übersprungen wird
                if (currentPlayerDiamondHeroIndex == currentPlayerSquareHeroIndex)
                {
                    currentPlayerDiamondHeroIndex = (currentPlayerDiamondHeroIndex - 1 + availableHeroes.Length) % availableHeroes.Length;
                }
            }
            else
            {
                Debug.LogError("Der HeroType " + heroType + " existiert nicht!");
                return;
            }
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void UpdateHeroRpc(string heroType)
    {
        OnActivateChangeHero?.Invoke(currentPlayerSquareHeroIndex, currentPlayerDiamondHeroIndex, heroType);

        InstantiateNewHeroRpc(heroType);

        UpdateDisplaysRpc(heroType);
    }

    [Rpc(SendTo.Server)]
    private void InstantiateNewHeroRpc(string heroType)
    {
        //Nur ausführen, wenn es der Server ist
        if (!IsServer) { return; }

        //Zurzeit vorhandenen Helden in Zwischenvariable speichern, um ihn danach löschen zu können
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
            SetHeroToAllRpc(newHeroNetworkObject, heroType);
        }
        else
        {
            Debug.LogError("Das Objekt hat keine NetworkObject-Komponente und kann nicht gespawnt werden.");
        }
    }

    [Rpc(SendTo.Everyone)]
    private void SetHeroToAllRpc(NetworkObjectReference heroNetworkObjectReference, string heroType)
    {
        if (heroNetworkObjectReference.TryGet(out NetworkObject heroNetworkObject))
        {
            if (heroType == "Square")
            {
                activePlayerSquareHero = heroNetworkObject.gameObject;

                activePlayerSquareHero.GetComponent<Hero>().SetHeroParent(playerHeroObject, HeroSpawnDummy.PlayerSideKey.Player, HeroSpawnDummy.HeroSideKey.Square);

                currentPlayerSquareHero = activePlayerSquareHero.GetComponent<Hero>();
                OnSquareHeroChanged?.Invoke(currentPlayerSquareHero);
            }
            else
            {
                activePlayerDiamondHero = heroNetworkObject.gameObject;

                activePlayerDiamondHero.GetComponent<Hero>().SetHeroParent(playerHeroObject, HeroSpawnDummy.PlayerSideKey.Player, HeroSpawnDummy.HeroSideKey.Diamond);

                currentPlayerDiamondHero = activePlayerDiamondHero.GetComponent<Hero>();
                OnDiamondHeroChanged?.Invoke(currentPlayerDiamondHero);
            }
        }
        else
        {
            Debug.LogWarning($"Aus '{heroNetworkObjectReference}' konnte kein NetworkObject herausgezogen werden.");
        }
    }

    [Rpc(SendTo.Everyone)]
    private void UpdateDisplaysRpc(string heroType)
    {
        if (heroType == "Square")
        {
            if (currentPlayerSquareHero != null)
            {
                currentPlayerSquareHero.SetUIUpdater(playerSquareHeroUIUpdater);
                playerSquareHeroUIUpdater.UpdateHeroDisplay(currentPlayerSquareHero);
                currentPlayerSquareHero.SetEnergyBar(playerSquareEnergyBar);
                //playerSquareEnergyBar.UpdateEnergieDisplay(0, currentPlayerSquareHero.GetMaxEnergy());
                currentPlayerSquareHero.SetXPLightManager(playerSquareXPLightManager);
            }
            else
            {
                Debug.LogWarning($"Der Held '{currentPlayerSquareHero}' ist nicht vorhanden!");
            }
        }
        else
        {
            if (currentPlayerDiamondHero != null)
            {
                currentPlayerDiamondHero.SetUIUpdater(playerDiamondHeroUIUpdater);
                playerDiamondHeroUIUpdater.UpdateHeroDisplay(currentPlayerDiamondHero);
                currentPlayerDiamondHero.SetEnergyBar(playerDiamondEnergyBar);
                //playerDiamondEnergyBar.UpdateEnergieDisplay(0, currentPlayerDiamondHero.GetMaxEnergy());
                currentPlayerDiamondHero.SetXPLightManager(playerDiamondXPLightManager);
            }
            else
            {
                Debug.LogWarning($"Der Held '{currentPlayerDiamondHero}' ist nicht vorhanden!");
            }
        }
    }
}
