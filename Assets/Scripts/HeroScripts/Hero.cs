using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using Unity.Netcode;
using Unity.Collections;

public class Hero : NetworkBehaviour
{
    public HeroType heroType;
    public HeroRank heroRank = HeroRank.Bronze;

    [SerializeField] Material bronzeMaterial;
    [SerializeField] Material silverMaterial;
    [SerializeField] Material goldMaterial;

    private HeroStats currentStats;
    private int xp = 0;
    private int currentEnergy = 0;  //Aktuelle Energie des Helden

    private HeroUIUpdater uiUpdater;
    private EnergyBar energyBar;
    private XPLightManager xpLightManager;

    private bool canSendBomb = false;
    private bool canMakeAction = false;

    private HeroActions heroActions;

    private void Awake()
    {
        InitialUpdateHeroStats();
    }

    //Methode, um XP hinzuzuf�gen und den Rang zu erh�hen
    public void AddXP(int amount)
    {
        xp += amount;
        //Debug.Log(heroType + " hat nun " + xp + " xp.");
        if (xp >= 6)
        {
            Debug.Log(this + " hat ein Level Up!");
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
            //SendBomb();     //Sende Bombe bei Gold
            canSendBomb = true;
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
    }

    private void UpdateHeroStats()
    {
        currentStats = HeroManager.heroData[(heroType, heroRank)];
        energyBar.UpdateEnergieDisplay(currentEnergy, GetMaxEnergy());
        uiUpdater.UpdateHeroDisplay(this);
    }

    public void SetHeroActions(HeroActions actions)
    {
        heroActions = actions;
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

    //Bombe Senden
    public IEnumerator SendBomb()
    {
        Debug.Log(heroType + " hat eine Bombe geschickt!");
        yield return StartCoroutine(heroActions.SendBomb());
    }

    //Methode zur Energieerfassung
    public void AddEnergy(int amount)
    {
        currentEnergy += amount;
        int maxEnergy = GetMaxEnergy();
        if (currentEnergy >= maxEnergy)
        {
            currentEnergy = maxEnergy;
            canMakeAction = true;
        }

        energyBar.UpdateEnergieDisplay(currentEnergy, maxEnergy);
    }

    //Methode um Energie abzuziehen
    public void DecreaseEnergy(int amount)
    {
        currentEnergy -= amount;
        if (currentEnergy < 0)
        {
            currentEnergy = 0;
        }

        energyBar.UpdateEnergieDisplay(currentEnergy, GetMaxEnergy());
    }

    public IEnumerator ActivateAction(HeroType type)
    {
        Debug.Log(type + " f�hrt seine Aktion aus!");

        //Vielleicht mit extra Coroutine
        currentEnergy = 0;
        energyBar.UpdateEnergieDisplay(currentEnergy, GetMaxEnergy());

        yield return StartCoroutine(heroActions.ActivateHeroAction(this));
    }

    public IEnumerator ActivateSecondPriest(HeroType type)
    {
        Debug.Log(type + " f�hrt seine zweite Aktion aus!");
        yield return StartCoroutine(heroActions.PriestSecondAction());
    }



    //Parent setzen
    public void SetHeroParent(GameObject parentTransformRoot, HeroSpawnDummy.PlayerSideKey playerSide, HeroSpawnDummy.HeroSideKey heroSide)
    {
        SetHeroParentServerRpc(parentTransformRoot, playerSide, heroSide);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetHeroParentServerRpc(NetworkObjectReference heroObjectParentRootReference, HeroSpawnDummy.PlayerSideKey playerSide, HeroSpawnDummy.HeroSideKey heroSide)
    {
        if (heroObjectParentRootReference.TryGet(out NetworkObject rootNetworkObject))
        {
            GameObject rootObject = rootNetworkObject.gameObject;
            //Transform heroObjectParentTransform = rootObject.transform.Find(childPath.ToString());
            Transform heroObjectParentTransform = GetTransformFromKeywords(rootObject, playerSide, heroSide);

            this.transform.parent = heroObjectParentTransform;
            this.transform.position = Vector3.zero;
            this.transform.rotation = Quaternion.identity;
        }
        else
        {
            Debug.LogError("Root oder Child konnte nicht gefunden werden.");
        }
    }

    private Transform GetTransformFromKeywords(GameObject rootObject, HeroSpawnDummy.PlayerSideKey playerSide, HeroSpawnDummy.HeroSideKey heroSide)
    {
        string tagToSearchFor = "";

        if (playerSide == HeroSpawnDummy.PlayerSideKey.Player)
        {
            if (heroSide == HeroSpawnDummy.HeroSideKey.Square)
            {
                tagToSearchFor = "PlayerSquareHeroSpawn";
            }
            else
            {
                tagToSearchFor = "PlayerDiamondHeroSpawn";
            }
        }
        else
        {
            if (heroSide == HeroSpawnDummy.HeroSideKey.Square)
            {
                tagToSearchFor = "EnemySquareHeroSpawn";
            }
            else
            {
                tagToSearchFor = "EnemyDiamondHeroSpawn";
            }
        }

        return FindChildTransformWithTag(rootObject, tagToSearchFor);
    }

    private Transform FindChildTransformWithTag(GameObject rootObject, string tag)
    {
        foreach (Transform child in rootObject.transform)
        {
            if (child.CompareTag(tag))
            {
                return child;
            }
        }
        Debug.LogWarning($"Der Tag '{tag}' konnte nicht in {rootObject} gefunden werden!");
        return null;    //Kein Child mit dem Tag gefunden
    }



    //Methoden um die Werte auszulesen
    public int GetCrownDamage()
    {
        return currentStats.crownDamage;
    }

    public int GetBulwarkDamage()
    {
        return currentStats.bulwarkDamage;
    }

    public int GetDelayAdding()
    {
        return currentStats.delayAdding;
    }

    public int GetHealingAdding()
    {
        return currentStats.healingAdding;
    }

    public int GetEnergyAdding()
    {
        return currentStats.energyAdding;
    }

    public string GetHeroName()
    {
        return heroType.ToString();
    }

    public int GetMaxEnergy()
    {
        return currentStats.energyToAct;
    }

    public void SetEnergyBar(EnergyBar bar)
    {
        energyBar = bar;
    }

    public void SetXPLightManager(XPLightManager light)
    {
        xpLightManager = light;
    }

    public void SetUIUpdater(HeroUIUpdater updater)
    {
        uiUpdater = updater;
    }

    public bool GetCanSendBomb()
    {
        bool sendBomb = canSendBomb;
        canSendBomb = false;
        return sendBomb;
    }

    public bool GetCanMakeAction()
    {
        bool makeAction = canMakeAction;
        canMakeAction = false;
        return makeAction;
    }

    public bool PriestChecksAction()
    {
        return canMakeAction;
    }

    public void SetCanMakeAction(bool stateOfAction)
    {
        canMakeAction = stateOfAction;
    }

    public HeroType GetHeroType()
    {
        return heroType;
    }

    public int GetCurrentEnergy()
    {
        return currentEnergy;
    }

    public bool GetPriestBoosted()
    {
        return heroActions.GetPriestBoosted();
    }
}
