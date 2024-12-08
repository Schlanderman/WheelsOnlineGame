using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class DebugMultiplayerUI : MonoBehaviour
{
    [SerializeField] private Button startOfflineButton;
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;

    //Spieler Prefabs
    [SerializeField] private GameObject playerOnePrefab;
    [SerializeField] private GameObject playerTwoPrefab;

    //Copyer Prefab
    [SerializeField] private GameObject actionsCopyer;


    [SerializeField] private Transform gameBoardPlayerOne;
    [SerializeField] private Transform gameBoardPlayerTwo;

    [SerializeField] private SelfHud selfHud;
    [SerializeField] private EnemyHudDebug enemyHud;
    [SerializeField] private HPScripts hpScriptsPlayerOne;
    [SerializeField] private HPScripts hpScriptsPlayerTwo;

    private void Awake()
    {
        startOfflineButton.onClick.AddListener(() =>
        {
            ActivatePlayers();
        });

        startHostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });

        startClientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }

    public void ActivatePlayers()
    {
        //Spielerobjekte Spawnen
        GameObject newPlayerOne = Instantiate(playerOnePrefab, gameBoardPlayerOne, false);
        GameObject newPlayerTwo = Instantiate(playerTwoPrefab, gameBoardPlayerTwo, false);
        selfHud.SetManagers(newPlayerOne.GetComponent<PlayerScript>().GetSelectionRotator(), newPlayerOne.GetComponent<PlayerScript>().GetWheelManager());
        enemyHud.SetManagers(newPlayerTwo.GetComponent<EnemyScript>().GetSelectionRotator(), newPlayerTwo.GetComponent<EnemyScript>().GetWheelManager());
        enemyHud.SetEnemyScript(newPlayerTwo.GetComponent<EnemyScript>());

        newPlayerOne.GetComponent<PlayerScript>().SetHPScripts(hpScriptsPlayerOne, hpScriptsPlayerTwo);
        newPlayerTwo.GetComponent<EnemyScript>().SetHPScripts(hpScriptsPlayerTwo, hpScriptsPlayerOne);

        StartCoroutine(TurnManager.Instance.InitializeReadynessLate());
        StartCoroutine(TurnManager.Instance.InitializeCrownHPLate());
        StartCoroutine(InitialHeroSetting.Instance.InitializeReadynessLate());
    }
}
