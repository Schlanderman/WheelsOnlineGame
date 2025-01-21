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

    //Multiplayer-Menü anzeigen
    public void Multiplayer()
    {
        mainMenu.SetActive(false);
        multiplayerMenu.SetActive(true);
        gameBrowser.SetActive(false);
    }

    //Zurück zum Hauptmenü
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

    //Methode für das Verlassen einer Lobby
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
