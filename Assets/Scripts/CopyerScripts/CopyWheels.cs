using System.Collections.Generic;
using UnityEngine;

public class CopyWheels : ManagerCopiesHandler<WheelManager>
{
    [SerializeField] private Transform[] wheels;                    //Eine Liste mit allen Rädern
    [SerializeField] private Material[] materialOfSymbols;          //Eine Liste mit allen Radvariationen (anhand von Materialien)
    [SerializeField] private ClampActivator[] clampedActivators;     //Eine Liste mit allen Clampaktivatoren
    private readonly int symbolMaterialIndex = 2;                   //Index des Materials

    //Definiere ein Dictionary für jedes Rad und dessen Symbolreihenfolge
    private Dictionary<int, Symbol[]> wheelSymbols = new Dictionary<int, Symbol[]>
    {
        //Räder 1 - 4
        { 0, new Symbol[] { Symbol.Square, Symbol.Hammer, Symbol.DiamondDiamondPlus, Symbol.Hammer, Symbol.Diamond, Symbol.SquarePlus, Symbol.Square, Symbol.Diamond } },
        { 1, new Symbol[] { Symbol.SquarePlus, Symbol.HammerHammer, Symbol.DiamondDiamond, Symbol.Hammer, Symbol.Square, Symbol.DiamondPlus, Symbol.SquareSquare, Symbol.Diamond } },
        { 2, new Symbol[] { Symbol.SquarePlus, Symbol.HammerHammer, Symbol.SquareSquare, Symbol.HammerHammer, Symbol.Diamond, Symbol.Square, Symbol.DiamondPlus, Symbol.Diamond } },
        { 3, new Symbol[] { Symbol.Square, Symbol.HammerHammer, Symbol.DiamondPlus, Symbol.Square, Symbol.HammerHammer, Symbol.Diamond, Symbol.SquarePlus, Symbol.Diamond } },

        //Alle 5. Räder
        //Copper
        { 6, new Symbol[] { Symbol.Square, Symbol.Empty, Symbol.Diamond, Symbol.Square, Symbol.Empty, Symbol.Empty, Symbol.Hammer, Symbol.Diamond } },
        //Bronze
        { 5, new Symbol[] { Symbol.Square, Symbol.HammerHammer, Symbol.Diamond, Symbol.Square, Symbol.Empty, Symbol.Empty, Symbol.Hammer, Symbol.Diamond } },
        //Silver
        { 4, new Symbol[] { Symbol.Square, Symbol.HammerHammer, Symbol.Diamond, Symbol.SquareSquarePlus, Symbol.Diamond, Symbol.Empty, Symbol.Hammer, Symbol.Diamond } },
        //Gold
        { 7, new Symbol[] { Symbol.Square, Symbol.HammerHammer, Symbol.Diamond, Symbol.SquareSquarePlus, Symbol.Diamond, Symbol.Square, Symbol.Hammer, Symbol.DiamondDiamondPlus } },
        //Diamond
        { 8, new Symbol[] { Symbol.Square, Symbol.HammerHammer, Symbol.Diamond, Symbol.SquareSquarePlus, Symbol.DiamondDiamond, Symbol.SquareSquare, Symbol.HammerHammer, Symbol.DiamondDiamondPlus } },
        //Platinum
        { 9, new Symbol[] { Symbol.Square, Symbol.HammerHammer, Symbol.Diamond, Symbol.SquareSquarePlus, Symbol.DiamondDiamondPlus, Symbol.SquareSquarePlus, Symbol.HammerHammerHammer, Symbol.DiamondDiamondPlus } }
    };

    private void Start()
    {
        //Weisen den Shadern den jeweiligen Rädern zu, indem wir die Wheelspin-Objekte verwenden (nur Räder 1 - 4)
        for (int i = 0; i < wheels.Length; i++)
        {
            MeshRenderer renderer = wheels[i].GetComponent<MeshRenderer>();
            if (renderer != null && renderer.materials.Length > symbolMaterialIndex)
            {
                //Holen das aktuelle Material-Array des Rads
                Material[] materials = renderer.materials;

                //ändern nur das Material an dem angegebenen Index (für die Symbole)
                materials[symbolMaterialIndex] = materialOfSymbols[i];

                //Setzen das Material-Array zurück
                renderer.materials = materials;
            }
        }

        //Rad 5
        MeshRenderer fifthRenderer = wheels[4].GetComponent<MeshRenderer>();
        if (fifthRenderer != null && fifthRenderer.materials.Length > symbolMaterialIndex)
        {
            //Holen das aktuelle Material-Array des Rads
            Material[] materials = fifthRenderer.materials;

            //ändern nur das Material an dem angegebenen Index (für die Symbole)
            materials[symbolMaterialIndex] = materialOfSymbols[6];

            //Setzen das Material-Array zurück
            fifthRenderer.materials = materials;
        }
    }

    protected override void SetEvents()
    {
        //Events
        originalManager.OnWheelsHaveStopped += OriginalWheelManager_OnWheelsHaveStopped;
        originalManager.OnUnlockClamps += OriginalWheelManager_OnUnlockClamps;
    }

    private void OriginalWheelManager_OnWheelsHaveStopped(int rotWheel1, int rotWheel2, int rotWheel3, int rotWheel4, int rotWheel5)
    {
        //Array mit den Rotationswerten erstellen
        int[] rotations = { rotWheel1, rotWheel2, rotWheel3, rotWheel4, rotWheel5 };

        //Sicherstellen, dass das Array die richtige Größe hat
        if (wheels == null || wheels.Length != rotations.Length)
        {
            Debug.LogError("Das Wheels-Array hat nicht die richtige Größe oder ist null.");
            return;
        }

        //x-Rotationen der Räder setzen
        for (int i = 0; i < wheels.Length; i++)
        {
            if (wheels[i] != null)  //Sicherstellen, dass das Transform-Objekt nicht null ist
            {
                Vector3 newRotation = new Vector3(0f, 0f, 0f);
                newRotation.x = 45f + (rotations[i] * 45f);   //Nur x-Achse setzen
                //Quaternion targetRotation = Quaternion.Euler(45f + (rotations[i] * 45f), newRotation.y, newRotation.z);
                wheels[i].localEulerAngles = newRotation;
            }
            else
            {
                Debug.LogWarning($"Wheel[{i}] ist null. Rotation wird übersprungen!");
            }
        }

        ChangeClampActivation(true);
    }

    private void OriginalWheelManager_OnUnlockClamps(object sender, System.EventArgs e)
    {
        ChangeClampActivation(false);
    }

    private void ChangeClampActivation(bool status)
    {
        foreach (var clamp in clampedActivators)
        {
            clamp.ToggleClamps(status);
        }
    }
}
