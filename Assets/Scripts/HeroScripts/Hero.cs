using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections;

public class Hero : NetworkBehaviour
{
    public HeroType heroType;
    private NetworkVariable<HeroRank> heroRank = new NetworkVariable<HeroRank>(
        HeroRank.Bronze,    //Startwert
        NetworkVariableReadPermission.Everyone,     //Clients dürfen den Wert lesen
        NetworkVariableWritePermission.Server       //Nur der Server darf den Wert schreiben
        );

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
    private HeroActionsLengthManager heroActionsLengthManager;

    private Hero copyHero;

    private void Awake()
    {
        InitialUpdateHeroStats();
    }

    private void Start()
    {
        heroRank.OnValueChanged += HeroRankValueChanged;
        xp.OnValueChanged += XPValueChanged;
        currentEnergy.OnValueChanged += EnergyValueChanged;
    }

    public override void OnDestroy()
    {
        heroRank.OnValueChanged -= HeroRankValueChanged;
        xp.OnValueChanged -= XPValueChanged;
        currentEnergy.OnValueChanged -= EnergyValueChanged;
    }

    private void InitialUpdateHeroStats()
    {
        currentStats = HeroManager.heroData[(heroType, heroRank.Value)];
    }

    //Methode, um XP hinzuzufügen und den Rang zu erhöhen
    public void AddXP(int amount)
    {
        xp.Value += amount;
        //Debug.Log(heroType + " hat nun " + xp + " xp.");
        if (xp.Value >= 6)
        {
            RankUp();
        }
    }

    private void XPValueChanged(int previousValue, int newValue)
    {
        xpLightManager.UpdateXPLamps(xp.Value);
    }

    //Methode für den Rangaufstieg
    private void RankUp()
    {
        xp.Value = 0;
        if (heroRank.Value == HeroRank.Gold)
        {
            //SendBomb();     //Sende Bombe bei Gold
            canSendBomb = true;
        }
        else
        {
            heroRank.Value++;
            copyHero.SetHeroRank(heroRank.Value);
        }
    }

    private void HeroRankValueChanged(HeroRank previousValue, HeroRank newValue)
    {
        //Nur ausführen, wenn das hier ein OriginalHero ist (also kein copyHero vorhanden)
        if (copyHero != null) { UpdateHeroStats(); }
        UpdateHeroAppearence();
    }

    private void UpdateHeroStats()
    {
        currentStats = HeroManager.heroData[(heroType, heroRank.Value)];
        uiUpdater.UpdateHeroDisplay(this);
    }

    //Funktion zum Ändern des Aussehens
    private void UpdateHeroAppearence()
    {
        Debug.Log($"{this} steigt einen Rang auf, auf {heroRank.Value}");
        MeshRenderer renderer = this.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            switch (heroRank.Value)
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
                    Debug.LogError("Heldenrang " + heroRank.Value + "ist nicht verfügbar!");
                    break;
            }
        }
    }

    //Energy hat sich geändert, also EnergyBar bewegen
    private void EnergyValueChanged(int previousValue, int newValue)
    {
        //Debug.Log($"EnergyBar: {energyBar}, Values: {previousValue} {newValue}, MaxEnergy: {GetMaxEnergy()}");
        energyBar.UpdateEnergieDisplay(currentEnergy.Value, GetMaxEnergy());
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
    }

    //Methode um Energie abzuziehen
    public void DecreaseEnergy(int amount)
    {
        currentEnergy.Value -= amount;
        if (currentEnergy.Value < 0)
        {
            currentEnergy.Value = 0;
        }
    }

    //Animationen per Rpc senden
    [Rpc(SendTo.Everyone)]
    public void SendBombRpc()
    {
        Debug.Log(heroType + " hat eine Bombe geschickt!");
        heroActionsActivator.SendBomb();
    }

    [Rpc(SendTo.Everyone)]
    public void ActivateActionRpc()
    {
        Debug.Log(heroType + " führt seine Aktion aus!");

        //Vielleicht mit extra Coroutine
        //if (IsServer) { currentEnergy.Value = 0; }

        StartCoroutine(heroActionsActivator.ActivateHeroAction(this));
    }

    [Rpc(SendTo.Everyone)]
    public void ActivateSecondPriestRpc()
    {
        Debug.Log(heroType + " führt seine zweite Aktion aus!");
        StartCoroutine(heroActionsActivator.PriestSecondAction());
    }


    //Länge der Animation an TurnManager übergeben
    public float GetAnimationLength()
    {
        return heroActionsLengthManager.GetAnimationLength(this);
    }

    public float GetBombAnimationLength()
    {
        return heroActionsLengthManager.GetBombAnimationLength();
    }

    public float GetPriestSecondAnimationLength()
    {
        return heroActionsLengthManager.GetPriestSecondAnimationLength();
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

    public HeroRank GetHeroRank()
    {
        return heroRank.Value;
    }



    public void SetEnergyBar(EnergyBar bar)
    {
        energyBar = bar;
        EnergyValueChanged(0, 0);
    }

    public void SetXPLightManager(XPLightManager light)
    {
        xpLightManager = light;
    }

    public void SetUIUpdater(HeroUIUpdater updater)
    {
        uiUpdater = updater;
    }

    public void SetHeroActions(HeroActionsActivator actionsActivator, HeroActionsLengthManager actionsLengthManager)
    {
        heroActionsActivator = actionsActivator;
        heroActionsLengthManager = actionsLengthManager;
    }

    public void SetCopyHero(Hero copy)
    {
        copyHero = copy;
    }

    public void SetHeroRank(HeroRank newRank)
    {
        heroRank.Value = newRank;
    }

    public void ResetEnergyBar()
    {
        currentEnergy.Value = 0;
    }
}
