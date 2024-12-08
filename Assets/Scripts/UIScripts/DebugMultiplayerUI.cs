using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class DebugMultiplayerUI : MonoBehaviour
{
    [SerializeField] private Button startOfflineButton;
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform gameBoard;

    [SerializeField] private SelfHud selfHud;
    [SerializeField] private EnemyHudDebug enemyHud;
    [SerializeField] private HPScripts hpScripts;

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
        GameObject newPlayer = Instantiate(playerPrefab, gameBoard, true);
        GameObject newEnemy = Instantiate(enemyPrefab, gameBoard, true);
        selfHud.SetManagers(newPlayer.GetComponent<PlayerScript>().GetSelectionRotator(), newPlayer.GetComponent<PlayerScript>().GetWheelManager());
        enemyHud.SetManagers(newEnemy.GetComponent<EnemyScriptDebug>().GetSelectionRotator(), newEnemy.GetComponent<EnemyScriptDebug>().GetWheelManager());
        enemyHud.SetEnemyScript(newEnemy.GetComponent<EnemyScriptDebug>());

        PlayerScript.Instance.SetHPScripts(hpScripts);

        StartCoroutine(TurnManager.Instance.InitializeReadynessLate());
        StartCoroutine(TurnManager.Instance.InitializeCrownHPLate());
        StartCoroutine(InitialHeroSetting.Instance.InitializeReadynessLate());
    }
}
