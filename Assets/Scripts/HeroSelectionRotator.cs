using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelectionRotator : MonoBehaviour
{
    [SerializeField] private GameObject[] availableHeroes;      //Liste der verfügbaren Helden

    //Referenzen auf die UI-Updater
    [SerializeField] private HeroUIUpdater playerSquareHeroUIUpdater;
    [SerializeField] private HeroUIUpdater playerDiamondHeroUIUpdater;
    [SerializeField] private HeroUIUpdater enemySquareHeroUIUpdater;
    [SerializeField] private HeroUIUpdater enemyDiamondHeroUIUpdater;

    //Referenzen auf die Energy Bars
    [SerializeField] private EnergyBar playerSquareEnergyBar;
    [SerializeField] private EnergyBar playerDiamondEnergyBar;
    [SerializeField] private EnergyBar enemySquareEnergyBar;
    [SerializeField] private EnergyBar enemyDiamondEnergyBar;

    //Referenzen auf die XP Lichter
    [SerializeField] private XPLightManager playerSquareXPLightManager;
    [SerializeField] private XPLightManager playerDiamondXPLightManager;
    [SerializeField] private XPLightManager enemySquareXPLightManager;
    [SerializeField] private XPLightManager enemyDiamondXPLightManager;

    //Referenzen auf die Spawnpositionen der Helden
    [SerializeField] private Transform playerSquareHeroSpawnPoint;      //Position, an der der Spieler-Square-Held erscheinen soll
    [SerializeField] private Transform playerDiamondHeroSpawnPoint;     //Position, an der der Spieler-Diamond-Held erscheinen soll
    [SerializeField] private Transform enemySquareHeroSpawnPoint;       //Position, an der der Gegner-Square-Held erscheinen soll
    [SerializeField] private Transform enemyDiamondHeroSpawnPoint;      //Position, an der der Gegner-Diamond-Held erscheinen soll

    //Welcher Held in der Liste ist ausgewählt
    private int currentPlayerSquareHeroIndex = 0;               //Aktuelle Position des Spieler Square Helden in der Liste
    private int currentPlayerDiamondHeroIndex = 0;              //Aktuelle Position des Spieler Diamond Helden in der Liste
    private int currentEnemySquareHeroIndex = 0;                //Aktuelle Position des Gegner Square Helden in der Liste
    private int currentEnemyDiamondHeroIndex = 0;               //Aktuelle Position des Gegner Diamond Helden in der Liste

    //Referenz auf den ausgewählten Helden
    private Hero currentPlayerSquareHero;
    private Hero currentPlayerDiamondHero;
    private Hero currentEnemySquareHero;
    private Hero currentEnemyDiamondHero;

    //Referenz auf das aktive Objekt des Helden
    private GameObject activePlayerSquareHero;      //Referenz zum Objekt des Spieler-Square-Helden
    private GameObject activePlayerDiamondHero;     //Referenz zum Objekt des Spieler-Diamond-Helden
    private GameObject activeEnemySquareHero;       //Referenz zum Objekt des Gegner-Square-Helden
    private GameObject activeEnemyDiamondHero;      //Referenz zum Objekt des Gegner-Diamond-Helden

    //Initialisierung
    private void Start()
    {
        UpdateHero("PlayerSquare");
        UpdateHero("PlayerDiamond");
        UpdateHero("EnemySquare");
        UpdateHero("EnemyDiamond");
    }

    //Methode zum Rotieren nach rechts
    public void RotateRight(string heroType)
    {
        if (heroType == "PlayerSquare")
        {
            currentPlayerSquareHeroIndex = (currentPlayerSquareHeroIndex + 1) % availableHeroes.Length;
        }
        else if (heroType == "PlayerDiamond")
        {
            currentPlayerDiamondHeroIndex = (currentPlayerDiamondHeroIndex + 1) % availableHeroes.Length;
        }
        else if (heroType == "EnemySquare")
        {
            currentEnemySquareHeroIndex = (currentEnemySquareHeroIndex + 1) % availableHeroes.Length;
        }
        else if (heroType == "EnemyDiamond")
        {
            currentEnemyDiamondHeroIndex = (currentEnemyDiamondHeroIndex + 1) % availableHeroes.Length;
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
        if (heroType == "PlayerSquare")
        {
            currentPlayerSquareHeroIndex = (currentPlayerSquareHeroIndex - 1 + availableHeroes.Length) % availableHeroes.Length;
        }
        else if (heroType == "PlayerDiamond")
        {
            currentPlayerDiamondHeroIndex = (currentPlayerDiamondHeroIndex - 1 + availableHeroes.Length) % availableHeroes.Length;
        }
        else if (heroType == "EnemySquare")
        {
            currentEnemySquareHeroIndex = (currentEnemySquareHeroIndex - 1 + availableHeroes.Length) % availableHeroes.Length;
        }
        else if (heroType == "EnemyDiamond")
        {
            currentEnemyDiamondHeroIndex = (currentEnemyDiamondHeroIndex - 1 + availableHeroes.Length) % availableHeroes.Length;
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
        if (heroType == "PlayerSquare")
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
                playerSquareHeroUIUpdater.UpdateHeroDisplay(currentPlayerSquareHero);
                currentPlayerSquareHero.SetEnergyBar(playerSquareEnergyBar);
                playerSquareEnergyBar.UpdateEnergieDisplay(0, currentPlayerSquareHero.GetMaxEnergy());
                currentPlayerSquareHero.SetXPLightManager(playerSquareXPLightManager);
            }
        }

        else if (heroType == "PlayerDiamond")
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
                playerDiamondHeroUIUpdater.UpdateHeroDisplay(currentPlayerDiamondHero);
                currentPlayerDiamondHero.SetEnergyBar(playerDiamondEnergyBar);
                playerDiamondEnergyBar.UpdateEnergieDisplay(0, currentPlayerDiamondHero.GetMaxEnergy());
                currentPlayerDiamondHero.SetXPLightManager(playerDiamondXPLightManager);
            }
        }

        else if (heroType == "EnemySquare")
        {
            //Lösche den aktuellen Gegner-Square-Held, wenn einer existiert
            if (activeEnemySquareHero != null)
            {
                Destroy(activeEnemySquareHero);
            }

            //Instanziere den neuen Gegner-Square-Held an der vorgesehenen Position
            activeEnemySquareHero = Instantiate(availableHeroes[currentEnemySquareHeroIndex], enemySquareHeroSpawnPoint.position, enemySquareHeroSpawnPoint.rotation);

            activeEnemySquareHero.transform.SetParent(enemySquareHeroSpawnPoint, true);

            currentEnemySquareHero = activeEnemySquareHero.GetComponent<Hero>();

            if (currentEnemySquareHero != null)
            {
                enemySquareHeroUIUpdater.UpdateHeroDisplay(currentEnemySquareHero);
                currentEnemySquareHero.SetEnergyBar(enemySquareEnergyBar);
                enemySquareEnergyBar.UpdateEnergieDisplay(0, currentEnemySquareHero.GetMaxEnergy());
                currentEnemySquareHero.SetXPLightManager(enemyDiamondXPLightManager);
            }
        }

        else if (heroType == "EnemyDiamond")
        {
            //Lösche den aktuellen Gegner-Diamond-Helden, wenn einer existiert
            if (activeEnemyDiamondHero != null)
            {
                Destroy(activeEnemyDiamondHero);
            }

            //Instanziere den neuen Gegner-Diamond-Held an der vorgesehenen Position
            activeEnemyDiamondHero = Instantiate(availableHeroes[currentEnemyDiamondHeroIndex], enemyDiamondHeroSpawnPoint.position, enemyDiamondHeroSpawnPoint.rotation);

            activeEnemyDiamondHero.transform.SetParent(enemyDiamondHeroSpawnPoint, true);

            currentEnemyDiamondHero = activeEnemyDiamondHero.GetComponent<Hero>();

            if(currentEnemyDiamondHero != null)
            {
                enemyDiamondHeroUIUpdater.UpdateHeroDisplay(currentEnemyDiamondHero);
                currentEnemyDiamondHero.SetEnergyBar(enemyDiamondEnergyBar);
                enemyDiamondEnergyBar.UpdateEnergieDisplay(0, currentEnemyDiamondHero.GetMaxEnergy());
                currentEnemyDiamondHero.SetXPLightManager(enemyDiamondXPLightManager);
            }
        }
    }

    //Getter für die Helden
    public Hero GetPlayerSquareHero()
    {
        return currentPlayerSquareHero;
    }

    public Hero GetPlayerDiamondHero()
    {
        return currentPlayerDiamondHero;
    }

    public Hero GetEnemySquareHero()
    {
        return currentEnemySquareHero;
    }

    public Hero GetEnemyDiamondHero()
    {
        return currentEnemyDiamondHero;
    }
}
