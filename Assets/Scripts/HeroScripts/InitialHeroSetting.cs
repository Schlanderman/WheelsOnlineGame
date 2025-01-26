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
    public event EventHandler OnInitializeCrownHP;
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
        OnInitializeCrownHP?.Invoke(this, EventArgs.Empty);     //CrownHP an den TurnManager senden und damit initialisieren
        SetHeroesOnManagersRpc();     //Heroes in den EvaluationManager und TurnManager laden (auf beiden Instanzen)
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
            //Wenn die IDs übereinstimmen, dann sind das die Helden vom gengerischen Spieler und werden PlayerTwo zugewiesen
            playerTwoSquareHero = square;
            playerTwoDiamondHero = diamond;
        }
        else
        {
            //Wenn die IDs nicht übereinstimmen, dann sind das die eigenen Helden und werden PlayerOne zugewiesen
            playerOneSquareHero = square;
            playerOneDiamondHero = diamond;
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
            playerOneEvaluationManager = playerReference.GetEvaluationManager();
        }
        else
        {
            playerTwoEvaluationManager = playerReference.GetEvaluationManager();
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

        if (IsServer)
        {
            SetHeroesToTurnManagerRpc(
                playerOneSquareHero.GetComponent<NetworkObject>(),
                playerOneDiamondHero.GetComponent<NetworkObject>(),
                playerTwoSquareHero.GetComponent<NetworkObject>(),
                playerTwoDiamondHero.GetComponent<NetworkObject>());
        }

        //Hier wurde alles zugewiesen, also kann das Cover hochgestellt werden
        OnSetCoverUp?.Invoke(this, EventArgs.Empty);
    }

    //Die Helden nur vom Server aus übertragen, damit bei beiden Turnmanagern die Helden gleich gesetzt sind
    [Rpc(SendTo.Everyone)]
    private void SetHeroesToTurnManagerRpc(NetworkObjectReference p1SquareHeroNOReference,NetworkObjectReference p1DiamondHeroNOReference,
        NetworkObjectReference p2SquareHeroNOReference, NetworkObjectReference p2DiamondHeroNOReference)
    {
        if (!p1SquareHeroNOReference.TryGet(out NetworkObject p1SquareHeroNO))
        {
            Debug.LogError($"Konnte keine NetworkObject-Komponente in {p1SquareHeroNOReference} finden!");
            return;
        }
        if (!p1DiamondHeroNOReference.TryGet(out NetworkObject p1DiamondHeroNO))
        {
            Debug.LogError($"Konnte keine NetworkObject-Komponente in {p1DiamondHeroNOReference} finden!");
            return;
        }
        if (!p2SquareHeroNOReference.TryGet(out NetworkObject p2SquareHeroNO))
        {
            Debug.LogError($"Konnte keine NetworkObject-Komponente in {p2SquareHeroNOReference} finden!");
            return;
        }
        if (!p2DiamondHeroNOReference.TryGet(out NetworkObject p2DiamondHeroNO))
        {
            Debug.LogError($"Konnte keine NetworkObject-Komponente in {p2DiamondHeroNOReference} finden!");
            return;
        }

        Hero p1SquareHero = p1SquareHeroNO.GetComponent<Hero>();
        Hero p1DiamondHero = p1DiamondHeroNO.GetComponent<Hero>();
        Hero p2SquareHero = p2SquareHeroNO.GetComponent<Hero>();
        Hero p2DiamondHero = p2DiamondHeroNO.GetComponent<Hero>();

        TurnManager.Instance.SetHeroes(p1SquareHero, p1DiamondHero, p2SquareHero, p2DiamondHero);
    }

    public void SetEnemyManagers(ulong playerId, CrownManager enemyCrownManager, BulwarkMover enemyBulwarkMover, ActionRodAnimManager enemyActionRodManager)
    {
        OnSetEnemyManagers?.Invoke(playerId, enemyCrownManager, enemyBulwarkMover, enemyActionRodManager);
    }
}
