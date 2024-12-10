using System.Collections;
using UnityEngine;

public class CopyEnergyBar : ManagerCopiesHandler<EnergyBar>
{
    [SerializeField] private Transform energyBar;       //Das Objekt, das die Stange darstellt

    protected override void SetEvents()
    {
        originalManager.OnActivateChangeEnergyBar += EnergyBar_OnActivateChangeEnergyBar;
    }

    private void EnergyBar_OnActivateChangeEnergyBar(float targetYPosition)
    {
        StartCoroutine(MoveEnergyBar(targetYPosition));
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
