using UnityEngine;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private GameObject tieScreen;

    private void Start()
    {
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
        tieScreen.SetActive(false);

        TurnManager.Instance.OnSetEndscreen += TurnManager_OnSetEndscreen;
    }

    private void TurnManager_OnSetEndscreen(bool isTied, ulong winnerId)
    {
        //if (isTied)
        //{
        //    tieScreen.SetActive(true);
        //}
        //else if (winnerId == PlayerScript.Instance.playerId)
        //{
        //    winScreen.SetActive(true);
        //}
        //else
        //{
        //    loseScreen.SetActive(true);
        //}
    }
}
