using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulwarkMover : MonoBehaviour
{
    [SerializeField] private Transform bulwark;             //Das Objekt, das das Bulwark darstellt
    [SerializeField] private float restingPosition = 0f;    //Die Position, wenn das Bulwark auf 0 steht
    [SerializeField] private float startYPosition = 0.42f;  //Die Startposition des Bulwark wenn es mindestens 1 ist
    [SerializeField] private float stepSize = 0.07f;        //Die Größe jedes Schritts (Verschiebung pro Energieeinheit)

    private int bulwarkLevel = 0;

    public void increaseBulwark(int height)
    {
        bulwarkLevel += height;
        if (bulwarkLevel >= 5)
        {
            bulwarkLevel = 5;
        }
        UpdateBulwark();
    }

    public void decreaseBulwark(int height)
    {
        bulwarkLevel -= height;
        if (bulwarkLevel <= 0)
        {
            bulwarkLevel = 0;
        }
        UpdateBulwark();
    }

    //Methode um das Bulwark zu aktualisieren
    private void UpdateBulwark()
    {
        if (bulwarkLevel == 0)
        {
            StartCoroutine(MoveBulwark(restingPosition));
        }
        else
        {
            //Berechne die neue Y-Position basierend auf der Höhe
            float newYPosition = startYPosition + (bulwarkLevel * stepSize);

            //Setze die neue Y-Position des Bulwark
            StartCoroutine(MoveBulwark(newYPosition));
        }
    }

    private IEnumerator MoveBulwark(float targetYPosition)
    {
        //Dauer der Animation
        float duration = 0.5f;
        float timeElapsed = 0f;

        //Ausgangsposition
        Vector3 startPosition = bulwark.localPosition;

        //Zielposition mit neuem Y-Wert
        Vector3 finalPosition = new Vector3(startPosition.x, targetYPosition, startPosition.z);

        //Bewege die Stange
        while (timeElapsed < duration)
        {
            //Interpoliere die Position
            bulwark.localPosition = Vector3.Lerp(startPosition, finalPosition, timeElapsed / duration);

            //Erhöhe die vergangene Zeit
            timeElapsed += Time.deltaTime;

            //Warte bis zum nächsten Frame
            yield return null;
        }

        bulwark.localPosition = finalPosition;
    }

    public int GetBulwarkLevel()
    {
        return bulwarkLevel;
    }
}
