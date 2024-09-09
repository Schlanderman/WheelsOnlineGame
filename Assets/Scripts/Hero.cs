using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class Hero : MonoBehaviour
{
    public HeroType heroType;
    public HeroRank heroRank = HeroRank.Bronze;

    private HeroStats currentStats;
    private int xp = 0;
    private int currentEnergy = 0;  //Aktuelle Energie des Helden

    private EnergyBar energyBar;
    private XPLightManager xpLightManager;

    private void Awake()
    {
        InitialUpdateHeroStats();
    }

    //Methode, um XP hinzuzufügen und den Rang zu erhöhen
    public void AddXP(int amount)
    {
        xp += amount;
        if (xp >= 6)
        {
            RankUp();
        }
        xpLightManager.UpdateXPLamps(xp);
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

    private void InitialUpdateHeroStats()
    {
        currentStats = HeroManager.heroData[(heroType, heroRank)];
        //Debug.Log($"{heroType} auf {heroRank} hat {currentStats.energyToAct} Energie, {currentStats.crownDamage} KronenSchaden, {currentStats.bulwarkDamage} Bulwarkschaden, {currentStats.delayAdding} Delay, {currentStats.healingAdding} Healing und {currentStats.energyAdding} Energiezufuhr.");
    }

    private void UpdateHeroStats()
    {
        currentStats = HeroManager.heroData[(heroType, heroRank)];
        energyBar.UpdateEnergieDisplay(currentEnergy, getMaxEnergy());
        //Debug.Log($"{heroType} auf {heroRank} hat {currentStats.energyToAct} Energie, {currentStats.crownDamage} KronenSchaden, {currentStats.bulwarkDamage} Bulwarkschaden, {currentStats.delayAdding} Delay, {currentStats.healingAdding} Healing und {currentStats.energyAdding} Energiezufuhr.");
    }

    // Beispiel für das Senden einer Bombe
    private void SendBomb()
    {
        Debug.Log(heroType + " hat eine Bombe geschickt!");
        // TODO
    }

    //Methode zur Energieerfassung
    public void AddEnergy(int amount)
    {
        currentEnergy += amount;
        int maxEnergy = getMaxEnergy();
        if (currentEnergy >= maxEnergy)
        {
            ActivateAction(heroType);
        }

        energyBar.UpdateEnergieDisplay(currentEnergy, maxEnergy);
    }

    private void ActivateAction(HeroType type)
    {
        Debug.Log(type + " führt seine Aktion aus!");
        currentEnergy = 0;
        //TODO
    }

    //Methoden um die Werte auszulesen
    public int getCrownDamage()
    {
        return currentStats.crownDamage;
    }

    public int getBulwarkDamage()
    {
        return currentStats.bulwarkDamage;
    }

    public int getDelayAdding()
    {
        return currentStats.delayAdding;
    }

    public int getHealingAdding()
    {
        return currentStats.healingAdding;
    }

    public int getEnergyAdding()
    {
        return currentStats.energyAdding;
    }

    public string getHeroName()
    {
        return heroType.ToString();
    }

    public int getMaxEnergy()
    {
        return currentStats.energyToAct;
    }

    public void setEnergyBar(EnergyBar bar)
    {
        energyBar = bar;
    }

    public void setXPLightManager(XPLightManager light)
    {
        xpLightManager = light;
    }
}
