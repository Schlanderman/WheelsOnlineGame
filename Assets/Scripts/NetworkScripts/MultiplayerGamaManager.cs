using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerGamaManager : MonoBehaviour
{
    public static MultiplayerGamaManager Instance;

    //Copyer Prefab
    [SerializeField] private GameObject actionsCopyer;

    [SerializeField] private Transform gameBoardPlayerOne;
    [SerializeField] private Transform gameBoardPlayerTwo;

    private List<ulong> connectedPlayerIds = new List<ulong>();
    private Dictionary<ulong, PlayerScript> connectedPlayerDictionary = new Dictionary<ulong, PlayerScript>();

    //Events
    public event Action<ulong> OnGetPlayerScriptWithId;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //Aboniere die Callbacks für Verbindungen
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
    }

    //private void OnDisable()
    //{
    //    //Entferne die Callbacks bei Deaktivierung
    //    NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
    //    NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
    //}

    private void OnClientConnectedCallback(ulong clientId)
    {
        if (!connectedPlayerIds.Contains(clientId))
        {
            connectedPlayerIds.Add(clientId);
            OnGetPlayerScriptWithId?.Invoke(clientId);
        }
        CheckAllPlayersConnected();
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        if (connectedPlayerIds.Contains(clientId))
        {
            connectedPlayerIds.Remove(clientId);
        }
    }

    private void CheckAllPlayersConnected()
    {
        List<ulong> currentlyConnectedClients = new List<ulong>();
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            currentlyConnectedClients.Add(client.ClientId);
        }

        if (currentlyConnectedClients.Count == 2)
        {
            TurnManager.Instance.InitializeReadynessOnline(currentlyConnectedClients);
            TurnManager.Instance.InitializeCrownHPOnline(currentlyConnectedClients);
            InitialHeroSetting.Instance.InitializeReadynessOnline(currentlyConnectedClients);

            //CopyObjekte spawnen
            GameObject newCopyOfPlayerOne = Instantiate(actionsCopyer, gameBoardPlayerTwo, false);
            GameObject newCopyOfPlayerTwo = Instantiate(actionsCopyer, gameBoardPlayerOne, false);

            //newCopyOfPlayerOne.GetComponent<CopyManager>().SetManagersFromOriginal(connectedPlayerDictionary[connectedPlayerIds[0]].gameObject);
            //newCopyOfPlayerTwo.GetComponent<CopyManager>().SetManagersFromOriginal(connectedPlayerDictionary[connectedPlayerIds[1]].gameObject);
        }
    }

    public void SetPlayerScriptFromClientId(ulong playerId, PlayerScript player)
    {
        connectedPlayerDictionary.Add(playerId, player);
    }
}
