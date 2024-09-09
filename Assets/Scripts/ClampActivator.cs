using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ClampActivator : MonoBehaviour
{
    [SerializeField]
    private Transform upperClamp;      //Obere Klammer
    [SerializeField]
    private Transform lowerClamp;      //Untere Klammer

    private Vector3 upperUnlockedState = new Vector3(-60f, 0f, 0f);     //Position für die obere Klammer offen
    private Vector3 lowerUnlockedState = new Vector3(110f, 0f, 0f);     //Position für die untere Klammer offen
    private Vector3 lockedState = new Vector3(0f, 0f, 0f);              //Position für bede Klammern geschlossen

    [SerializeField]
    private float animationDuration = 0.2f;     //Dauer der Animation
    private Quaternion targetRotationUpper;     //Zielrotation der oberen Klammer
    private Quaternion targetRotationLower;     //Zielrotation der unteren Klammer
    private Quaternion startRotationUpper;      //Startrotation der oberen Klammer
    private Quaternion startRotationLower;      //Startrotation der unteren Klammer
    private float animationTime = 0f;           //Fortschritt der Animation
    private bool isAnimating = false;           //Ob gerade eine Animation läuft

    public void ToggleClamps(bool isLocked)
    {
        //Debug.Log("Clamp " + this + " wurde aktiviert.");

        // setze die Zielrotation basierend auf dem Lock-Status
        targetRotationUpper = isLocked ? Quaternion.Euler(lockedState) : Quaternion.Euler(upperUnlockedState);
        targetRotationLower = isLocked ? Quaternion.Euler(lockedState) : Quaternion.Euler(lowerUnlockedState);
        startRotationUpper = upperClamp.localRotation;
        startRotationLower = lowerClamp.localRotation;
        animationTime = 0f;
        isAnimating = true;
    }

    private void Update()
    {
        if (isAnimating)
        {
            //Erhöhe den Animationsfortschritt
            animationTime += Time.deltaTime / animationDuration;

            //Lerp zwischen Start- und Zielrotation basierend auf der Animationszeit
            upperClamp.localRotation = Quaternion.Lerp(startRotationUpper, targetRotationUpper, animationTime);
            lowerClamp.localRotation = Quaternion.Lerp(startRotationLower, targetRotationLower, animationTime);

            //Beende die Animation, wenn sie abgeschlossen ist
            if (animationTime >= 1f)
            {
                isAnimating = false;
            }
        }
    }
}
