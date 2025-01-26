using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandleSystemMessagesUI : MonoBehaviour
{
    [Header("HandleUI")]
    [SerializeField] private GameObject handleSystemMessagesUI;
    [SerializeField] private TMP_Text systemMessageText;
    [SerializeField] private Button backToMainMenuButton;

    [Header("Andere Menüs")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject multiplayerMenu;
    [SerializeField] private GameObject lobbyMenu;

    private void Awake()
    {
        Hide();
        backToMainMenuButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        LobbyManager.Instance.OnCreateLobbyStarted += LobbyManager_OnCreateLobbyStarted;
        LobbyManager.Instance.OnCreateLobbyCompleted += LobbyManager_OnCreateLobbyCompleted;
        LobbyManager.Instance.OnCreateLobbyFailed += LobbyManager_OnCreateLobbyFailed;
        LobbyManager.Instance.OnJoinStarted += LobbyManager_OnJoinStarted;
        LobbyManager.Instance.OnJoinCompleted += LobbyManager_OnJoinCompleted;
        LobbyManager.Instance.OnJoinFailed += LobbyManager_OnJoinFailed;
    }

    private void LobbyManager_OnCreateLobbyStarted(object sender, System.EventArgs e)
    {
        Debug.Log("Creating Lobby aufruf...");
        ShowMessage("Creating Lobby...");
    }

    private void LobbyManager_OnCreateLobbyCompleted(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void LobbyManager_OnCreateLobbyFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to create Lobby!");
        ShowButtonToMenu();
    }

    private void LobbyManager_OnJoinStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Joining Lobby...");
    }

    private void LobbyManager_OnJoinCompleted(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void LobbyManager_OnJoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to join Lobby!");
        ShowButtonToMenu();
    }


    //Helperfunktionen
    private void Show()
    {
        handleSystemMessagesUI.SetActive(true);
    }

    private void Hide()
    {
        handleSystemMessagesUI.SetActive(false);
    }

    public void BackToMainMenu()
    {
        Hide();
        multiplayerMenu.SetActive(false);
        lobbyMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    private void ShowMessage(string message)
    {
        Show();
        systemMessageText.text = message;
    }

    private void ShowButtonToMenu()
    {
        backToMainMenuButton.gameObject.SetActive(true);
    }
}
