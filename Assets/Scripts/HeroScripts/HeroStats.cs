using System.Collections.Generic;

[System.Serializable]
public class HeroStats
{
    public int energyToAct;
    public int crownDamage;
    public int bulwarkDamage;
    public int delayAdding;
    public int healingAdding;
    public int energyAdding;

    public HeroStats(int energyToAct, int crownDamage, int bulwarkDamage, int delayAdding, int healingAdding, int energyAdding)
    {
        this.energyToAct = energyToAct;
        this.crownDamage = crownDamage;
        this.bulwarkDamage = bulwarkDamage;
        this.delayAdding = delayAdding;
        this.healingAdding = healingAdding;
        this.energyAdding = energyAdding;
    }
}







//public class HeroStats
//{
//    public int baseCrownDamage;
//    public int baseBulwarkDamage;
//    public int baseDelayAdding;
//    public int baseEnergyAdding;
//    public int baseHealingGive;
//    public int maxEnergyAct;
//    public int[] rankCrownDamage;   //Array für Kronenschaden auf verschiedenen Rängen
//    public int[] rankBulwarkDamage; //Array für Bulwarkschaden auf verschiedenen Rängen
//    public int[] rankDelayAdding;   //Array für Delay auf verschiedenen Rängen
//    public int[] rankEnergyAdding;  //Array für Engergiezufuhr auf verschiedenen Rängen
//    public int[] rankHealing;       //Array für Healing auf verschiedenen Rängen
//    public int[] rankMaxEnergy;     //Array für Energie auf verschiedenen Rängen

//    public HeroStats(int baseCrown, int baseBulwark, int baseDelay, int baseEnergy, int baseHealing, int maxEnergyAct, int[] crownDamage, int[] bulwarkDamage, int[] delayAdding, int[] energyAdding, int[] healingGive, int[] energyAct)
//    {
//        this.baseCrownDamage = baseCrown;
//        this.baseBulwarkDamage = baseBulwark;
//        this.baseDelayAdding = baseDelay;
//        this.baseEnergyAdding = baseEnergy;
//        this.baseHealingGive = baseHealing;
//        this.maxEnergyAct = maxEnergyAct;
//        this.rankCrownDamage = crownDamage;
//        this.rankBulwarkDamage = bulwarkDamage;
//        this.rankDelayAdding = delayAdding;
//        this.rankEnergyAdding = energyAdding;
//        this.rankHealing = healingGive;
//        this.rankMaxEnergy = energyAct;
//    }
//}


//Das hier gehört in HeroManager

//Dictionary<string, HeroStats> heroStatsDictionary = new Dictionary<string, HeroStats>();
//private void Start()
//{
//    //Initialisierung der Heldenwerte
//    heroStatsDictionary.Add("Warrior", new HeroStats(3, 3, 0, 0, 0, 3, new int[] { 3, 5, 7 }, new int[] { 3, 5, 5 }, new int[] { 0, 0, 0 }, new int[] { 0, 0, 0 }, new int[] { 0, 0, 0 }, new int[] { 3, 3, 3 }));
//    heroStatsDictionary.Add("Mage", new HeroStats(2, 2, 0, 0, 0, 5, new int[] { 2, 3, 3 }, new int[] { 2, 3, 5 }, new int[] { 0, 0, 0 }, new int[] { 0, 0, 0 }, new int[] { 0, 0, 0 }, new int[] { 5, 4, 4 }));
//    heroStatsDictionary.Add("Archer", new HeroStats(3, 1, 0, 0, 0, 4, new int[] { 3, 4, 6 }, new int[] { 1, 2, 3 }, new int[] { 0, 0, 0 }, new int[] { 0, 0, 0 }, new int[] { 0, 0, 0 }, new int[] { 4, 3, 3 }));
//    heroStatsDictionary.Add("Engineer", new HeroStats(1, 3, 0, 0, 0, 4, new int[] { 1, 2, 4 }, new int[] { 3, 5, 5 }, new int[] { 0, 0, 0 }, new int[] { 0, 0, 0 }, new int[] { 0, 0, 0 }, new int[] { 4, 4, 3 }));
//    heroStatsDictionary.Add("Assassin", new HeroStats(1, 0, 1, 0, 0, 3, new int[] { 1, 2, 2 }, new int[] { 0, 0, 0 }, new int[] { 1, 1, 2 }, new int[] { 0, 0, 0 }, new int[] { 0, 0, 0 }, new int[] { 3, 3, 3 }));
//    heroStatsDictionary.Add("Priest", new HeroStats(0, 0, 0, 2, 1, 4, new int[] { 0, 0, 0 }, new int[] { 0, 0, 0 }, new int[] { 0, 0, 0 }, new int[] { 2, 2, 3 }, new int[] { 1, 2, 2 }, new int[] { 4, 3, 3 }));
//}

////Methode zum Abrufen von Stats
//public HeroStats GetHeroStats(string heroName, int rank)
//{
//    if (heroStatsDictionary.TryGetValue(heroName, out HeroStats stats))
//    {
//        return new HeroStats(
//            stats.baseCrownDamage,
//            stats.baseBulwarkDamage,
//            stats.baseDelayAdding,
//            stats.baseEnergyAdding,
//            stats.baseHealingGive,
//            stats.maxEnergyAct,
//            stats.rankCrownDamage,
//            stats.rankBulwarkDamage,
//            stats.rankDelayAdding,
//            stats.rankHealing,
//            stats.rankEnergyAdding,
//            stats.rankMaxEnergy
//            );
//    }
//    else
//    {
//        Debug.LogError("Hero not found!: " + heroName);
//        return null;
//    }
//}