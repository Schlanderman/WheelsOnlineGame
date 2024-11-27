using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeroManager
{
    //Dictionary, um Helden und Rang mit spezifischen Werten zu verknüpfen
    public static Dictionary<(HeroType, HeroRank), HeroStats> heroData = new Dictionary<(HeroType, HeroRank), HeroStats>()
    {
        { (HeroType.Warrior, HeroRank.Bronze), new HeroStats(3, 3, 3, 0, 0, 0) },
        { (HeroType.Warrior, HeroRank.Silver), new HeroStats(3, 5, 5, 0, 0, 0) },
        { (HeroType.Warrior, HeroRank.Gold),   new HeroStats(3, 7, 5, 0, 0, 0) },
        { (HeroType.Mage, HeroRank.Bronze), new HeroStats(5, 2, 2, 0, 0, 0) },
        { (HeroType.Mage, HeroRank.Silver), new HeroStats(4, 3, 3, 0, 0, 0) },
        { (HeroType.Mage, HeroRank.Gold),   new HeroStats(4, 3, 5, 0, 0, 0) },
        { (HeroType.Archer, HeroRank.Bronze), new HeroStats(4, 3, 1, 0, 0, 0) },
        { (HeroType.Archer, HeroRank.Silver), new HeroStats(3, 4, 2, 0, 0, 0) },
        { (HeroType.Archer, HeroRank.Gold),   new HeroStats(3, 6, 3, 0, 0, 0) },
        { (HeroType.Engineer, HeroRank.Bronze), new HeroStats(4, 1, 3, 0, 0, 0) },
        { (HeroType.Engineer, HeroRank.Silver), new HeroStats(4, 2, 5, 0, 0, 0) },
        { (HeroType.Engineer, HeroRank.Gold),   new HeroStats(3, 4, 5, 0, 0, 0) },
        { (HeroType.Assassin, HeroRank.Bronze), new HeroStats(3, 1, 0, 1, 0, 0) },
        { (HeroType.Assassin, HeroRank.Silver), new HeroStats(3, 2, 0, 1, 0, 0) },
        { (HeroType.Assassin, HeroRank.Gold),   new HeroStats(3, 2, 0, 2, 0, 0) },
        { (HeroType.Priest, HeroRank.Bronze), new HeroStats(4, 0, 0, 0, 1, 2) },
        { (HeroType.Priest, HeroRank.Silver), new HeroStats(3, 0, 0, 0, 2, 2) },
        { (HeroType.Priest, HeroRank.Gold),   new HeroStats(3, 0, 0, 0, 2, 3) }
    };
}
