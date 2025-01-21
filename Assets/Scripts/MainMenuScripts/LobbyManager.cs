using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using System;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    [Header("Lobby Objects")]
    public GameObject lobbyPanel;
    public Transform playerListParent;
    public GameObject playerListItemPrefab;
    public Button startGameButton;

    [Header("Gamebrowser Objects")]
    public GameObject multiplayerMenu;
    public GameObject browsePanel;
    public Transform lobbyListParent;
    public GameObject lobbyListItemPrefab;

    private readonly int maxPlayers = 2;

    public string currentLobbyId { get; private set; }
    public Lobby hostLobby { get; private set; }
    public Lobby joinedLobby { get; private set; }
    private Coroutine pollingCoroutine;

    //Keys
    string KEY_PLAYER_NAME = "PlayerName";
    string KEY_START_GAME = "StartGame";
    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";

    //Events
    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnCreateLobbyCompleted;
    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnJoinStarted;
    public event EventHandler OnJoinCompleted;
    public event EventHandler OnJoinFailed;

    private void Awake()
    {
        Instance = this;
    }

    private async void Start()
    {
        //Initialisiere die Unity Services
        await InitializeUnityServiceAsync();

        //Authentifiziere den Spieler
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await SignInAnonymouslyAsync();
        }
    }

    //1. Lobby erstellen
    public async void CreateLobby()
    {
        try
        {
            OnCreateLobbyStarted?.Invoke(this, new EventArgs());

            //Lobby Optionen
            CreateLobbyOptions options = new CreateLobbyOptions();
            options.IsPrivate = false;
            options.Player = GetPlayer();
            options.Data = new Dictionary<string, DataObject>
            {
                { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, "0") }
            };

            //Allocate vom Relay abrufen
            Allocation allocation = await RelayManager.Instance.CreateRelay();

            string joinRelayCode = await RelayManager.Instance.GetRelayJoinCode(allocation);

            RelayServerData serverData = AllocationUtils.ToRelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);

            NetworkManager.Singleton.StartHost();

            //Lobby erstellen
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(ProfileData.Instance.ProfileName + "'s Lobby", maxPlayers, options);

            hostLobby = lobby;
            joinedLobby = hostLobby;

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions { 
                Data = new Dictionary<string, DataObject>
                {
                    { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, joinRelayCode) }
                }
            });

            OnCreateLobbyCompleted?.Invoke(this, EventArgs.Empty);
            DisplayLobby();
            await StartPollingForUpdates(currentLobbyId);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Lobby creation failed: {e.Message}");
            OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    //2. Verfügbare Lobbys abrufen
    public async void FetchLobbies()
    {
        try
        {
            var lobbies = await LobbyService.Instance.QueryLobbiesAsync();
            UpdateLobbyList(lobbies.Results);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Fetching lobbies failed: {e.Message}");
        }
    }

    //3. Lobby beitreten
    public async void JoinLobby(string lobbyId)
    {
        try
        {
            OnJoinStarted?.Invoke(this, EventArgs.Empty);
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions
            {
                Player = GetPlayer()
            };

            Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, options);
            joinedLobby = lobby;
            DisplayLobby();
            await StartPollingForUpdates(currentLobbyId);

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await RelayManager.Instance.JoinRelay(relayJoinCode);

            RelayServerData serverData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);

            NetworkManager.Singleton.StartClient();
            OnJoinCompleted?.Invoke(this, EventArgs.Empty);
        }
        catch(LobbyServiceException e)
        {
            Debug.LogError($"Joining lobby failed: {e.Message}");
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    //4. Lobby verlassen
    public async void LeaveLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                joinedLobby = null;
                hostLobby = null;
                lobbyPanel.SetActive(false);
                multiplayerMenu.SetActive(true);
                StopPollingForLobbyUpdates();
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Leaving lobby failed: {e.Message}");
            }
        }
    }

    //5. Game Starten
    public void StartGame()
    {
        if (IsLobbyHost())
        {
            try
            {
                MultiplayerInitialisation.Instance.SetIsInMenu(false);

                //Hier logik für den Spielstart hinzufügen
                Debug.Log($"Starting the Game, mit {RelayManager.Instance}");

                //Szene für alle Spieler Synchronisieren
                StopPollingForLobbyUpdates();
                NetworkManager.Singleton.SceneManager.LoadScene("WheelsGameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
                //DeleteLobby();
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Couldn't start the Game: {e.Message}");
            }
        }
    }

    //Hilfsmethoden
    //Starte das Polling, wenn du einer Lobby beitrittst oder eine erstellst
    public async Task StartPollingForUpdates(string lobbyId)
    {
        currentLobbyId = lobbyId;

        //Rufe die Lobby-Daten initial ab
        await FetchLobbyData();

        //Starte die Coroutine, um regelmäßig Updates abzurufen
        if (pollingCoroutine == null)
        {
            pollingCoroutine = StartCoroutine(PollLobbyUpdates());
        }
    }

    //Stoppe das Polling, wenn der Spieler die Lobby verlässt
    public void StopPollingForLobbyUpdates()
    {
        if (pollingCoroutine != null)
        {
            StopCoroutine(pollingCoroutine);
            pollingCoroutine = null;
        }
        joinedLobby = null;
        currentLobbyId = null;
    }

    private void DisplayLobby()
    {
        lobbyPanel.SetActive(true);
        browsePanel.SetActive(false);
        multiplayerMenu.SetActive(false);

        //Spieleranzeige aktualisieren
        foreach (Transform child in playerListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var player in joinedLobby.Players)
        {
            var item = Instantiate(playerListItemPrefab, playerListParent);
            string playerName = player.Data[KEY_PLAYER_NAME].Value;
            //Texture2D playerIcon = ConvertBase64ToTexture(player.Data["PlayerIcon"].Value);
            item.GetComponentInChildren<PlayerSlotItem>().SetPlayerData(playerName, AuthenticationService.Instance.PlayerId);
        }

        //"Start Game"-Button nur für den Host aktivieren
        startGameButton.gameObject.SetActive(IsLobbyHost());
    }

    private void UpdateLobbyList(List<Lobby> lobbies)
    {
        foreach (Transform child in lobbyListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var lobby in lobbies)
        {
            var item = Instantiate(lobbyListItemPrefab, lobbyListParent);

            //Hole das LobbyListItem-Skript
            var lobbyItem = item.GetComponent<LobbyListItem>();

            //Setze Lobby-Details
            lobbyItem.SetLobbyDetails(
                lobby.Name,
                lobby.Players.Count,
                lobby.MaxPlayers
                );

            item.GetComponent<Button>().onClick.AddListener(() => JoinLobby(lobby.Id));
        }
    }

    private async Task InitializeUnityServiceAsync()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            try
            {
                await UnityServices.InitializeAsync();
                Debug.Log("Unity Services initialized successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to initialize Unity Services: {ex.Message}");
            }
        }
    }

    private async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Signed in as Player ID: {AuthenticationService.Instance.PlayerId}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to sign in: {ex.Message}");
        }
    }

    //Coroutine für den Heartbeat
    private IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSeconds(waitTimeSeconds);

        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    // Coroutine für das regelmäßige Polling
    private IEnumerator PollLobbyUpdates()
    {
        while (joinedLobby != null && SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainMenu"))
        {
            yield return new WaitForSeconds(2); // Alle 2 Sekunden updaten
            //Asynchrone Methode aufrufen und darauf warten, dass sie fertig ist
            var fetchTask = FetchLobbyData();
            while (!fetchTask.IsCompleted)
            {
                yield return null;  //Warte, bis der Task abgeschlossen ist
            }
        }
    }

    //Abrufen der aktuellen Lobby-Daten
    private async Task FetchLobbyData()
    {
        try
        {
            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
            joinedLobby = lobby;
            //Debug.Log("Lobby geupdated!");

            if (joinedLobby.Data[KEY_START_GAME].Value != "0")
            {
                //Start Game!
                if (!IsLobbyHost())     //Lobby Host already joined Relay
                {
                    await RelayManager.Instance.JoinRelay(joinedLobby.Data[KEY_START_GAME].Value);
                }

                joinedLobby = null;
            }

            UpdateLobbyUI();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to fetch lobby updates: {ex.Message}");
        }
    }

    private async void DeleteLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);

                joinedLobby = null;
            }
            catch (LobbyServiceException ex)
            {
                Debug.LogError($"Failed to delete lobby: {ex.Message}");
            }
        }
    }

    private void UpdateLobbyUI()
    {
        //Spieleranzeige aktualisieren
        foreach (Transform child in playerListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var player in joinedLobby.Players)
        {
            var item = Instantiate(playerListItemPrefab, playerListParent);
            string playerName = player.Data[KEY_PLAYER_NAME].Value;
            item.GetComponentInChildren<PlayerSlotItem>().SetPlayerData(playerName, AuthenticationService.Instance.PlayerId);
        }
    }

    private Player GetPlayer()
    {
        string profileName = ProfileData.Instance.ProfileName;  //Profilnamen abrufen

        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                {
                    { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, profileName) },
                }
        };
    }

    private Dictionary<string, DataObject> GetData()
    {
        string profileImageBase64 = PrepareProfileImage(ProfileData.Instance.ProfileImage); //Profilbild abrufen

        return new Dictionary<string, DataObject>
        {
            { "ProfileIcon", new DataObject(DataObject.VisibilityOptions.Public, profileImageBase64) }
        };
    }

    private bool IsLobbyHost()
    {
        return joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private string ConvertTextureToBase64(Texture2D texture)
    {
        byte[] imageBytes = texture.EncodeToPNG();  //Bild als PNG speichern
        return System.Convert.ToBase64String(imageBytes);   //In Base64 umwandeln
    }

    private Texture2D ResizeTexture(Texture2D originalTexture, int targetWidth, int targetHeight)
    {
        //Neues Texture2D-Objekt erstellen
        Texture2D resizedTexture = new Texture2D(targetWidth, targetHeight, originalTexture.format, false);

        //Pixel aus der originalen Textur skalieren
        Color[] pixels = originalTexture.GetPixels(0, 0, originalTexture.width, originalTexture.height);
        resizedTexture.SetPixels(pixels);
        resizedTexture.Apply();

        return resizedTexture;
    }

    private string CompressAndConvertToTexture(Texture2D texture, int compressionQuality)
    {
        byte[] imageBytes = texture.EncodeToJPG(compressionQuality);    //JPG statt PNG verwenden
        return System.Convert.ToBase64String(imageBytes);
    }

    public string PrepareProfileImage(Texture2D originalTexture)
    {
        //Textur auf 128x128 reduzieren
        Texture2D resizedTexture = ResizeTexture(originalTexture, 128, 128);

        //Komprimieren und in Base64 konvertieren
        return CompressAndConvertToTexture(resizedTexture, 75);     //75 = mittlere Qualität
    }

    private Texture2D ConvertBase64ToTexture(string base64)
    {
        byte[] imageBytes = System.Convert.FromBase64String(base64);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);
        return texture;
    }
}
