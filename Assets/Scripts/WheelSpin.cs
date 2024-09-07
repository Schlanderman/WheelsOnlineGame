using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSpin : MonoBehaviour
{
    [SerializeField]
    private float spinSpeed = 500f;     //Anfängliche Geschwindigkeit der Rotation
    [SerializeField]
    private float slowDownRate = 2f;    //Rate, mit der das Rad langsamer wird
    private bool isSpinning = false;
    private float currentSpinSpeed;
    public bool hasStopped = false;     //Ob das Rad angehalten hat
    public bool isLocked = false;       //Ob das Rad gesperrt ist

    [SerializeField]
    private ClampActivator clampedActivator;

    private void Update()
    {
        if (isSpinning && !isLocked)    //Nur rotieren, wenn das Rad nicht gesperrt ist
        {
            //Solange das Rad dreht, rotieren
            transform.Rotate(Vector3.right, currentSpinSpeed * Time.deltaTime);

            //Reduziere die Spingeschwindigkeit allmählich
            currentSpinSpeed = Mathf.Lerp(currentSpinSpeed, 0f, Time.deltaTime * slowDownRate);

            //Wenn die Geschwindigkeit fast 0 erreicht, stoppe das Rad
            if (currentSpinSpeed < 1f)
            {
                currentSpinSpeed = 0f;
                isSpinning = false;
                SnapToNearestSymbol();
                hasStopped = true;
                Debug.Log("Rad ist gestoppt! Rotation: " + transform.eulerAngles.z);
            }
        }
    }

    //Methode, um das Drehen zu starten
    public void StartSpin()
    {
        if (!isSpinning && !isLocked)   //Drehe nur, wenn das Rad nicht gesperrt ist
        {
            currentSpinSpeed = spinSpeed;
            isSpinning = true;
            hasStopped = false;
            slowDownRate = Random.Range(1f, 2f);
        }
    }

    //Methode um das Rad zu sperren oder zu entsperren
    public void ToggleLock()
    {
        isLocked = !isLocked;
        clampedActivator.ToggleClamps(isLocked);    //Klammeranimation triggern
        Debug.Log("Rad " + (isLocked ? "gesperrt" : "entsperrt"));
    }

    //Diese Methode wird aufgerufen, wenn das Rad angeklickt wird
    public void OnMouseDown()
    {
        ToggleLock();   //Entsperre oder sperre das Rad beim Klick
    }

    //Methode, um das Ergebnis basierend auf der Rotation zu analysieren
    public int GetResult()
    {
        if (hasStopped)
        {
            float xRotation = transform.eulerAngles.x;
            //Berechne das angezeigte Symbol basierend auf der z-Rotation
            //Jedes Segment des Rades ist 45 Grad (360/8 Symbole)
            int symbolindex = Mathf.FloorToInt(xRotation / (360f / 8f)) % 8;
            return symbolindex;     //Gib den Index des Symbols zurück
        }

        return -1;      //Wenn das Rad noch nicht gestoppt hat
    }

    void SnapToNearestSymbol()
    {
        //Aktuelle X-Rotation abrufen
        float currentXRotation = transform.eulerAngles.x;

        //Die nächste Symbolposition berechnen (vielfaches von 45°)
        float snappedRotation = Mathf.Round(currentXRotation / 45f) * 45f;

        //Setze die Rotation des Rads auf diese genaue Symbolpsoition
        transform.eulerAngles = new Vector3(snappedRotation, transform.eulerAngles.y, transform.eulerAngles.z);

        Debug.Log("Gesnapped auf: " + snappedRotation);
    }
}
