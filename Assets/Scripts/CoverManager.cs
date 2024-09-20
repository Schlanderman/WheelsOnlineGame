using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverManager : MonoBehaviour
{
    [SerializeField] private Transform cover;

    private float startRotation = 90f;
    private float finalRotation = 0f;

    private void Start()
    {
        cover.localEulerAngles = new Vector3(startRotation, 0, 0);
    }

    public IEnumerator SetCoverUp()
    {
        //Dauer der Animation
        float duration = 0.8f;
        float timeElapsed = 0f;

        //Die aktuelle Rotation in Quaternion speichern
        Quaternion initialRotation = cover.rotation;

        //Zielrotation in Quaternion berechnen, um eine sanfte Imterpolation durchzuführen
        Quaternion targetRotation = Quaternion.Euler(finalRotation, cover.eulerAngles.y, cover.eulerAngles.z);

        //Führe die Interpolation über die Zeit durch
        while (timeElapsed < duration)
        {
            //Linear interpolieren zwischen der Startrotation und der Zielrotation
            cover.rotation = Quaternion.Lerp(initialRotation, targetRotation, timeElapsed / duration);

            //Erhöhe die verstrichene Zeit
            timeElapsed += Time.deltaTime;
            //Warte bis zum nächsten Frame
            yield return null;
        }

        cover.rotation = targetRotation;
    }

    public IEnumerator SetCoverDown()
    {
        //Dauer der Animation
        float duration = 0.8f;
        float timeElapsed = 0f;

        //Die aktuelle Rotation in Quaternion speichern
        Quaternion initialRotation = cover.rotation;

        //Zielrotation in Quaternion berechnen, um eine sanfte Imterpolation durchzuführen
        Quaternion targetRotation = Quaternion.Euler(startRotation, cover.eulerAngles.y, cover.eulerAngles.z);

        //Führe die Interpolation über die Zeit durch
        while (timeElapsed < duration)
        {
            //Linear interpolieren zwischen der Startrotation und der Zielrotation
            cover.rotation = Quaternion.Lerp(initialRotation, targetRotation, timeElapsed / duration);

            //Erhöhe die verstrichene Zeit
            timeElapsed += Time.deltaTime;
            //Warte bis zum nächsten Frame
            yield return null;
        }

        cover.rotation = targetRotation;
    }
}
