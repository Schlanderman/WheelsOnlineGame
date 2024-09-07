using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [SerializeField]
    private string heroName;     //Name des Helden (z.B. Archer, Engineer)
    [SerializeField]
    private int energy;          //Enerhie, die der Held benötigt zum Agieren
    [SerializeField]
    private int xp = 0;          //Aktuelle XP des Helden
    [SerializeField]
    private int rank = 1;        //Rang des Helden
    [SerializeField]
    private int crownDamage;     //Schaden, den der Held der Krone zufügen kann
    [SerializeField]
    private int bulwarkDamage;   //Schaden, den der Held dem Bulwark zufügen kann

    //Methode, um XP hinzuzufügen und den Rang zu erhöhen
    public void AddXP(int amount)
    {
        xp += amount;
        if (xp >= 6)
        {
            RankUp();
        }
    }

    //Methode für den Rangaufstieg
    private void RankUp()
    {
        xp = 0;
        rank++;
        if (rank > 3)
        {
            rank = 3;       //Maximaler Rang ist Gold
            SendBomb();     // Sende Bombe bei Gold
        }
    }

    // Beispiel für das Senden einer Bombe
    private void SendBomb()
    {
        Debug.Log(heroName + " hat eine Bombe geschickt!");
        // TODO
    }
}
