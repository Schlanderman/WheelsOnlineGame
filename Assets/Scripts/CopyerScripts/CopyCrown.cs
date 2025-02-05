using System.Collections;
using UnityEngine;

public class CopyCrown : ManagerCopiesHandler<CrownManager>
{
    //Referenzen zu den  Rädern für die Anzeige
    [SerializeField] Transform oneWheel;
    [SerializeField] Transform tenWheel;

    protected override void SetEvents()
    {
        originalManager.OnSetNewHPStatus += CrownManager_OnSetNewHPStatus;
        if (IsServer) { originalManager.UpdateHPDisplayGlobal(); }  //Ursprüngliches Update düe die HP Anzeige
    }

    private void CrownManager_OnSetNewHPStatus(float rotationOnes, float rotationTens)
    {
        StartCoroutine(WheelRotator(rotationOnes, rotationTens));
    }

    private IEnumerator WheelRotator(float rotationOnes, float rotationTens)
    {
        //Audio abspielen
        if (IsOwner) { AudioManager.Instance.PlaySoundClip(SoundClipRef.LoopingRotatingWheelShort, SoundSourceRef.SFXSource, 0.1f); }

        //Dauer der Animation
        float duration = 0.3f;
        float timeElapsed = 0f;

        //Die aktuelle Rotation in Quaternion speichern
        Vector3 initialRotationOnes = Vector3.zero;
        Vector3 initialRotationTens = Vector3.zero;

        initialRotationOnes.x = oneWheel.localEulerAngles.x;
        initialRotationTens.x = tenWheel.localEulerAngles.x;

        //Zielrotation in Quaternion berechnen, um eine sanfte Imterpolation durchzuführen
        Vector3 targetRotationOnes = Vector3.zero;
        Vector3 targetRotationTens = Vector3.zero;

        targetRotationOnes.x = rotationOnes;
        targetRotationTens.x = rotationTens;

        //Führe die Interpolation über die Zeit durch
        while (timeElapsed < duration)
        {
            //Linear interpolieren zwischen der Startrotation und der Zielrotation
            oneWheel.localEulerAngles = Vector3.Lerp(initialRotationOnes, targetRotationOnes, timeElapsed / duration);
            tenWheel.localEulerAngles = Vector3.Lerp(initialRotationTens, targetRotationTens, timeElapsed / duration);
            

            //Erhöhe die verstrichene Zeit
            timeElapsed += Time.deltaTime;
            //Warte bis zum nächsten Frame
            yield return null;
        }

        oneWheel.localEulerAngles = targetRotationOnes;
        tenWheel.localEulerAngles = targetRotationTens;
    }
}
