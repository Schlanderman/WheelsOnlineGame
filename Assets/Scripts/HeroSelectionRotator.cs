using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelectionRotator : MonoBehaviour
{
    [SerializeField]
    private GameObject[] availableHeroes;           //Liste der verfügbaren Helden
    private int currentSquareHeroIndex = 0;         //Aktuelle Position des Square Helden in der Liste
    private int currentDiamondHeroIndex = 0;        //Aktuelle Position des Diamond Helden in der Liste

    [SerializeField]
    private Transform squareHeroSpawnPoint;     //Position, an der der Square-Held erscheinen soll
    [SerializeField]
    private Transform diamondHeroSpawnPoint;    //Position, an der der Diamond-Held erscheinen soll

    private GameObject activeSquareHero;    //Referenz zum Object des Square-Helden
    private GameObject activeDiamondHero;   //Referenz zum Object des Diamond-Helden

    //Initialisierung
    private void Start()
    {
        UpdateHero("Square");
        UpdateHero("Diamond");
    }

    //Methode zum Rotieren nach rechts
    public void RotateRight(string heroType)
    {
        if (heroType == "Square")
        {
            currentSquareHeroIndex = (currentSquareHeroIndex + 1) % availableHeroes.Length;
            UpdateHero("Square");
        }
        else if (heroType == "Diamond")
        {
            currentDiamondHeroIndex = (currentDiamondHeroIndex + 1) % availableHeroes.Length;
            UpdateHero("Diamond");
        }
    }

    //Methode zum Rotieren nach links
    public void RotateLeft(string heroType)
    {
        if (heroType == "Square")
        {
            currentSquareHeroIndex = (currentSquareHeroIndex - 1 + availableHeroes.Length) % availableHeroes.Length;
            UpdateHero("Square");
        }
        else if (heroType == "Diamond")
        {
            currentDiamondHeroIndex = (currentDiamondHeroIndex - 1 + availableHeroes.Length) % availableHeroes.Length;
            UpdateHero("Diamond");
        }
    }

    //Methode zum Aktualisieren der 3D-Helden in der Szene
    private void UpdateHero(string heroType)
    {
        if (heroType == "Square")
        {
            //Lösche den aktuellen Square-Held, wenn einer existiert
            if (activeSquareHero != null)
            {
                Destroy(activeSquareHero);
            }

            //Instanziere den neuen Square-Held an der vorgesehenen Position
            activeSquareHero = Instantiate(availableHeroes[currentSquareHeroIndex], squareHeroSpawnPoint.position, squareHeroSpawnPoint.rotation);
            Debug.Log("Square Hero: " + availableHeroes[currentSquareHeroIndex].name + " wurde ausgewählt.");
        }
        else if (heroType == "Diamond")
        {
            //Lösche den aktuellen Diamond-Held, wenn einer existiert
            if (activeSquareHero != null)
            {
                Destroy(activeDiamondHero);
            }

            //Instanziere den neuen Square-Held an der vorgesehenen Position
            activeDiamondHero = Instantiate(availableHeroes[currentDiamondHeroIndex], diamondHeroSpawnPoint.position, diamondHeroSpawnPoint.rotation);
            Debug.Log("Diamond Hero: " + availableHeroes[currentDiamondHeroIndex].name + " wurde ausgewählt.");
        }
    }
}
