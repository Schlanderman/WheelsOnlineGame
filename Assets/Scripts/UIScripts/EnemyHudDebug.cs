using UnityEngine;
using UnityEngine.UI;

public class EnemyHudDebug : MonoBehaviour
{
    [SerializeField] private EnemyScript enemyScript;   //Offline only

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
            //InitialHeroSetting.Instance.ChangePlayerReadyness(enemyScript.playerId, true);
            //InitialHeroSetting.Instance.CheckReady();
        });
    }

    private void Start()
    {
        //InitialHeroSetting.Instance.OnDeactivateRotatorButtons += InitialHeroSetting_OnDeactivateRotatorButtons;
        InitialHeroSetting.Instance.OnDeactivateReadyButton += InitialHeroSetting_OnDeactivateRotatorButtons;
    }

    private void InitialHeroSetting_OnDeactivateRotatorButtons(ulong playerId)
    {
        if (playerId != enemyScript.playerId)
        {
            return;
        }

        figureSquareSelectLeftButton.gameObject.SetActive(false);
        figureSquareSelectRightButton.gameObject.SetActive(false);
        figureDiamondSelectLeftButton.gameObject.SetActive(false);
        figureDiamondSelectRightButton.gameObject.SetActive(false);
    }

    private void InitialHeroSetting_OnDeactivateRotatorButtons(object sender, System.EventArgs e)
    {
        readyButton.gameObject.SetActive(false);
    }

    public void SetManagers(HeroSelectionRotator hsr, WheelManager wm)
    {
        heroSelection = hsr;
        wheelManager = wm;

        wm.SetUIElements(spinButton.gameObject, spinlamps);
    }

    public void SetEnemyScript(EnemyScript es)
    {
        enemyScript = es;
    }
}
