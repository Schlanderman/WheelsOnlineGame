using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerGameManager : NetworkBehaviour
{
    //Singleton
    public static MultiplayerGameManager Instance { get; private set; }

    //Werte, f�r den lokalen Spieler
    private bool isLocalPlayerReadyToPlay = false;
    private bool isLocalPlayerRoundFinished = false;
    private int localPlayerCurrentHP = 10;

    //Dictionaries f�r alle Spieler
    private Dictionary<ulong, bool> playerReadyToPlayDictionary = new Dictionary<ulong, bool>();
    private Dictionary<ulong, bool> playerRoundFinishedDictionary = new Dictionary<ulong, bool>();

    //Copyer Prefab
    [SerializeField] private GameObject actionsCopyer;

    //Boardtransforms
    [SerializeField] private Transform gameBoardPlayerOne;
    [SerializeField] private Transform gameBoardPlayerTwo;

    [SerializeField] private GameObject heroSpawnPosition;

    [SerializeField] private Transform playerPrefab;

    //Events
    public event EventHandler OnPlayersAreReadyToPlay;
    public event EventHandler OnPlayersRoundIsFinished;
    public event EventHandler OnLocalPlayerCurrentHPChanged;


    private void Awake()
    {
        Instance = this;

        playerReadyToPlayDictionary = new Dictionary<ulong, bool>();
        playerRoundFinishedDictionary = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        if (PlayerScript.LocalInstance != null)
        {
            //ClientConnectedToServer();
        }
        else
        {
            PlayerScript.OnAnyPlayerSpawned += PlayerScript_OnAnyPlayerSpawned;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }

        ClientConnectedToServer();
    }

    private void PlayerScript_OnAnyPlayerSpawned(object sender, EventArgs e)
    {
        if (sender is PlayerScript playerScript)
        {
            if (playerScript != PlayerScript.LocalInstance)
            {
                return;
            }
        }


        if (PlayerScript.LocalInstance != null)
        {
            Debug.Log($"Client Connected to Server von {sender} aufgerufen");
            //ClientConnectedToServer();
        }
    }


    //Getter / Setter f�r die lokalen Spielerwerte
    public void SetLocalPlayerReadyToPlay(bool state)
    {
        isLocalPlayerReadyToPlay = state;

        SetPlayerReadyToPlayServerRpc();
    }

    public void SetLocalPlayerRoundFinished(bool state)
    {
        isLocalPlayerRoundFinished = state;

        SetPlayerRoundFinishedServerRpc();
    }

    public void ChangeLocalPlayerCurrentHP(int newHP)
    {
        localPlayerCurrentHP = newHP;
        OnLocalPlayerCurrentHPChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool GetIsLocalPlayerReadyToPlay()
    {
        return isLocalPlayerReadyToPlay;
    }

    public bool GetIsLocalPlayerRoundFinished()
    {
        return isLocalPlayerRoundFinished;
    }

    public int GetLocalPlayerCurrentHP()
    {
        return localPlayerCurrentHP;
    }



    private void ClientConnectedToServer()
    {
        //Nur ausf�hren, wenn 2 Spieler verbunden sind
        if (NetworkManager.Singleton.ConnectedClients.Count != 2) { return; }

        ActivatePlayerUIElementsRpc();
        SpawnCopyersRpc();
    }



    //RPCs f�r die Funktionen
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyToPlayServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyToPlayDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyToPlayDictionary.ContainsKey(clientId) || !playerReadyToPlayDictionary[clientId])
            {
                //This Player is not Ready!
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            OnPlayersAreReadyToPlay?.Invoke(this, EventArgs.Empty);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerRoundFinishedServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerRoundFinishedDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerRoundFinishedDictionary.ContainsKey(clientId) || !playerRoundFinishedDictionary[clientId])
            {
                //This Player is not Ready!
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            OnPlayersRoundIsFinished?.Invoke(this, EventArgs.Empty);
            playerRoundFinishedDictionary.Clear();
        }
    }

    [Rpc(SendTo.Server)]
    private void SpawnCopyersRpc()
    {
        if (!IsServer) { return; }

        GameObject newPlayerOneCopyer = Instantiate(actionsCopyer);
        GameObject newPlayerTwoCopyer = Instantiate(actionsCopyer);

        NetworkObject copyOfPlayerOneNetwork = newPlayerOneCopyer.GetComponent<NetworkObject>();
        NetworkObject copyOfPlayerTwoNetwork = newPlayerTwoCopyer.GetComponent<NetworkObject>();

        copyOfPlayerOneNetwork.transform.position = gameBoardPlayerTwo.transform.position;
        copyOfPlayerTwoNetwork.transform.position = gameBoardPlayerOne.transform.position;

        copyOfPlayerOneNetwork.Spawn(true);
        copyOfPlayerTwoNetwork.Spawn(true);

        SpawnParentsForHeroesRpc(copyOfPlayerOneNetwork);
        SpawnParentsForHeroesRpc(copyOfPlayerTwoNetwork);

        var players = FindObjectsByType<PlayerScript>(FindObjectsSortMode.None);

        foreach (var player in players)
        {
            if (player.OwnerClientId == NetworkManager.ServerClientId)
            {
                copyOfPlayerOneNetwork.GetComponent<CopyManager>().SetManagersFromOriginal(player.gameObject);
            }
            else
            {
                copyOfPlayerTwoNetwork.GetComponent<CopyManager>().SetManagersFromOriginal(player.gameObject);
            }
        }
    }

    [Rpc(SendTo.Server)]
    private void SpawnParentsForHeroesRpc(NetworkObjectReference copyObjectReference)
    {
        if (!IsServer) { return; }

        if (copyObjectReference.TryGet(out NetworkObject copyNetworkObject))
        {
            GameObject heroSpawnPositionSquare = Instantiate(heroSpawnPosition);
            GameObject heroSpawnPositionDiamond = Instantiate(heroSpawnPosition);

            heroSpawnPositionSquare.GetComponent<HeroSpawnDummy>().SetPositionForHeroSpawn(
                HeroSpawnDummy.PlayerSideKey.Enemy,
                HeroSpawnDummy.HeroSideKey.Square,
                copyNetworkObject.gameObject
                );

            heroSpawnPositionDiamond.GetComponent<HeroSpawnDummy>().SetPositionForHeroSpawn(
                HeroSpawnDummy.PlayerSideKey.Enemy,
                HeroSpawnDummy.HeroSideKey.Diamond,
                copyNetworkObject.gameObject
                );

            heroSpawnPositionSquare.GetComponent<NetworkObject>().Spawn(true);
            heroSpawnPositionDiamond.GetComponent<NetworkObject>().Spawn(true);

            heroSpawnPositionSquare.GetComponent<HeroSpawnDummy>().SetParentForHeroSpawn(copyNetworkObject.gameObject);
            heroSpawnPositionDiamond.GetComponent<HeroSpawnDummy>().SetParentForHeroSpawn(copyNetworkObject.gameObject);
        }
    }

    [Rpc(SendTo.Everyone)]
    private void ActivatePlayerUIElementsRpc()
    {
        PlayerScript.LocalInstance.ChangePlayerUIElements(true);
    }
}
