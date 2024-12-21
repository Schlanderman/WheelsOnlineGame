using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelManager : MonoBehaviour
{
    [SerializeField] private EnemyScript enemyScript;  //Offline only

    [SerializeField] private WheelSpin[] wheels;            //Eine Liste mit allen Rädern
    [SerializeField] private Material[] materialOfSymbols;  //Eine Liste mit allen Radvariationen (anhand von Materialien)
    private readonly int symbolMaterialIndex = 2;           //Index des Materials

    [SerializeField] private Image[] spinLamps;      //Array von UI-Images für die Lampen
    [SerializeField] private Color inactiveColor;
    [SerializeField] private Color activeColor;
    [SerializeField] private GameObject spinButton;

    private readonly int maxSpins = 3;      //Maximale Anzahl an Spins
    private int spinCount = 0;              //Zähler für die Anzahl der Spins
    public int stoppedWheels = 0;           //Anzahl der Räder, die sich nicht mehr drehen
    private bool canSpin = true;            //Zeigt an, ob man alle Räder drehen darf
    private bool firstSpin = true;          //Zeigt an, ob dies der erste Dreh des Spiels auf einer Seite ist

    //Events für andere Skripts
    public event Action<int, int, int, int, int> OnWheelsHaveStopped;
    public event EventHandler OnUnlockClamps;

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

        foreach (WheelSpin wheel in wheels)
        {
            //Registriere das Event, das aufgerufen wird, wenn ein Rad stoppt
            wheel.WheelStoppedEvent += OnWheelStopped;
        }

        //Zu begin den Drehbutton deaktivieren
        spinButton.SetActive(false);

        TurnManager.Instance.OnResetRound += TurnManager_OnResetRound;
        TurnManager.Instance.OnInitializePlayerRoundFinished += TurnManager_OnInitializePlayerRoundFinished;
        InitialHeroSetting.Instance.OnActivateSpinButton += InitialHeroSetting_OnActivateSpinButton;
    }

    //Events für dieses Skript
    private void TurnManager_OnResetRound(object sender, EventArgs e)
    {
        ResetRound();
        OnUnlockClamps?.Invoke(this, EventArgs.Empty);
    }

    private void TurnManager_OnInitializePlayerRoundFinished(object sender, EventArgs e)
    {
        if (enemyScript != null)
        {
            TurnManager.Instance.ChangePlayerRoundFinished(enemyScript.playerId, false);
        }
        else
        {
            //TurnManager.Instance.ChangePlayerRoundFinished(PlayerScript.Instance.playerId, false);
        }
    }

    private void InitialHeroSetting_OnActivateSpinButton(object sender, EventArgs e)
    {
        ActivateSpinButton();
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
            //Starte den Spin für alle Räder
            foreach (var wheel in wheels)
            {
                wheel.StartSpin();
            }

            spinCount++;    //Zähler erhöhen
            UpdateSpinLamps();      //Aktualisieren der Lampen nach jedem Spin
        }
        else if (spinCount < maxSpins && canSpin && allLocked)
        {
            spinCount++;    //Zähler erhöhen
            UpdateSpinLamps();      //Aktualisieren der Lampen nach jedem Spin

            if (spinCount >= maxSpins)
            {
                EndRound();
            }
        }

        if (firstSpin)
        {
            //InitiateHeroAssignment();
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
        //Berechne, in welchem der 8 möglichen Slots das Rad gestoppt hat
        int symbolIndex = Mathf.RoundToInt(rotationAngle / 45f) % 8;
        if ((symbolIndex - 1) < 0)
        {
            symbolIndex = 7;
        }
        else { symbolIndex--; }

        return wheelSymbols[wheelIndex][symbolIndex];       //Gib das Symbol zurück
    }

    private void OnWheelStopped()
    {
        stoppedWheels++;

        //Überprüfen, ob alle Räder gestoppt haben
        if (stoppedWheels == wheels.Length)
        {
            if (spinCount == 3)
            {
                EndRound();     //Rufe EndRound auf, wenn der dritte Spin beendet ist
                return;
            }
            stoppedWheels = 0;
            canSpin = true;
        }
    }

    //Funktion zum beenden der Runde
    public void EndRound()
    {
        Debug.Log("Runde beendet. Alle Spins verbraucht!");

        foreach (var wheel in wheels)
        {
            wheel.StopWheel();
        }

        //Hier übergeben, dass der Spieler bereit ist
        if (enemyScript != null)
        {
            TurnManager.Instance.ChangePlayerRoundFinished(enemyScript.playerId, true);
        }
        else
        {
            //TurnManager.Instance.ChangePlayerRoundFinished(PlayerScript.Instance.playerId, true);
        }

        OnWheelsHaveStopped?.Invoke(
            wheels[0].GetTopSymbolIndex(),
            wheels[1].GetTopSymbolIndex(),
            wheels[2].GetTopSymbolIndex(),
            wheels[3].GetTopSymbolIndex(),
            wheels[4].GetTopSymbolIndex()
            );

        TurnManager.Instance.TestForReadyness();
    }

    private void ResetRound()
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
                wheel.SetClampLock(false, true);
            }
            foreach (var lamp in spinLamps)
            {
                lamp.color = inactiveColor;
            }
        }
    }

    //Die Buttons zum Drehen aktivieren, wenn beide Spieler bereit sind
    private void ActivateSpinButton()
    {
        spinButton.SetActive(true);
    }

    public WheelSpin[] GetWheels()
    {
        return wheels;
    }

    public void SetUIElements(GameObject spinBtn, Image[] lamps)
    {
        spinButton = spinBtn;
        spinLamps = lamps;
    }
}
