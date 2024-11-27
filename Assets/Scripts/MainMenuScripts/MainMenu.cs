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
        //Startet den Server und den Host
        //TODO

        multiplayerMenu.SetActive(false);
        lobbyMenu.SetActive(true);
    }

    //Join Game
    public void JoinGame()
    {
        //Startet den Client
        //TODO

        //Szene zum Lobbymen� wechseln
        multiplayerMenu.SetActive(false);
        lobbyMenu.SetActive(true);
    }

    //Methode f�r das Verlassen einer Lobby
    public void LeaveLobby()
    {
        //Networkmanager Code zum Verlassen
        //TODO

        //Szene zur�ck zum Multiplayermen� wechseln
        lobbyMenu.SetActive(false);
        multiplayerMenu.SetActive(true);
    }

    //Spiel beenden
    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }
}
