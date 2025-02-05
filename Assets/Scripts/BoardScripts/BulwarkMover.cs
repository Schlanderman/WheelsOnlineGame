using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulwarkMover : NetworkBehaviour
{
    [SerializeField] private Transform bulwark;             //Das Objekt, das das Bulwark darstellt
    [SerializeField] private float restingPosition = 0f;    //Die Position, wenn das Bulwark auf 0 steht
    [SerializeField] private float startYPosition = 0.42f;  //Die Startposition des Bulwark wenn es mindestens 1 ist
    [SerializeField] private float stepSize = 0.07f;        //Die Größe jedes Schritts (Verschiebung pro Energieeinheit)

    public NetworkVariable<int> bulwarkLevel = new NetworkVariable<int>(
        0,  //Startwert
        NetworkVariableReadPermission.Everyone,     //Clients dürfen den Wert lesen
        NetworkVariableWritePermission.Server       //Nur der Server darf den Wert schreiben
        );

    //Events
    public event Action<float> OnMoveBulwark;

    private void Start()
    {
        bulwarkLevel.OnValueChanged += BulwarkLevelChanged;
    }

    public void increaseBulwark(int height)
    {
        if (!IsServer) { return; }

        bulwarkLevel.Value += height;
        if (bulwarkLevel.Value >= 5)
        {
            bulwarkLevel.Value = 5;
        }
    }

    public void decreaseBulwark(int height)
    {
        if (!IsServer) { return; }

        bulwarkLevel.Value -= height;
        if (bulwarkLevel.Value <= 0)
        {
            bulwarkLevel.Value = 0;
        }
    }

    //Methode, wenn sich das Bulwark bewegen soll
    private void BulwarkLevelChanged(int previousValue, int newValue)
    {
        UpdateBulwark();
    }

    //Methode um das Bulwark zu aktualisieren
    private void UpdateBulwark()
    {
        if (bulwarkLevel.Value == 0)
        {
            StartCoroutine(MoveBulwark(restingPosition));
        }
        else
        {
            //Berechne die neue Y-Position basierend auf der Höhe
            float newYPosition = startYPosition + (bulwarkLevel.Value * stepSize);

            //Setze die neue Y-Position des Bulwark
            StartCoroutine(MoveBulwark(newYPosition));
        }
    }

    private IEnumerator MoveBulwark(float targetYPosition)
    {
        OnMoveBulwark?.Invoke(targetYPosition);

        //Audio abspielen
        AudioManager.Instance.PlaySoundClip(SoundClipRef.BrickBuildUp, SoundSourceRef.SFXSource, 0.3f);

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
        return bulwarkLevel.Value;
    }
}
