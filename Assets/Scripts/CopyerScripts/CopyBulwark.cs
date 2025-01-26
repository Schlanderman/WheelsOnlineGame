using System.Collections;
using UnityEngine;

public class CopyBulwark : ManagerCopiesHandler<BulwarkMover>
{
    [SerializeField] private Transform bulwark;

    protected override void SetEvents()
    {
        originalManager.OnMoveBulwark += BulwarkMover_OnMoveBulwark;
    }

    private void BulwarkMover_OnMoveBulwark(float targetYPosition)
    {
        StartCoroutine(MoveBulwark(targetYPosition));
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
}
