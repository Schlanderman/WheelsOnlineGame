using UnityEngine;

public class CopyManager : MonoBehaviour
{
    //private EnemyScript enemyScript;    //Offline only

    //Sachen, die ich vom CopyObjekt brauche:
    //Wheels 1 - 5
    [SerializeField] private CopyWheels copyWheels;
    //Square Hero
    [SerializeField] private CopyHeroFigures copyHeroFigures;
    //XP Lights Square
    [SerializeField] private CopyXPLights copyXPLightsSquare;
    //XP Lights Diamond
    [SerializeField] private CopyXPLights copyXPLightsDiamond;
    //XP Particles
    [SerializeField] private CopyXPParticles copyXPParticles;
    //Hero UI Square
    [SerializeField] private CopyHeroUI copyHeroUISquare;
    //Hero UI Diamond
    [SerializeField] private CopyHeroUI copyHeroUIDiamond;
    //Bulwark
    [SerializeField] private CopyBulwark copyBulwark;
    //Crown
    [SerializeField] private CopyCrown copyCrown;
    //Action Rods 1 - 4
    [SerializeField] private CopyActionRods copyActionRods;
    //EnergyBar Square
    [SerializeField] private CopyEnergyBar copyEnergyBarSquare;
    //EnergyBar Diamond
    [SerializeField] private CopyEnergyBar CopyEnergyBarDiamond;


    //Scripte, die ich vom Original bekomme
    private WheelManager originalWheels;
    private HeroSelectionRotator originalHeroRotator;
    private XPLightManager originalXPLightsSquare;
    private XPLightManager originalXPLightsDiamond;
    private ParticleManager originalXPParticles;
    private HeroUIUpdater originalHeroUISquare;
    private HeroUIUpdater originalHeroUIDiamond;
    private BulwarkMover originalBulwark;
    private CrownManager originalCrown;
    private ActionRodAnimManager originalActionRods;
    private EnergyBar originalEnergyBarSquare;
    private EnergyBar originalEnergyBarDiamond;

    public void SetManagersFromOriginal(GameObject originalPlayerObject)
    {
        //Manager, die nur einfach vorhanden sind
        originalWheels = originalPlayerObject.GetComponentInChildren<WheelManager>();
        originalHeroRotator = originalPlayerObject.GetComponentInChildren<HeroSelectionRotator>();
        originalXPParticles = originalPlayerObject.GetComponentInChildren<ParticleManager>();
        originalBulwark = originalPlayerObject.GetComponentInChildren<BulwarkMover>();
        originalCrown = originalPlayerObject.GetComponentInChildren<CrownManager>();
        originalActionRods = originalPlayerObject.GetComponentInChildren<ActionRodAnimManager>();

        //Manager, die mehrfach vorhanden sind
        var xpLights = originalPlayerObject.GetComponentsInChildren<XPLightManager>();
        if (xpLights.Length >= 2)
        {
            originalXPLightsSquare = xpLights[0];
            originalXPLightsDiamond = xpLights[1];
        }

        var heroUIUpdaters = originalPlayerObject.GetComponentsInChildren<HeroUIUpdater>();
        if (heroUIUpdaters.Length >= 2)
        {
            originalHeroUISquare = heroUIUpdaters[0];
            originalHeroUIDiamond = heroUIUpdaters[1];
        }

        var energyBars = originalPlayerObject.GetComponentsInChildren<EnergyBar>();
        if (energyBars.Length >= 2)
        {
            originalEnergyBarSquare = energyBars[0];
            originalEnergyBarDiamond = energyBars[1];
        }

        InitializeTheManagersInGameObjects();
    }

    private void InitializeTheManagersInGameObjects()
    {
        copyWheels.SetOriginalManager(originalWheels);
        copyHeroFigures.SetOriginalManager(originalHeroRotator);
        copyHeroFigures.SetActionRodManager(originalActionRods);
        copyXPLightsSquare.SetOriginalManager(originalXPLightsSquare);
        copyXPLightsDiamond.SetOriginalManager(originalXPLightsDiamond);
        copyXPParticles.SetOriginalManager(originalXPParticles);
        copyHeroUISquare.SetOriginalManager(originalHeroUISquare);
        copyHeroUIDiamond.SetOriginalManager(originalHeroUIDiamond);
        copyBulwark.SetOriginalManager(originalBulwark);
        copyCrown.SetOriginalManager(originalCrown);
        copyActionRods.SetOriginalManager(originalActionRods);
        copyEnergyBarSquare.SetOriginalManager(originalEnergyBarSquare);
        CopyEnergyBarDiamond.SetOriginalManager(originalEnergyBarDiamond);
    }
}
