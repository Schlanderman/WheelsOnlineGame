using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CopyEnergyBar : ManagerCopiesHandler<EnergyBar>
{
    [SerializeField] private Transform energyBar;       //Das Objekt, das die Stange darstellt
    [SerializeField] private GameObject uiCoverEnergyBar;

    protected override void SetEvents()
    {
        originalManager.OnActivateChangeEnergyBar += EnergyBar_OnActivateChangeEnergyBar;
        MultiplayerGameManager.Instance.OnPlayersAreReadyToPlay += MultiplayerGameManager_OnPlayersAreReadyToPlay;
        HideUI();
    }

    private void MultiplayerGameManager_OnPlayersAreReadyToPlay(object sender, System.EventArgs e)
    {
        ShowUIRpc();
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

            //Erhöhe die vergangene Zeit
            timeElapsed += Time.deltaTime;

            //Warte bis zum nächsten Frame
            yield return null;
        }

        energyBar.localPosition = finalPosition;
    }

    [Rpc(SendTo.Everyone)]
    private void ShowUIRpc()
    {
        energyBar.gameObject.SetActive(true);
        uiCoverEnergyBar.SetActive(false);
    }

    //[Rpc(SendTo.Everyone)]
    private void HideUI()
    {
        energyBar.gameObject.SetActive(false);
        uiCoverEnergyBar.SetActive(true);
    }
}
