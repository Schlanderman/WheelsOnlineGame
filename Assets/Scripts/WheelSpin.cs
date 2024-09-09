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
    private bool clampLock = false;
    private bool turnClampLock = false;
    private bool initialClampLock = true;

    [SerializeField]
    private int wheelIndex;     //Welches Rad ist das? (0-4)
    private float finalRotation;

    private WheelManager wheelManager;

    [SerializeField]
    private ClampActivator clampedActivator;

    public delegate void OnWheelStopped();
    public event OnWheelStopped wheelStoppedEvent;

    private void Start()
    {
        wheelManager = FindObjectOfType<WheelManager>();
    }

    private void Update()
    {
        if (isSpinning && !isLocked)    //Nur rotieren, wenn das Rad nicht gesperrt ist
        {
            //Solange das Rad dreht, rotieren
            transform.Rotate(Vector3.right, currentSpinSpeed * Time.deltaTime);

            //Reduziere die Spingeschwindigkeit allmählich
            currentSpinSpeed = Mathf.Lerp(currentSpinSpeed, 0f, Time.deltaTime * slowDownRate);

            //Wenn die Geschwindigkeit fast 0 erreicht, stoppe das Rad
            if (currentSpinSpeed < 10f)
            {
                currentSpinSpeed = 0f;
                isSpinning = false;
                SnapToNearestSymbol();
                hasStopped = true;
                turnClampLock = false;
                //Debug.Log("Rad ist gestoppt! Rotation: " + transform.eulerAngles.x);

                //Benachrichtige den WheelManager, dass dieses Rad gestoppt hat
                if (wheelStoppedEvent != null)
                {
                    wheelStoppedEvent.Invoke();
                }
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
            turnClampLock = true;
            slowDownRate = Random.Range(1f, 2f);
            spinSpeed = Random.Range(400f, 900f);
        }
        else if (isLocked)
        {
            //Benachrichtige den WheelManager, dass dieses Rad gelocked ist
            if (wheelStoppedEvent != null)
            {
                wheelStoppedEvent.Invoke();
                clampLock = true;
            }
        }

        initialClampLock = false;
    }

    //Methode um das Rad zu sperren oder zu entsperren
    public void ToggleLock()
    {
        if (initialClampLock)
        {
            return;
        }

        if (!clampLock && !turnClampLock)
        {
            isLocked = !isLocked;
            clampedActivator.ToggleClamps(isLocked);    //Klammeranimation triggern
            //Debug.Log("Rad " + (isLocked ? "gesperrt" : "entsperrt"));
        }
    }

    public void LockAnimationStart()
    {
        clampedActivator.ToggleClamps(isLocked);    //Klammeranimation triggern
    }

    //Diese Methode wird aufgerufen, wenn das Rad angeklickt wird
    public void OnMouseDown()
    {
        ToggleLock();   //Entsperre oder sperre das Rad beim Klick
    }

    void SnapToNearestSymbol()
    {
        //Berechne die X-Rotation konsistent mit Atan2
        //float currentXRotation = GetXRotationConsistent(transform);

        // Aktuelle Z-Rotation abrufen
        float currentXRotation = transform.eulerAngles.x;

        //Die nächste Symbolposition berechnen (vielfaches von 45°)
        float snappedRotation = Mathf.Round(currentXRotation / 45f) * 45f;
        finalRotation = Mathf.Round(GetXRotationConsistent(transform) / 45) * 45;
        //Debug.Log(this + " war " + currentXRotation + " und ist gesnapped auf: " + snappedRotation);

        //Setze die Rotation des Rads auf diese genaue Symbolpsoition
        //transform.eulerAngles = new Vector3(snappedRotation, transform.eulerAngles.y, transform.eulerAngles.z);

        //Starte die Coroutine für die sanfte Rotation
        StartCoroutine(SmoothSnapRotation(snappedRotation));
    }

    public void StopWheel()
    {
        //Berechne die X-Rotation, wo das Rad gestoppt hat, konsistent mit Atan2
        //float currentXRotation = GetXRotationConsistent(transform);

        //Debug.Log(this + " stoppt bei Winkel: " + finalRotation);
        //Hole das Symbol, das oben liegt
        Symbol topSymbol = wheelManager.GetTopSymbol(wheelIndex, finalRotation);

        isLocked = true;
        clampLock = true;
        clampedActivator.ToggleClamps(isLocked);    //Klammeranimation triggern

        //Verarbeite das Symbol
        //Debug.Log("Rad " + wheelIndex + " Symbol: " + topSymbol);
    }

    public bool HasStopped()
    {
        return hasStopped;
    }

    float GetXRotationConsistent(Transform targetTransform)
    {
        // Berechne den up-Vektor der lokalen Rotation des Objekts
        Vector3 up = targetTransform.up;

        // Berechne den Winkel in der Y-Z-Ebene (x-Rotation)
        float xRotation = Mathf.Atan2(up.z, up.y) * Mathf.Rad2Deg;
        xRotation *= -1f;

        // Stelle sicher, dass der Winkel im Bereich 0 bis 360 Grad liegt
        if (xRotation < 0)
        {
            xRotation += 360;
        }

        return xRotation;
    }

    public Symbol getCurrentSymbol()
    {
        return wheelManager.GetTopSymbol(wheelIndex, finalRotation);
    }

    private IEnumerator SmoothSnapRotation(float targetXRotation)
    {
        //Dauer der Animation
        float duration = 0.5f;
        float timeElapsed = 0f;

        //Die aktuelle Rotation in Quaternion speichern
        Quaternion initialRotation = transform.rotation;

        //Zielrotation in Quaternion berechnen, um eine sanfte Imterpolation durchzuführen
        Quaternion targetRotation = Quaternion.Euler(targetXRotation, transform.eulerAngles.y, transform.eulerAngles.z);

        //Führe die Interpolation über die Zeit durch
        while (timeElapsed < duration)
        {
            //Linear interpolieren zwischen der Startrotation und der Zielrotation
            transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, timeElapsed / duration);

            //Erhöhe die verstrichene Zeit
            timeElapsed += Time.deltaTime;
            //Warte bis zum nächsten Frame
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    public void setClampLock(bool overalLLock, bool initialLock)
    {
        clampLock = overalLLock;
        initialClampLock = initialLock;
    }
}
