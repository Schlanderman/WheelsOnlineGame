using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public HeroType heroType;
    public HeroRank heroRank = HeroRank.Bronze;

    private HeroStats currentStats;
    private int xp = 0;

    private void Start()
    {
        UpdateHeroStats();
    }

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
        if (heroRank == HeroRank.Gold)
        {
            SendBomb();     //Sende Bombe bei Gold
        }
        else
        {
            heroRank++;
            UpdateHeroStats();
        }
    }

    private void UpdateHeroStats()
    {
        currentStats = HeroManager.heroData[(heroType, heroRank)];
        Debug.Log($"{heroType} auf {heroRank} hat {currentStats.energyToAct} Energie, {currentStats.crownDamage} KronenSchaden, {currentStats.bulwarkDamage} Bulwarkschaden, {currentStats.delayAdding} Delay, {currentStats.healingAdding} Healing und {currentStats.energyAdding} Energiezufuhr.");
    }

    // Beispiel für das Senden einer Bombe
    private void SendBomb()
    {
        Debug.Log(heroType + " hat eine Bombe geschickt!");
        // TODO
    }

    //Methode, um Energie hinzuzufügen
}
