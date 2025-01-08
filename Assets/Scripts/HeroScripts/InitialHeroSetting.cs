using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class InitialHeroSetting : NetworkBehaviour
{
    public static InitialHeroSetting Instance { get; private set; }

    private EvaluationManager playerOneEvaluationManager;
    private EvaluationManager playerTwoEvaluationManager;

    private Hero playerOneSquareHero;
    private Hero playerOneDiamondHero;
    private Hero playerTwoSquareHero;
    private Hero playerTwoDiamondHero;

    //Events
    public event EventHandler OnActivateSpinButton;
    public event EventHandler OnSetCoverUp;
    public event EventHandler OnDeactivateReadyButton;
    public event EventHandler OnGetHeroes;
    public event EventHandler OnGetPlayerComponents;
    public event Action<ulong, CrownManager, BulwarkMover, ActionRodAnimManager> OnSetEnemyManagers;

    //Multiplayer Events
    public event Action<Hero, Hero, Hero, Hero> OnSetMultiplayerHeroesInitially;

    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        MultiplayerGameManager.Instance.OnPlayersAreReadyToPlay += MultiplayerGameManager_OnPlayersAreReadyToPlay;
    }

    private void MultiplayerGameManager_OnPlayersAreReadyToPlay(object sender, EventArgs e)
    {
        PlayersAreReadyToPlayRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void PlayersAreReadyToPlayRpc()
    {
        //ReadyDeaktivieren, Spin aktivieren
        OnDeactivateReadyButton?.Invoke(this, EventArgs.Empty);
        OnActivateSpinButton?.Invoke(this, EventArgs.Empty);

        InitializeEverything();
    }

    private void InitializeEverything()
    {
        Debug.Log("Alle sind bereit, Spiel wird gestartet!");
        OnGetPlayerComponents?.Invoke(this, EventArgs.Empty);   //Bekommt HeroActions + EvaluationManager von PlayerScript
        OnGetHeroes?.Invoke(this, EventArgs.Empty);     //Heroes in diesen Manager laden
        SetHeroesOnManagersRpc();     //Heroes in den EvaluationManager und TurnManager laden (auf beiden Instanzen)
    }

    public IEnumerator InitializeReadynessLate()
    {
        //Macht nix mehr, kann entfernt werden
        yield return new WaitForEndOfFrame();
    }

    public void SetPlayerHeroes(Hero square, Hero diamond)
    {
        SetMultiplayerPlayerHeroesRpc(square.GetComponent<NetworkObject>(), diamond.GetComponent<NetworkObject>());
    }

    [Rpc(SendTo.Everyone)]
    private void SetMultiplayerPlayerHeroesRpc(NetworkObjectReference squareNetworkObjectReference, NetworkObjectReference diamondNetworkObjectReference, RpcParams rpcParams = default)
    {
        if (!squareNetworkObjectReference.TryGet(out NetworkObject squareNetworkObject))
        {
            Debug.LogWarning($"'{squareNetworkObjectReference}' enthält kein NetworkObject!");
            return;
        }
        if (!diamondNetworkObjectReference.TryGet(out NetworkObject diamondNetworkObject))
        {
            Debug.LogWarning($"'{diamondNetworkObjectReference}' enthält kein NetworkObject!");
            return;
        }

        Hero square = squareNetworkObject.GetComponent<Hero>();
        Hero diamond = diamondNetworkObject.GetComponent<Hero>();

        //Testen, ob die SenderId mit der lokalen ID übereinstimmt
        if (rpcParams.Receive.SenderClientId == NetworkManager.Singleton.LocalClientId)
        {
            //Wenn die IDs übereinstimmen, dann sind das die Helden vom lokalen Spieler und werden PlayerOne zugewiesen
            playerOneSquareHero = square;
            playerOneDiamondHero = diamond;
        }
        else
        {
            //Wenn die IDs nicht übereinstimmen, dann sind das die Helden des Gegners und werden PlayerTwo zugewiesen
            playerTwoSquareHero = square;
            playerTwoDiamondHero = diamond;
        }

        //Nur der Server und das Script, das Spieler 1 entspricht, kann ab hier etwas machen
        if (!IsServer || rpcParams.Receive.SenderClientId == NetworkManager.Singleton.LocalClientId) { return; }

        //Heroes in HeroActions setzen
        OnSetMultiplayerHeroesInitially?.Invoke(playerOneSquareHero, playerOneDiamondHero, playerTwoSquareHero, playerTwoDiamondHero);
    }

    public void SetMultiplayerPlayerComponents(PlayerScript playerScriptObject)
    {
        SetMultiplayerPlayerComponentsRpc(playerScriptObject.GetComponent<NetworkObject>());
    }

    [Rpc(SendTo.Everyone)]
    private void SetMultiplayerPlayerComponentsRpc(NetworkObjectReference playerNetworkObjectReference, RpcParams rpcParams = default)
    {
        if (!playerNetworkObjectReference.TryGet(out NetworkObject playerNetworkObject))
        {
            Debug.LogWarning($"'{playerNetworkObjectReference}' enthält kein NetworkObject!");
            return;
        }

        PlayerScript playerReference = playerNetworkObject.GetComponent<PlayerScript>();

        //Testen, ob die SenderId mit der lokalen ID übereinstimmt
        if (rpcParams.Receive.SenderClientId == NetworkManager.Singleton.LocalClientId)
        {
            playerTwoEvaluationManager = playerReference.GetEvaluationManager();
        }
        else
        {
            playerOneEvaluationManager = playerReference.GetEvaluationManager();
        }
    }

    [Rpc(SendTo.Everyone)]
    private void SetHeroesOnManagersRpc()
    {
        StartCoroutine(SetHeroesOnManagersLate());
    }

    private IEnumerator SetHeroesOnManagersLate()
    {
        //Warten, bis alle komponenten befüllt sind, erst dann alles zuweisen
        while (playerOneSquareHero == null || playerTwoDiamondHero == null || playerTwoSquareHero == null || playerTwoDiamondHero == null)
        {
            yield return null;
        }

        //EvaluationManager und TurnManager befüllen
        playerOneEvaluationManager.SetHeroes(playerOneSquareHero, playerOneDiamondHero);
        playerTwoEvaluationManager.SetHeroes(playerTwoSquareHero, playerTwoDiamondHero);

        TurnManager.Instance.SetHeroes(playerOneSquareHero, playerOneDiamondHero, playerTwoSquareHero, playerTwoDiamondHero);

        //Hier wurde alles zugewiesen, also kann das Cover hochgestellt werden
        OnSetCoverUp?.Invoke(this, EventArgs.Empty);
    }

    public void SetEnemyManagers(ulong playerId, CrownManager enemyCrownManager, BulwarkMover enemyBulwarkMover, ActionRodAnimManager enemyActionRodManager)
    {
        OnSetEnemyManagers?.Invoke(playerId, enemyCrownManager, enemyBulwarkMover, enemyActionRodManager);
    }
}
