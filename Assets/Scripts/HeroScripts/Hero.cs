using UnityEngine;
using Unity.Netcode;

public class Hero : NetworkBehaviour
{
    public HeroType heroType;
    public HeroRank heroRank = HeroRank.Bronze;

    [SerializeField] Material bronzeMaterial;
    [SerializeField] Material silverMaterial;
    [SerializeField] Material goldMaterial;

    private HeroStats currentStats;
    private NetworkVariable<int> xp = new NetworkVariable<int>(
        0,  //Startwert
        NetworkVariableReadPermission.Everyone,     //Clients dürfen den Wert lesen
        NetworkVariableWritePermission.Server       //Nur der Server darf den Wert schreiben
        );
    private NetworkVariable<int> currentEnergy = new NetworkVariable<int>(  //Aktuelle Energie des Helden
        0,  //Startwert
        NetworkVariableReadPermission.Everyone,     //Clients dürfen den Wert lesen
        NetworkVariableWritePermission.Server       //Nur der Server darf den Wert schreiben
        );

    private HeroUIUpdater uiUpdater;
    private EnergyBar energyBar;
    private XPLightManager xpLightManager;

    private bool canSendBomb = false;
    private bool canMakeAction = false;

    private HeroActionsActivator heroActionsActivator;

    private void Awake()
    {
        InitialUpdateHeroStats();
    }

    //Methode, um XP hinzuzufügen und den Rang zu erhöhen
    public void AddXP(int amount)
    {
        xp.Value += amount;
        //Debug.Log(heroType + " hat nun " + xp + " xp.");
        if (xp.Value >= 6)
        {
            Debug.Log(this + " hat ein Level Up!");
            RankUp();
        }
        xpLightManager.UpdateXPLamps(xp.Value);
    }

    //Methode für den Rangaufstieg
    private void RankUp()
    {
        xp.Value = 0;
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
        energyBar.UpdateEnergieDisplay(currentEnergy.Value, GetMaxEnergy());
        uiUpdater.UpdateHeroDisplay(this);
    }

    public void SetHeroActions(HeroActionsActivator actionsActivator)
    {
        heroActionsActivator = actionsActivator;
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
                    Debug.LogError("Heldenrang " + heroRank + "ist nicht verfügbar!");
                    break;
            }
        }
    }

    //Bombe Senden
    public float SendBomb()
    {
        Debug.Log(heroType + " hat eine Bombe geschickt!");
        return heroActionsActivator.SendBomb();
    }

    //Methode zur Energieerfassung
    public void AddEnergy(int amount)
    {
        currentEnergy.Value += amount;
        int maxEnergy = GetMaxEnergy();
        if (currentEnergy.Value >= maxEnergy)
        {
            currentEnergy.Value = maxEnergy;
            canMakeAction = true;
        }

        energyBar.UpdateEnergieDisplay(currentEnergy.Value, maxEnergy);
    }

    //Methode um Energie abzuziehen
    public void DecreaseEnergy(int amount)
    {
        currentEnergy.Value -= amount;
        if (currentEnergy.Value < 0)
        {
            currentEnergy.Value = 0;
        }

        energyBar.UpdateEnergieDisplay(currentEnergy.Value, GetMaxEnergy());
    }

    public float ActivateAction(HeroType type)
    {
        Debug.Log(type + " führt seine Aktion aus!");

        //Vielleicht mit extra Coroutine
        currentEnergy.Value = 0;
        energyBar.UpdateEnergieDisplay(currentEnergy.Value, GetMaxEnergy());

        return heroActionsActivator.ActivateHeroAction(this);
    }

    public float ActivateSecondPriest(HeroType type)
    {
        Debug.Log(type + " führt seine zweite Aktion aus!");
        return heroActionsActivator.PriestSecondAction();
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
        return currentEnergy.Value;
    }

    public bool GetPriestBoosted()
    {
        return heroActionsActivator.GetPriestBoosted();
    }
}
