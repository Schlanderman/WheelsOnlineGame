using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject multiplayerMenu;
    public TMP_InputField ipInput;

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
        NetworkManager.singleton.StartHost();

        SceneManager.LoadScene("WheelsGameScene");    //Wechsle zur Spielszene
    }

    //Join Game
    public void JoinGame()
    {
        Debug.Log("Join Game clicked");

        string ipAdress = ipInput.text;

        if (!IsValidIPAdress(ipAdress))
        {
            Debug.LogWarning(ipAdress + " ist keine gültige IP-Adresse!");
            return;
        }

        //Debug.Log("Als Client mit Server verbinden!");
        NetworkManager.singleton.networkAddress = ipAdress;
        NetworkManager.singleton.StartClient();
    }

    //Spiel beenden
    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }

    //Funktion zur überprüfung der IP-Adresse
    private bool IsValidIPAdress(string ipAdress)
    {
        //Regex für IPv4-Adressen (Format: 0-255.0-255.0-255.0-255)
        string pattern = @"^(\d{1,3}\.){3}\d{1,3}$";

        //Erst prüfen, ob das Format korrekt ist
        if (Regex.IsMatch(ipAdress, pattern))
        {
            //Dann die einzelnen Zahlen überprüfen, ob sie im Bereich 0 - 255 liegen
            string[] parts = ipAdress.Split('.');
            foreach (string part in parts)
            {
                int number = int.Parse(part);
                if (number < 0 || number > 255)
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}
