using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelManager : MonoBehaviour
{
    [SerializeField]
    private WheelSpin[] wheels;     //Eine Liste mit allen R�dern

    private int maxSpins = 3;       //Maximale Anzahl an Spins
    private int spinCount = 0;      //Z�hler f�r die Anzahl der Spins

    [SerializeField]
    private Image[] spinLamps;      //Array von UI-Images f�r die Lampen
    [SerializeField]
    private Color inactiveColor;
    [SerializeField]
    private Color activeColor;

    public void SpinAllWheels()
    {
        if (spinCount < maxSpins)
        {
            //Starte den Spin f�r alle R�der
            foreach (var wheel in wheels)
            {
                wheel.StartSpin();
            }
        }

        spinCount++;    //Z�hler erh�hen
        UpdateSpinLamps();      //Aktualisieren der Lampen nach jedem Spin

        //Nach dem Stoppen, wenn die max. Spin Anzahl erreicht ist, Runde beenden
        if (spinCount >= maxSpins)
        {
            EndRound();
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

    //Funktion zum beenden der Runde
    void EndRound()
    {
        Debug.Log("Runde beendet. Alle Spins verbraucht!");
        //TODO
    }
}
