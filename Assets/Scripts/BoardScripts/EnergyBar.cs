using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBar : MonoBehaviour
{
    [SerializeField] private Transform energyBar;       //Das Objekt, das die Stange darstellt
    [SerializeField] private float startYPosition = 0f; //Die Startposition der Stange
    [SerializeField] private float stepSize = .12f;     //Die Gr��e jedes Schritts (Verschiebung pro Energieeinheit)

    //Methode, um die Energieanzeige zu aktualisieren
    public void UpdateEnergieDisplay(int currentEnergy, int maxEnergy)
    {
        //Berechne die Anzehl der Schritte, die die Stange bewegen soll
        int energyNeeded = maxEnergy - currentEnergy;

        //Berechne die neue Y-Position basierend auf der verbleibenden Energie
        float newYPosition = startYPosition + (energyNeeded * stepSize);

        //Setze die neue Y-Position der Stange
        //Vector3 newPosition = new Vector3(energyBar.localPosition.x, newYPosition, energyBar.localPosition.z);
        //energyBar.localPosition = newPosition;
        StartCoroutine(MoveEnergyBar(newYPosition));
    }

    private IEnumerator MoveEnergyBar(float targetYPosition)
    {
        //Dauer der Animation
        float duration = 0.2f;
        float timeElapsed = 0f;

        //Ausgangsposition
        Vector3 startPosition = energyBar.localPosition;

        //Zielposition mit neuem Y-Wert
        Vector3 finalPosition = new Vector3(startPosition.x, targetYPosition, startPosition.z);

        //Bewege die Stange
        while (timeElapsed < duration)
        {
            //Interpoliere die Position
            energyBar.localPosition = Vector3.Lerp(startPosition, finalPosition, timeElapsed / duration);

            //Erh�he die vergangene Zeit
            timeElapsed += Time.deltaTime;

            //Warte bis zum n�chsten Frame
            yield return null;
        }

        energyBar.localPosition = finalPosition;
    }
}
