using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SelfHud : MonoBehaviour
{
    [SerializeField] private Button figureSquareSelectLeftButton;
    [SerializeField] private Button figureSquareSelectRightButton;
    [SerializeField] private Button figureDiamondSelectLeftButton;
    [SerializeField] private Button figureDiamondSelectRightButton;
    [SerializeField] private Button spinButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private Image[] spinlamps;

    [SerializeField] private HeroSelectionRotator heroSelection;
    [SerializeField] private WheelManager wheelManager;

    private float buttonCooldownTime = 0.15f;

    private void Awake()
    {
        figureSquareSelectLeftButton.onClick.AddListener(() =>
        {
            heroSelection.RotateLeft("Square");
            StartCoroutine(ButtonCooldown(figureSquareSelectLeftButton));
        });

        figureSquareSelectRightButton.onClick.AddListener(() =>
        {
            heroSelection.RotateRight("Square");
            StartCoroutine(ButtonCooldown(figureSquareSelectRightButton));
        });

        figureDiamondSelectLeftButton.onClick.AddListener(() =>
        {
            heroSelection.RotateLeft("Diamond");
            StartCoroutine(ButtonCooldown(figureDiamondSelectLeftButton));
        });

        figureDiamondSelectRightButton.onClick.AddListener(() =>
        {
            heroSelection.RotateRight("Diamond");
            StartCoroutine(ButtonCooldown(figureDiamondSelectRightButton));
        });


        spinButton.onClick.AddListener(() =>
        {
            wheelManager.SpinAllWheels();
        });

        readyButton.onClick.AddListener(() =>
        {
            DeactivateAllButtons();
            MultiplayerGameManager.Instance.SetLocalPlayerReadyToPlay(true);
        });
    }

    private void Start()
    {
        InitialHeroSetting.Instance.OnDeactivateReadyButton += InitialHeroSetting_OnDeactivateRotatorButtons;
    }

    private void InitialHeroSetting_OnDeactivateRotatorButtons(object sender, System.EventArgs e)
    {
        readyButton.gameObject.SetActive(false);
    }

    public void SetManagers(HeroSelectionRotator hsl, WheelManager wm)
    {
        heroSelection = hsl;
        wheelManager = wm;

        wm.SetUIElements(spinButton.gameObject, spinlamps);
    }

    private void DeactivateAllButtons()
    {
        figureSquareSelectLeftButton.gameObject.SetActive(false);
        figureSquareSelectRightButton.gameObject.SetActive(false);
        figureDiamondSelectLeftButton.gameObject.SetActive(false);
        figureDiamondSelectRightButton.gameObject.SetActive(false);
    }

    //Methode, damit man die Buttons nicht zu schnell klicken kann, um so zu verhindern, dass die Spawns der Helden nicht richtig fertiggestellt wird
    private IEnumerator ButtonCooldown(Button clickedButton)
    {
        //Deaktiviere den Button
        clickedButton.interactable = false;

        //Wartezeit
        yield return new WaitForSeconds(buttonCooldownTime);

        //Reaktiviere den Button
        clickedButton.interactable = true;
    }
}
