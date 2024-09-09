using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelManager : MonoBehaviour
{
    [SerializeField]
    private WheelSpin[] wheels;     //Eine Liste mit allen R�dern
    [SerializeField]
    private Material[] materialOfSymbols;   //Eine Liste mit allen Radvariationen (anhand von Materialien)
    private readonly int symbolMaterialIndex = 2;    //Index des Materials

    [SerializeField] private HeroSelectionRotator heroSelectionRotator;

    private readonly int maxSpins = 3;  //Maximale Anzahl an Spins
    private int spinCount = 0;          //Z�hler f�r die Anzahl der Spins
    public int stoppedWheels = 0;       //Anzahl der R�der, die sich nicht mehr drehen
    private bool canSpin = true;
    private bool firstSpin = true;

    [SerializeField]
    private Image[] spinLamps;      //Array von UI-Images f�r die Lampen
    [SerializeField]
    private Color inactiveColor;
    [SerializeField]
    private Color activeColor;

    [SerializeField] private GameObject squareHeroSpawn;    //Spawn des Square-Helden
    [SerializeField] private GameObject diamondHeroSpawn;   //Spawn des Diamond-Helden
    [SerializeField] private BulwarkMover bulwarkMover;

    private Hero squareHero;    // Held, der Energie von Square-Symbolen erh�lt
    private Hero diamondHero;   // Held, der Energie von Diamond-Symbolen erh�lt

    //Definiere ein Dictionary f�r jedes Rad und dessen Symbolreihenfolge
    private Dictionary<int, Symbol[]> wheelSymbols = new Dictionary<int, Symbol[]>
    {
        //R�der 1 - 4
        { 0, new Symbol[] { Symbol.Square, Symbol.Hammer, Symbol.DiamondDiamondPlus, Symbol.Hammer, Symbol.Diamond, Symbol.SquarePlus, Symbol.Square, Symbol.Diamond } },
        { 1, new Symbol[] { Symbol.SquarePlus, Symbol.HammerHammer, Symbol.DiamondDiamond, Symbol.Hammer, Symbol.Square, Symbol.DiamondPlus, Symbol.SquareSquare, Symbol.Diamond } },
        { 2, new Symbol[] { Symbol.SquarePlus, Symbol.HammerHammer, Symbol.SquareSquare, Symbol.HammerHammer, Symbol.Diamond, Symbol.Square, Symbol.DiamondPlus, Symbol.Diamond } },
        { 3, new Symbol[] { Symbol.Square, Symbol.HammerHammer, Symbol.DiamondPlus, Symbol.Square, Symbol.HammerHammer, Symbol.Diamond, Symbol.SquarePlus, Symbol.Diamond } },

        //Alle 5. R�der
        //Copper
        { 4, new Symbol[] { Symbol.Square, Symbol.Empty, Symbol.Diamond, Symbol.Square, Symbol.Empty, Symbol.Empty, Symbol.Hammer, Symbol.Diamond } },
        //Bronze
        { 5, new Symbol[] { Symbol.Square, Symbol.HammerHammer, Symbol.Diamond, Symbol.Square, Symbol.Empty, Symbol.Empty, Symbol.Hammer, Symbol.Diamond } },
        //Silver
        { 6, new Symbol[] { Symbol.Square, Symbol.HammerHammer, Symbol.Diamond, Symbol.SquareSquarePlus, Symbol.Diamond, Symbol.Empty, Symbol.Hammer, Symbol.Diamond } },
        //Gold
        { 7, new Symbol[] { Symbol.Square, Symbol.HammerHammer, Symbol.Diamond, Symbol.SquareSquarePlus, Symbol.Diamond, Symbol.Square, Symbol.Hammer, Symbol.DiamondDiamondPlus } },
        //Diamond
        { 8, new Symbol[] { Symbol.Square, Symbol.HammerHammer, Symbol.Diamond, Symbol.SquareSquarePlus, Symbol.DiamondDiamond, Symbol.SquareSquare, Symbol.HammerHammer, Symbol.DiamondDiamondPlus } },
        //Platinum
        { 9, new Symbol[] { Symbol.Square, Symbol.HammerHammer, Symbol.Diamond, Symbol.SquareSquarePlus, Symbol.DiamondDiamondPlus, Symbol.SquareSquarePlus, Symbol.HammerHammerHammer, Symbol.DiamondDiamondPlus } }
    };

    //Ersatz, falls das umdrehen doch nichts bringt
    //private Dictionary<int, Symbol[]> wheelSymbols = new Dictionary<int, Symbol[]>
    //{
    //    //R�der 1 - 4
    //    { 0, new Symbol[] { Symbol.Square, Symbol.Diamond, Symbol.Square, Symbol.SquarePlus, Symbol.Diamond, Symbol.Hammer, Symbol.DiamondDiamondPlus, Symbol.Hammer } },
    //    { 1, new Symbol[] { Symbol.SquarePlus, Symbol.Diamond, Symbol.SquareSquare, Symbol.DiamondPlus, Symbol.Square, Symbol.Hammer, Symbol.DiamondDiamond, Symbol.HammerHammer } },
    //    { 2, new Symbol[] { Symbol.SquarePlus, Symbol.Diamond, Symbol.DiamondPlus, Symbol.Square, Symbol.Diamond, Symbol.HammerHammer, Symbol.SquareSquare, Symbol.HammerHammer } },
    //    { 3, new Symbol[] { Symbol.Square, Symbol.Diamond, Symbol.SquarePlus, Symbol.Diamond, Symbol.HammerHammer, Symbol.Square, Symbol.DiamondPlus, Symbol.HammerHammer } },

    //    //Alle 5. R�der
    //    //Copper
    //    { 4, new Symbol[] { Symbol.Square, Symbol.Diamond, Symbol.Hammer, Symbol.Empty, Symbol.Empty, Symbol.Square, Symbol.Diamond, Symbol.Empty } },
    //    //Bronze
    //    { 5, new Symbol[] { Symbol.Square, Symbol.Diamond, Symbol.Hammer, Symbol.Empty, Symbol.Empty, Symbol.Square, Symbol.Diamond, Symbol.HammerHammer } },
    //    //Silver
    //    { 6, new Symbol[] { Symbol.Square, Symbol.Diamond, Symbol.Hammer, Symbol.Empty, Symbol.Diamond, Symbol.SquareSquarePlus, Symbol.Diamond, Symbol.HammerHammer} },
    //    //Gold
    //    { 7, new Symbol[] { Symbol.Square, Symbol.DiamondDiamondPlus, Symbol.Hammer, Symbol.Square, Symbol.Diamond, Symbol.SquareSquarePlus, Symbol.Diamond, Symbol.HammerHammer} },
    //    //Diamond
    //    { 8, new Symbol[] { Symbol.Square, Symbol.DiamondDiamondPlus, Symbol.HammerHammer, Symbol.SquareSquare, Symbol.DiamondDiamond, Symbol.SquareSquarePlus, Symbol.Diamond, Symbol.HammerHammer } },
    //    //Platinum
    //    { 9, new Symbol[] { Symbol.Square, Symbol.DiamondDiamondPlus, Symbol.HammerHammerHammer, Symbol.SquareSquarePlus, Symbol.DiamondDiamondPlus, Symbol.SquareSquarePlus, Symbol.Diamond, Symbol.HammerHammer } }
    //};

    private void Start()
    {
        //Weisen den Shadern den jeweiligen R�dern zu, indem wir die Wheelspin-Objekte verwenden
        for (int i = 0; i < wheels.Length; i++)
        {
            MeshRenderer renderer = wheels[i].GetComponent<MeshRenderer>();
            if (renderer != null && renderer.materials.Length > symbolMaterialIndex)
            {
                //Holen das aktuelle Material-Array des Rads
                Material[] materials = renderer.materials;

                //�ndern nur das Material an dem angegebenen Index (f�r die Symbole)
                materials[symbolMaterialIndex] = materialOfSymbols[i];

                //Setzen das Material-Array zur�ck
                renderer.materials = materials;
            }
        }

        foreach (WheelSpin wheel in wheels)
        {
            //Registriere das Event, das aufgerufen wird, wenn ein Rad stoppt
            wheel.wheelStoppedEvent += OnWheelStopped;
        }
    }

    public void SpinAllWheels()
    {
        bool[] lockedWheels = new bool[wheels.Length];
        bool allLocked = true;

        for (int i = 0; i < wheels.Length; i++)
        {
            lockedWheels[i] = wheels[i].isLocked;
        }

        foreach (var lockedWheel in lockedWheels)
        {
            if (!lockedWheel)
            {
                allLocked = false;
                break;
            }
        }

        if (spinCount < maxSpins && canSpin && !allLocked)
        {
            canSpin = false;
            //Starte den Spin f�r alle R�der
            foreach (var wheel in wheels)
            {
                wheel.StartSpin();
            }

            spinCount++;    //Z�hler erh�hen
            UpdateSpinLamps();      //Aktualisieren der Lampen nach jedem Spin
        }
        else if (spinCount < maxSpins && canSpin && allLocked)
        {
            spinCount++;    //Z�hler erh�hen
            UpdateSpinLamps();      //Aktualisieren der Lampen nach jedem Spin

            if (spinCount >= maxSpins)
            {
                EndRound();
            }
        }

        if (firstSpin)
        {
            heroSelectionRotator.DeactivateSelection();
            AssignHeros();
            Debug.Log("Square Hero: " + squareHero + ", Diamond Hero: " + diamondHero);
            firstSpin = false;
        }
    }

    //Aktualisiere die Lampen je nach Anzahl der Spins
    void UpdateSpinLamps()
    {
        for (int i = 0; i < spinLamps.Length; i++)
        {
            if (i < spinCount)
            {
                //Schalte die Lampe ein (Farbe aktiv)
                spinLamps[i].color = activeColor;
            }
            else
            {
                //Lampe bleibt aus
                spinLamps[i].color = inactiveColor;
            }
        }
    }

    //Berechne, welches Symbol oben liegt, basierend auf der Rotation
    public Symbol GetTopSymbol(int wheelIndex, float rotationAngle)
    {
        //Berechne, in welchem der 8 m�glichen Slots das Rad gestoppt hat
        //int symbolIndex = Mathf.RoundToInt(rotationAngle + 180 / 45f) % 8;
        int symbolIndex = Mathf.RoundToInt(rotationAngle / 45f) % 8;
        if ((symbolIndex - 1) < 0)
        {
            symbolIndex = 7;
        }
        else { symbolIndex--; }
        //Debug.Log("Rad " + wheelIndex + " hat den Winkel: " + rotationAngle);
        //Debug.Log("Rad " + wheelIndex + " zeigt das Symbol: " + symbolIndex);
        return wheelSymbols[wheelIndex][symbolIndex];       //Gib das Symbol zur�ck
    }

    private void OnWheelStopped()
    {
        stoppedWheels++;

        //�berpr�fen, ob alle R�der gestoppt haben
        if (stoppedWheels == wheels.Length)
        {
            if (spinCount == 3)
            {
                EndRound();     //Rufe EndRound auf, wenn der dritte Spin beendet ist
            }
            stoppedWheels = 0;
            canSpin = true;
        }
    }

    private void AssignHeros()
    {
        squareHero = squareHeroSpawn.GetComponentInChildren<Hero>();
        diamondHero = diamondHeroSpawn.GetComponentInChildren<Hero>();
    }

    //Funktion zum beenden der Runde
    void EndRound()
    {
        Debug.Log("Runde beendet. Alle Spins verbraucht!");
        
        foreach (var wheel in wheels)
        {
            wheel.StopWheel();
        }
        EvaluateSymbols();
    }

    //Auswerten der R�der
    private void EvaluateSymbols()
    {
        //Z�hler f�r die Symbole
        int squareCount = 0;
        int diamondCount = 0;
        int hammerCount = 0;
        int xpSquareCount = 0;
        int xpDiamondCount = 0;

        //Symbole auf den R�dern auswerten
        foreach (var wheel in wheels)
        {
            Symbol topSymbol = wheel.getCurrentSymbol();

            switch (topSymbol)
            {
                case Symbol.Square:
                    squareCount++;
                    break;

                case Symbol.SquarePlus:
                    squareCount++;
                    xpSquareCount++;
                    break;

                case Symbol.SquareSquare:
                    squareCount += 2;
                    break;

                case Symbol.SquareSquarePlus:
                    squareCount += 2;
                    xpSquareCount++;
                    break;

                case Symbol.Diamond:
                    diamondCount++;
                    break;

                case Symbol.DiamondPlus:
                    diamondCount++;
                    xpDiamondCount++;
                    break;

                case Symbol.DiamondDiamond:
                    diamondCount += 2;
                    break;

                case Symbol.DiamondDiamondPlus:
                    diamondCount += 2;
                    xpDiamondCount++;
                    break;

                case Symbol.Hammer:
                    hammerCount++;
                    break;

                case Symbol.HammerHammer:
                    hammerCount += 2;
                    break;

                case Symbol.HammerHammerHammer:
                    hammerCount += 3;
                    break;

                case Symbol.Empty:
                    break;

                default:
                    Debug.LogError("Das Symbol " + topSymbol + " ist in diesem Kontext nicht vorhanden!");
                    break;
            }
        }

        //Berechne die Energie f�r die Square- und Diamant-Helden
        UpdateHeroEnergy(squareHero, squareCount, xpSquareCount);
        UpdateHeroEnergy(diamondHero, diamondCount, xpDiamondCount);

        //Bulwark-Level erh�hen, wenn gen�gend H�mmer gerollt wurden
        UpdateBulwark(hammerCount);
    }

    private void UpdateHeroEnergy(Hero hero, int symbolCount, int gainedXP)
    {
        if (gainedXP > 0)
        {
            hero.AddXP(gainedXP);
        }

        if (symbolCount >= 3)
        {
            //Energieberechnung: Ziehe 2 von der Anzahl der Symbole ab
            int energyGained = symbolCount - 2;
            hero.AddEnergy(energyGained);
            Debug.Log(hero + " hat " + gainedXP + " XP, und " + (symbolCount - 2) + " Energie erhalten");
        }
    }

    private void UpdateBulwark(int hammerCount)
    {
        if (hammerCount >= 3)
        {
            //Bulwark wird um die Anzahl der H�mmer - 2 erh�ht
            int bulwarkIncrease = hammerCount - 2;
            bulwarkMover.increaseBulwark(bulwarkIncrease);
            Debug.Log("Das Bulwark ist " + (hammerCount - 2) + " Stufen gr��er geworden.");
        }
    }

    public void ResetRound()
    {
        if (spinCount == 3)
        {
            spinCount = 0;
            stoppedWheels = 0;
            canSpin = true;

            foreach (var wheel in wheels)
            {
                wheel.isLocked = false;
                wheel.LockAnimationStart();
                wheel.setClampLock(false, true);
            }
            foreach (var lamp in spinLamps)
            {
                lamp.color = inactiveColor;
            }
        }
    }
}
