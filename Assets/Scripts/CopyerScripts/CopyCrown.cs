using System.Collections;
using UnityEngine;

public class CopyCrown : ManagerCopiesHandler<CrownManager>
{
    //Referenzen zu den  R�dern f�r die Anzeige
    [SerializeField] Transform oneWheel;
    [SerializeField] Transform tenWheel;

    protected override void SetEvents()
    {
        originalManager.OnSetNewHPStatus += CrownManager_OnSetNewHPStatus;
    }

    private void CrownManager_OnSetNewHPStatus(float rotationOnes, float rotationTens)
    {
        StartCoroutine(WheelRotator(rotationOnes, rotationTens));
    }

    private IEnumerator WheelRotator(float rotationOnes, float rotationTens)
    {
        //Dauer der Animation
        float duration = 0.3f;
        float timeElapsed = 0f;

        //Die aktuelle Rotation in Quaternion speichern
        Vector3 initialRotationOnes = Vector3.zero;
        Vector3 initialRotationTens = Vector3.zero;

        initialRotationOnes.x = oneWheel.localEulerAngles.x;
        initialRotationTens.x = tenWheel.localEulerAngles.x;

        //Zielrotation in Quaternion berechnen, um eine sanfte Imterpolation durchzuf�hren
        Vector3 targetRotationOnes = Vector3.zero;
        Vector3 targetRotationTens = Vector3.zero;

        targetRotationOnes.x = rotationOnes;
        targetRotationTens.x = rotationTens;

        //F�hre die Interpolation �ber die Zeit durch
        while (timeElapsed < duration)
        {
            //Linear interpolieren zwischen der Startrotation und der Zielrotation
            oneWheel.localEulerAngles = Vector3.Lerp(initialRotationOnes, targetRotationOnes, timeElapsed / duration);
            tenWheel.localEulerAngles = Vector3.Lerp(initialRotationTens, targetRotationTens, timeElapsed / duration);
            

            //Erh�he die verstrichene Zeit
            timeElapsed += Time.deltaTime;
            //Warte bis zum n�chsten Frame
            yield return null;
        }

        oneWheel.localEulerAngles = targetRotationOnes;
        tenWheel.localEulerAngles = targetRotationTens;
    }
}
