using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerGameManager : NetworkBehaviour
{
    //Singleton
    public static MultiplayerGameManager Instance { get; private set; }

    //Werte, für den lokalen Spieler
    private bool isLocalPlayerReadyToPlay = false;
    private bool isLocalPlayerRoundFinished = false;
    private int localPlayerCurrentHP = 10;

    //Dictionaries für alle Spieler
    private Dictionary<ulong, bool> playerReadyToPlayDictionary = new Dictionary<ulong, bool>();

    //Copyer Prefab
    [SerializeField] private GameObject actionsCopyer;

    //Boardtransforms
    [SerializeField] private Transform gameBoardPlayerOne;
    [SerializeField] private Transform gameBoardPlayerTwo;

    [SerializeField] private GameObject heroSpawnPosition;

    //Events
    public event EventHandler OnPlayersAreReadyToPlay;
    public event EventHandler OnLocalPlayerRoundFinishedChanged;
    public event EventHandler OnLocalPlayerCurrentHPChanged;


    private void Awake()
    {
        Instance = this;

        playerReadyToPlayDictionary = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        if (PlayerScript.LocalInstance != null)
        {
            ClientConnectedToServer();
        }
        else
        {
            PlayerScript.OnAnyPlayerSpawned += PlayerScript_OnAnyPlayerSpawned;
        }
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
            ClientConnectedToServer();
        }
    }


    //Getter / Setter für die lokalen Spielerwerte
    public void SetLocalPlayerReadyToPlay(bool state)
    {
        isLocalPlayerReadyToPlay = state;

        SetPlayerReadyToPlayServerRPC();
    }

    public void SetLocalPlayerRoundFinished(bool state)
    {
        isLocalPlayerRoundFinished = state;
        OnLocalPlayerRoundFinishedChanged?.Invoke(this, EventArgs.Empty);
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
        //Nur ausführen, wenn 2 Spieler verbunden sind
        if (NetworkManager.Singleton.ConnectedClients.Count != 2) { return; }

        ActivatePlayerUIElementsRpc();
        SpawnCopyersRpc();
    }



    //RPCs für die Funktionen
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyToPlayServerRPC(ServerRpcParams serverRpcParams = default)
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

    [Rpc(SendTo.Server)]
    private void SpawnCopyersRpc()
    {
        if (!IsServer) { return; }

        GameObject newPlayerOneCopyer = Instantiate(actionsCopyer);
        GameObject newPlayerTwoCopyer = Instantiate(actionsCopyer);

        NetworkObject copyOfPlayerOneNetwork = newPlayerOneCopyer.GetComponent<NetworkObject>();
        NetworkObject copyOfPlayerTwoNetwork = newPlayerTwoCopyer.GetComponent<NetworkObject>();

        copyOfPlayerOneNetwork.transform.position = gameBoardPlayerOne.transform.position;
        copyOfPlayerTwoNetwork.transform.position = gameBoardPlayerTwo.transform.position;

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
        PlayerScript.LocalInstance.ChangePlayerUIElements(false);
    }
}
