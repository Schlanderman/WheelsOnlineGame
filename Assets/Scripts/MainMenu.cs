using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject multiplayerMenu;

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
        Debug.Log("Host Game clicked!");

        SceneManager.LoadScene("WheelsGameScene");    //Wechsle zur Spielszene
        //Code zum Hosten eines Spiels kommt hier hin
    }

    //Join Game
    public void JoinGame()
    {
        Debug.Log("Join Game clicked");

        //Code zum Beitreten eines Spiels kommt hier hin
    }

    //Spiel beenden
    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }
}
