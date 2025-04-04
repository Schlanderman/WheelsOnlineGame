using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private GameObject tieScreen;
    [SerializeField] private Button quitToMainMenuButton;

    private void Awake()
    {
        
    }

    private void Start()
    {
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
        tieScreen.SetActive(false);
        quitToMainMenuButton.gameObject.SetActive(false);

        TurnManager.Instance.OnSetEndscreen += TurnManager_OnSetEndscreen;

        quitToMainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("MainMenu");
        });
    }

    private void TurnManager_OnSetEndscreen(bool isTied, ulong winnerId)
    {
        if (isTied)
        {
            tieScreen.SetActive(true);
            quitToMainMenuButton.gameObject.SetActive(true);
        }
        else if (winnerId == PlayerScript.LocalInstance.OwnerClientId)
        {
            winScreen.SetActive(true);
            quitToMainMenuButton.gameObject.SetActive(true);
        }
        else
        {
            loseScreen.SetActive(true);
            quitToMainMenuButton.gameObject.SetActive(true);
        }
    }
}
