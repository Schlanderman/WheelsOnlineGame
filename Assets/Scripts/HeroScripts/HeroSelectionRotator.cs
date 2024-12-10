using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelectionRotator : MonoBehaviour
{
    [SerializeField] private EnemyScript enemyScript;  //Offline only

    [SerializeField] private GameObject[] availableHeroes;      //Liste der verfügbaren Helden

    //Referenzen auf die UI-Updater
    [SerializeField] private HeroUIUpdater playerSquareHeroUIUpdater;
    [SerializeField] private HeroUIUpdater playerDiamondHeroUIUpdater;

    //Referenzen auf die Energy Bars
    [SerializeField] private EnergyBar playerSquareEnergyBar;
    [SerializeField] private EnergyBar playerDiamondEnergyBar;

    //Referenzen auf die XP Lichter
    [SerializeField] private XPLightManager playerSquareXPLightManager;
    [SerializeField] private XPLightManager playerDiamondXPLightManager;

    //Referenzen auf die Spawnpositionen der Helden
    [SerializeField] private Transform playerSquareHeroSpawnPoint;      //Position, an der der Spieler-Square-Held erscheinen soll
    [SerializeField] private Transform playerDiamondHeroSpawnPoint;     //Position, an der der Spieler-Diamond-Held erscheinen soll

    //Welcher Held in der Liste ist ausgewählt
    private int currentPlayerSquareHeroIndex = 0;               //Aktuelle Position des Spieler Square Helden in der Liste
    private int currentPlayerDiamondHeroIndex = 0;              //Aktuelle Position des Spieler Diamond Helden in der Liste

    //Referenz auf den ausgewählten Helden
    private Hero currentPlayerSquareHero;
    private Hero currentPlayerDiamondHero;

    //Referenz auf das aktive Objekt des Helden
    private GameObject activePlayerSquareHero;      //Referenz zum Objekt des Spieler-Square-Helden
    private GameObject activePlayerDiamondHero;     //Referenz zum Objekt des Spieler-Diamond-Helden

    //Events
    public event Action<int, int, string> OnActivateChangeHero;

    //Initialisierung
    private void Start()
    {
        UpdateHero("Square");
        UpdateHero("Diamond");

        InitialHeroSetting.Instance.OnGetHeroes += InitialHeroSetting_OnGetHeroes;
        InitialHeroSetting.Instance.OnGetPlayerComponents += InitialHeroSetting_OnGetPlayerComponents;
    }

    private void InitialHeroSetting_OnGetHeroes(object sender, System.EventArgs e)
    {
        if (enemyScript != null)
        {
            InitialHeroSetting.Instance.SetPlayerHeroes(enemyScript.playerId, currentPlayerSquareHero, currentPlayerDiamondHero);
        }
        else
        {
            InitialHeroSetting.Instance.SetPlayerHeroes(PlayerScript.Instance.playerId, currentPlayerSquareHero, currentPlayerDiamondHero);
        }
    }

    private void InitialHeroSetting_OnGetPlayerComponents(object sender, System.EventArgs e)
    {
        if (enemyScript != null)
        {
            InitialHeroSetting.Instance.SetPlayerComponents(
                enemyScript.playerId,
                enemyScript.GetSquareHeroActions(),
                enemyScript.GetDiamondHeroActions(),
                enemyScript.GetEvaluationManager()
                );
        }
        else
        {
            InitialHeroSetting.Instance.SetPlayerComponents(
                PlayerScript.Instance.playerId,
                PlayerScript.Instance.GetSquareHeroActions(),
                PlayerScript.Instance.GetDiamondHeroActions(),
                PlayerScript.Instance.GetEvaluationManager()
                );
        }
    }

    //Methode zum Rotieren nach rechts
    public void RotateRight(string heroType)
    {
        if (heroType == "Square")
        {
            currentPlayerSquareHeroIndex = (currentPlayerSquareHeroIndex + 1) % availableHeroes.Length;
        }
        else if (heroType == "Diamond")
        {
            currentPlayerDiamondHeroIndex = (currentPlayerDiamondHeroIndex + 1) % availableHeroes.Length;
        }
        else
        {
            Debug.LogError("Der HeroType " + heroType + " existiert nicht!");
            return;
        }

        UpdateHero(heroType);
    }

    //Methode zum Rotieren nach links
    public void RotateLeft(string heroType)
    {
        if (heroType == "Square")
        {
            currentPlayerSquareHeroIndex = (currentPlayerSquareHeroIndex - 1 + availableHeroes.Length) % availableHeroes.Length;
        }
        else if (heroType == "Diamond")
        {
            currentPlayerDiamondHeroIndex = (currentPlayerDiamondHeroIndex - 1 + availableHeroes.Length) % availableHeroes.Length;
        }
        else
        {
            Debug.LogError("Der HeroType " + heroType + " existiert nicht!");
            return;
        }

        UpdateHero(heroType);
    }

    //Methode zum Aktualisieren der 3D-Helden in der Szene
    private void UpdateHero(string heroType)
    {
        OnActivateChangeHero?.Invoke(currentPlayerSquareHeroIndex, currentPlayerDiamondHeroIndex, heroType);

        if (heroType == "Square")
        {
            //Lösche den aktuellen Spieler-Square-Held, wenn einer existiert
            if (activePlayerSquareHero != null)
            {
                Destroy(activePlayerSquareHero);
            }

            //Instanziere den neuen Spieler-Square-Held an der vorgesehenen Position
            activePlayerSquareHero = Instantiate(availableHeroes[currentPlayerSquareHeroIndex], playerSquareHeroSpawnPoint.position, playerSquareHeroSpawnPoint.rotation);

            activePlayerSquareHero.transform.SetParent(playerSquareHeroSpawnPoint, true);

            currentPlayerSquareHero = activePlayerSquareHero.GetComponent<Hero>();

            if (currentPlayerSquareHero != null)
            {
                currentPlayerSquareHero.SetUIUpdater(playerSquareHeroUIUpdater);
                playerSquareHeroUIUpdater.UpdateHeroDisplay(currentPlayerSquareHero);
                currentPlayerSquareHero.SetEnergyBar(playerSquareEnergyBar);
                playerSquareEnergyBar.UpdateEnergieDisplay(0, currentPlayerSquareHero.GetMaxEnergy());
                currentPlayerSquareHero.SetXPLightManager(playerSquareXPLightManager);
            }
        }

        else if (heroType == "Diamond")
        {
            //Lösche den aktuellen Spieler-Diamond-Held, wenn einer existiert
            if (activePlayerDiamondHero != null)
            {
                Destroy(activePlayerDiamondHero);
            }

            //Instanziere den neuen Spieler-Diamond-Held an der vorgesehenen Position
            activePlayerDiamondHero = Instantiate(availableHeroes[currentPlayerDiamondHeroIndex], playerDiamondHeroSpawnPoint.position, playerDiamondHeroSpawnPoint.rotation);

            activePlayerDiamondHero.transform.SetParent(playerDiamondHeroSpawnPoint, true);

            currentPlayerDiamondHero = activePlayerDiamondHero.GetComponent<Hero>();

            if (currentPlayerDiamondHero != null)
            {
                currentPlayerDiamondHero.SetUIUpdater(playerDiamondHeroUIUpdater);
                playerDiamondHeroUIUpdater.UpdateHeroDisplay(currentPlayerDiamondHero);
                currentPlayerDiamondHero.SetEnergyBar(playerDiamondEnergyBar);
                playerDiamondEnergyBar.UpdateEnergieDisplay(0, currentPlayerDiamondHero.GetMaxEnergy());
                currentPlayerDiamondHero.SetXPLightManager(playerDiamondXPLightManager);
            }
        }
    }
}
