using UnityEngine;
using UnityEngine.UI;

public class UmfrageManager : MonoBehaviour
{
    [SerializeField] private Button feedbackButton;     //Button, der zur Umfrage f�hrt
    [SerializeField] private GameObject tooltipText;    //Tooltip Text, der erscheint, wenn man �ber den Button hovert
    private string feedbackUrl = "https://forms.office.com/r/dym9W9Sfqu";

    private void Start()
    {
        //Tooltip deaktivieren
        tooltipText.SetActive(false);

        //Button zuweisen
        feedbackButton.onClick.AddListener(OpenFeedbackForm);
    }

    public void OnHoverEnter()
    {
        //Tooltip aktivieren
        tooltipText.SetActive(true);
    }

    public void OnHoverExit()
    {
        //Tooltip deaktivieren
        tooltipText.SetActive(false);
    }

    private void OpenFeedbackForm()
    {
        //Link im Standardbrowser �ffnen
        Application.OpenURL(feedbackUrl);
    }
}
