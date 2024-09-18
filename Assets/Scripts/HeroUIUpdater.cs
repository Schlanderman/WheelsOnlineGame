using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroUIUpdater : MonoBehaviour
{
    //Referenzen zu den beiden Blöcken und den Rädern
    [SerializeField] private GameObject block1Renderer;
    [SerializeField] private GameObject block2Renderer;
    [SerializeField] private Transform wheel1;
    [SerializeField] private Transform wheel2;

    //Materialien für die verschiedenen Aktionen
    [SerializeField] private Material crownDamageMaterial;
    [SerializeField] private Material bulwarkDamageMaterial;
    [SerializeField] private Material delayMaterial;
    [SerializeField] private Material healingMaterial;
    [SerializeField] private Material energyMaterial;

    //Methode zum aktualisieren der Blöcke und Räder
    public void UpdateHeroDisplay(Hero hero)
    {
        MeshRenderer renderer1 = block1Renderer.GetComponent<MeshRenderer>();
        MeshRenderer renderer2 = block2Renderer.GetComponent<MeshRenderer>();

        int firstStat = 0;
        int secondStat = 0;

        if (renderer1 != null && renderer2 != null)
        {
            //Holen das aktuelle Material-Array des Rads
            Material[] materials1 = renderer1.materials;
            Material[] materials2 = renderer2.materials;

            switch (hero.GetHeroName())
            {
                case "Warrior":
                    materials1[1] = crownDamageMaterial;
                    materials2[1] = bulwarkDamageMaterial;
                    firstStat = hero.GetCrownDamage();
                    secondStat = hero.GetBulwarkDamage();
                    break;

                case "Mage":
                    materials1[1] = crownDamageMaterial;
                    materials2[1] = bulwarkDamageMaterial;
                    firstStat = hero.GetCrownDamage();
                    secondStat = hero.GetBulwarkDamage();
                    break;

                case "Archer":
                    materials1[1] = crownDamageMaterial;
                    materials2[1] = bulwarkDamageMaterial;
                    firstStat = hero.GetCrownDamage();
                    secondStat = hero.GetBulwarkDamage();
                    break;

                case "Engineer":
                    materials1[1] = crownDamageMaterial;
                    materials2[1] = bulwarkDamageMaterial;
                    firstStat = hero.GetCrownDamage();
                    secondStat = hero.GetBulwarkDamage();
                    break;

                case "Assassin":
                    materials1[1] = delayMaterial;
                    materials2[1] = crownDamageMaterial;
                    firstStat = hero.GetDelayAdding();
                    secondStat = hero.GetCrownDamage();
                    break;

                case "Priest":
                    materials1[1] = healingMaterial;
                    materials2[1] = energyMaterial;
                    firstStat = hero.GetHealingAdding();
                    secondStat = hero.GetEnergyAdding();
                    break;

                default:
                    Debug.LogWarning("Der Held: " + hero.GetHeroName() + " ist nicht bekannt!");
                    break;
            }

            //Setzen das Material-Array zurück
            renderer1.materials = materials1;
            renderer2.materials = materials2;
        }

        SetWheelValue(firstStat, secondStat);
    }

    //Hilfsmethode zum setzen der Werte auf dem Rad
    private void SetWheelValue(int action1Value, int action2Value)
    {
        //Debug.Log("ActionValue ist: " + actionValue);
        //Angenommen, das Rad hat 10 Seiten, die jeweils 36 Grad betragen
        float anglePerValue = -36f;
        float target1Rotation = action1Value * anglePerValue;
        float target2Rotation = action2Value * anglePerValue;
        //Debug.Log("Winkel 1 ist: " + target1Rotation + "; Winkel 2 ist: " + target2Rotation);

        wheel1.localRotation = Quaternion.Euler(target1Rotation, wheel1.localRotation.y, wheel1.localRotation.z);
        wheel2.localRotation = Quaternion.Euler(target2Rotation, wheel2.localRotation.y, wheel2.localRotation.z);
    }
}
