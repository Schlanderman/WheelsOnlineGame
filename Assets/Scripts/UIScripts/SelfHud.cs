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

    private void Awake()
    {
        figureSquareSelectLeftButton.onClick.AddListener(() =>
        {
            heroSelection.RotateLeft("Square");
        });

        figureSquareSelectRightButton.onClick.AddListener(() =>
        {
            heroSelection.RotateRight("Square");
        });

        figureDiamondSelectLeftButton.onClick.AddListener(() =>
        {
            heroSelection.RotateLeft("Diamond");
        });

        figureDiamondSelectRightButton.onClick.AddListener(() =>
        {
            heroSelection.RotateRight("Diamond");
        });


        spinButton.onClick.AddListener(() =>
        {
            wheelManager.SpinAllWheels();
        });

        readyButton.onClick.AddListener(() =>
        {
            //InitialHeroSetting.Instance.ChangePlayerReadyness(PlayerScript.Instance.playerId, true);
            MultiplayerGameManager.Instance.SetLocalPlayerReadyToPlay(true);
        });
    }

    private void Start()
    {
        InitialHeroSetting.Instance.OnDeactivateReadyButton += InitialHeroSetting_OnDeactivateRotatorButtons;
    }

    private void InitialHeroSetting_OnDeactivateRotatorButtons(object sender, System.EventArgs e)
    {
        figureSquareSelectLeftButton.gameObject.SetActive(false);
        figureSquareSelectRightButton.gameObject.SetActive(false);
        figureDiamondSelectLeftButton.gameObject.SetActive(false);
        figureDiamondSelectRightButton.gameObject.SetActive(false);
        readyButton.gameObject.SetActive(false);
    }

    public void SetManagers(HeroSelectionRotator hsl, WheelManager wm)
    {
        heroSelection = hsl;
        wheelManager = wm;

        wm.SetUIElements(spinButton.gameObject, spinlamps);
    }
}
