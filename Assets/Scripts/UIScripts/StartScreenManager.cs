using UnityEngine;
using UnityEngine.UI;

public class StartScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject startScreen;
    [SerializeField] private Button continueButton;
    [SerializeField] private Toggle dontShowAgainToggle;

    private void Start()
    {
        //Falls man zuvor DontShowAgain ausgewählt hat, dann wird der Screen nicht angezeigt
        if (ProfileData.Instance.DontShowStartScreenAgain) { Hide(); }
        else { Show(); }

        continueButton.onClick.AddListener(ContinueClicked);
    }

    private void Show()
    {
        startScreen.SetActive(true);
    }

    private void Hide()
    {
        startScreen.SetActive(false);
    }

    private void ContinueClicked()
    {
        Hide();
        ProfileData.Instance.SetShowStartScreenAgain(dontShowAgainToggle.isOn);
    }
}
