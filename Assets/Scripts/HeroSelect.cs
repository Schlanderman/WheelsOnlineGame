using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSelect : MonoBehaviour
{
    [SerializeField]
    private Hero[] availableHeros;  //Liste der verf�gbaren Helden
    private Hero squareHero;        //Held f�r Square
    private Hero diamondHero;       //Held f�r Diamond

    //Methode, um die Helden zuzuweisen
    public void SelectHero(Hero chosenHero, string type)
    {

    }
}
