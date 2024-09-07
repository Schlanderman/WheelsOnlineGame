using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSelect : MonoBehaviour
{
    [SerializeField]
    private Hero[] availableHeros;  //Liste der verfügbaren Helden
    private Hero squareHero;        //Held für Square
    private Hero diamondHero;       //Held für Diamond

    //Methode, um die Helden zuzuweisen
    public void SelectHero(Hero chosenHero, string type)
    {

    }
}
