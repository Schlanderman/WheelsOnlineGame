using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [SerializeField]
    private string heroName;     //Name des Helden (z.B. Archer, Engineer)
    [SerializeField]
    private int energy;          //Enerhie, die der Held ben�tigt zum Agieren
    [SerializeField]
    private int xp = 0;          //Aktuelle XP des Helden
    [SerializeField]
    private int rank = 1;        //Rang des Helden
    [SerializeField]
    private int crownDamage;     //Schaden, den der Held der Krone zuf�gen kann
    [SerializeField]
    private int bulwarkDamage;   //Schaden, den der Held dem Bulwark zuf�gen kann

    //Methode, um XP hinzuzuf�gen und den Rang zu erh�hen
    public void AddXP(int amount)
    {
        xp += amount;
        if (xp >= 6)
        {
            RankUp();
        }
    }

    //Methode f�r den Rangaufstieg
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

    // Beispiel f�r das Senden einer Bombe
    private void SendBomb()
    {
        Debug.Log(heroName + " hat eine Bombe geschickt!");
        // TODO
    }
}
