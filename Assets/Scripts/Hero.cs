using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class Hero : MonoBehaviour
{
    public HeroType heroType;
    public HeroRank heroRank = HeroRank.Bronze;

    [SerializeField] Material bronzeMaterial;
    [SerializeField] Material silverMaterial;
    [SerializeField] Material goldMaterial;

    private HeroStats currentStats;
    private int xp = 0;
    private int currentEnergy = 0;  //Aktuelle Energie des Helden

    private EnergyBar energyBar;
    private XPLightManager xpLightManager;

    private bool canSendBomb = false;
    private bool canMakeAction = false;

    private void Awake()
    {
        InitialUpdateHeroStats();
    }

    //Methode, um XP hinzuzuf�gen und den Rang zu erh�hen
    public void AddXP(int amount)
    {
        xp += amount;
        if (xp >= 6)
        {
            RankUp();
        }
        xpLightManager.UpdateXPLamps(xp);
    }

    //Methode f�r den Rangaufstieg
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
            UpdateHeroAppearence();
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

    private void UpdateHeroAppearence()
    {
        MeshRenderer renderer = this.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            switch (heroRank)
            {
                case HeroRank.Bronze:
                    renderer.material = bronzeMaterial;
                    break;

                case HeroRank.Silver:
                    renderer.material = silverMaterial;
                    break;

                case HeroRank.Gold:
                    renderer.material = goldMaterial;
                    break;

                default:
                    Debug.LogError("Heldenrang " + heroRank + "ist nicht verf�gbar!");
                    break;
            }
        }
    }

    // Beispiel f�r das Senden einer Bombe
    public void SendBomb()
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
            canMakeAction = true;
        }

        energyBar.UpdateEnergieDisplay(currentEnergy, maxEnergy);
    }

    private void ActivateAction(HeroType type)
    {
        Debug.Log(type + " f�hrt seine Aktion aus!");
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

    public bool getCanSendBomb()
    {
        bool sendBomb = canSendBomb;
        canSendBomb = false;
        return sendBomb;
    }

    public bool getCanMakeAction()
    {
        bool makeAction = canMakeAction;
        canMakeAction = false;
        return makeAction;
    }
}
