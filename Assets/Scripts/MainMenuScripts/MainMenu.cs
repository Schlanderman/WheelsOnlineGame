using System.Collections;
using System.Collections.Generic;
//using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject multiplayerMenu;
    public GameObject lobbyMenu;
    public GameObject gameBrowser;

    public LobbyManager lobbyManager;

    private void Start()
    {
        multiplayerMenu.SetActive(false);
    }

    //Singleplayer-Dummy Button
    public void Singleplayer()
    {
        Debug.Log("Singleplayer Mode: Coming Soon!");
    }

    //Multiplayer-Men� anzeigen
    public void Multiplayer()
    {
        mainMenu.SetActive(false);
        multiplayerMenu.SetActive(true);
        gameBrowser.SetActive(false);
    }

    //Zur�ck zum Hauptmen�
    public void BackToMainMenu()
    {
        multiplayerMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    //Host Game
    public void HostGame()
    {
        multiplayerMenu.SetActive(false);
        lobbyMenu.SetActive(true);
    }

    //Join Game
    public void OpenGameBrowser()
    {
        gameBrowser.SetActive(true);
        lobbyManager.FetchLobbies();
    }

    //Methode f�r das Verlassen einer Lobby
    public void ExitLobbyBrowser()
    {
        //Browser ausblenden
        gameBrowser.SetActive(false);
    }

    //Spiel beenden
    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }
}
