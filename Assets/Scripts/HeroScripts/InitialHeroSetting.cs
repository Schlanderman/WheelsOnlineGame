using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class InitialHeroSetting : NetworkBehaviour
{
    public static InitialHeroSetting Instance { get; private set; }

    [SerializeField] private HeroActions playerOneSquareActions;
    [SerializeField] private HeroActions playerOneDiamondActions;
    [SerializeField] private HeroActions playerTwoSquareActions;
    [SerializeField] private HeroActions playerTwoDiamondActions;

    [SerializeField] private EvaluationManager playerOneEvaluationManager;
    [SerializeField] private EvaluationManager playerTwoEvaluationManager;

    [SerializeField] private Hero playerOneSquareHero;
    [SerializeField] private Hero playerOneDiamondHero;
    [SerializeField] private Hero playerTwoSquareHero;
    [SerializeField] private Hero playerTwoDiamondHero;

    private Dictionary<ulong, (HeroActions, HeroActions, EvaluationManager)> playerComponentsDictionary;
    private Dictionary<ulong, (Hero, Hero)> playerHeroesDictionary;

    //Events
    public event EventHandler OnActivateSpinButton;
    public event EventHandler OnSetCoverUp;
    public event EventHandler OnInitializePlayerReadyness;
    public event EventHandler OnDeactivateReadyButton;
    public event Action<ulong> OnDeactivateRotatorButtons;
    public event Action<ulong, Hero, Hero, Hero, Hero> OnSetHeroesInitially;
    public event EventHandler OnGetHeroes;
    public event EventHandler OnGetPlayerComponents;
    public event Action<ulong, CrownManager, BulwarkMover, ActionRodAnimManager> OnSetEnemyManagers;

    //Multiplayer Events
    public event Action<Hero, Hero, Hero, Hero> OnSetMultiplayerHeroesInitially;

    public void Awake()
    {
        Instance = this;

        playerComponentsDictionary = new Dictionary<ulong, (HeroActions, HeroActions, EvaluationManager)>();
        playerHeroesDictionary = new Dictionary<ulong, (Hero, Hero)>();
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

    private void InitializeEverythingAlt()
    {
        OnGetPlayerComponents?.Invoke(this, EventArgs.Empty);   //Bekommt HeroActions + EvaluationManager von PlayerScript
        OnGetHeroes?.Invoke(this, EventArgs.Empty);     //Heroes in diesen Manager laden

        //Heroes setzen
        //OnSetHeroesInitially?.Invoke(playerOneId, playerOneSquareHero, playerOneDiamondHero, playerTwoSquareHero, playerTwoDiamondHero);
        //OnSetHeroesInitially?.Invoke(playerTwoId, playerTwoSquareHero, playerTwoDiamondHero, playerOneSquareHero, playerOneDiamondHero);
        //OnSetMultiplayerHeroesInitially?.Invoke(playerOneSquareHero, playerOneDiamondHero, playerTwoSquareHero, playerTwoDiamondHero);

        //Alle Heroes mit HeroActions befüllen
        playerOneSquareHero.SetHeroActions(playerOneSquareActions);
        playerOneDiamondHero.SetHeroActions(playerOneDiamondActions);
        playerTwoSquareHero.SetHeroActions(playerTwoSquareActions);
        playerTwoDiamondHero.SetHeroActions(playerTwoDiamondActions);

        //EvaluationManager befüllen
        playerOneEvaluationManager.SetHeroes(playerOneSquareHero, playerOneDiamondHero);
        playerTwoEvaluationManager.SetHeroes(playerTwoSquareHero, playerTwoDiamondHero);

        //Turnmanager befüllen
        TurnManager.Instance.SetHeroes(playerOneSquareHero, playerOneDiamondHero, playerTwoSquareHero, playerTwoDiamondHero);

        //Cover hochstellen
        OnSetCoverUp?.Invoke(this, EventArgs.Empty);
    }

    private void InitializeEverything()
    {
        Debug.Log("Alle sind bereit, Spiel wird gestartet!");
        OnGetPlayerComponents?.Invoke(this, EventArgs.Empty);   //Bekommt HeroActions + EvaluationManager von PlayerScript
        OnGetHeroes?.Invoke(this, EventArgs.Empty);     //Heroes in diesen Manager laden
    }

    public IEnumerator InitializeReadynessLate()
    {
        yield return new WaitForEndOfFrame();

        OnInitializePlayerReadyness?.Invoke(this, EventArgs.Empty);
    }

    public void SetPlayerHeroesAlt(ulong playerId, Hero square, Hero diamond)
    {
        playerHeroesDictionary[playerId] = (square, diamond);

        PopulateHeroComponents();
    }

    private void PopulateHeroComponents()
    {
        //Nichts machen wenn nicht beide Spieler vorhanden sind
        if (playerHeroesDictionary.Count != 2) { return; }

        //Dicitonary in Arrays umwandeln, um die einzelnen Komponenten auszulesen
        ulong[] playerIds = playerHeroesDictionary.Keys.ToArray();
        var playerHeroes = playerHeroesDictionary.Values.ToArray();

        //Der erste eintrag im Dictionary gehört zu Spieler 1, ansonsten der erste Eintrag um Dictionary gehört zu Spieler 2
        //if (playerIds[0] == playerOneId)
        //{
        //    playerOneSquareHero = playerHeroes[0].Item1;
        //    playerOneDiamondHero = playerHeroes[0].Item2;
        //    playerTwoSquareHero = playerHeroes[1].Item1;
        //    playerTwoDiamondHero = playerHeroes[1].Item2;
        //}
        //else
        //{
        //    playerOneSquareHero = playerHeroes[1].Item1;
        //    playerOneDiamondHero = playerHeroes[1].Item2;
        //    playerTwoSquareHero = playerHeroes[0].Item1;
        //    playerTwoDiamondHero = playerHeroes[0].Item2;
        //}
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
            //Wenn ja, dann werden die gegnerischen Helden gesetzt
            playerTwoSquareActions = playerReference.GetSquareHeroActions();
            playerTwoDiamondActions = playerReference.GetDiamondHeroActions();
            playerTwoEvaluationManager = playerReference.GetEvaluationManager();
        }
        else
        {
            //Wenn nicht, dann werden die eigenen Helden gesetzt
            playerOneSquareActions = playerReference.GetSquareHeroActions();
            playerOneDiamondActions = playerReference.GetDiamondHeroActions();
            playerOneEvaluationManager = playerReference.GetEvaluationManager();
        }
    }

    private void PopulatePlayerComponents()
    {
        //Nichts machen wenn nicht beide Spieler vorhanden sind
        if (playerComponentsDictionary.Count != 2) { return; }

        //Dictionary in Arrays umwandeln, um die einzelnen Komponenten auszulesen
        ulong[] playerIds = playerComponentsDictionary.Keys.ToArray();
        var playerComponents = playerComponentsDictionary.Values.ToArray();

        //Zuweisung der einzelnen Komponenten
        //Der erste eintrag im Dictionary gehört zu Spieler 1, ansonsten der erste Eintrag um Dictionary gehört zu Spieler 2
        //if (playerIds[0] == playerOneId)
        //{
        //    playerOneSquareActions = playerComponents[0].Item1;
        //    playerOneDiamondActions = playerComponents[0].Item2;
        //    playerOneEvaluationManager = playerComponents[0].Item3;

        //    playerTwoSquareActions = playerComponents[1].Item1;
        //    playerTwoDiamondActions = playerComponents[1].Item2;
        //    playerTwoEvaluationManager = playerComponents[1].Item3;
        //}
        //else
        //{
        //    playerOneSquareActions = playerComponents[1].Item1;
        //    playerOneDiamondActions = playerComponents[1].Item2;
        //    playerOneEvaluationManager = playerComponents[1].Item3;

        //    playerTwoSquareActions = playerComponents[0].Item1;
        //    playerTwoDiamondActions = playerComponents[0].Item2;
        //    playerTwoEvaluationManager = playerComponents[0].Item3;
        //}
    }

    public void SetEnemyManagers(ulong playerId, CrownManager enemyCrownManager, BulwarkMover enemyBulwarkMover, ActionRodAnimManager enemyActionRodManager)
    {
        OnSetEnemyManagers?.Invoke(playerId, enemyCrownManager, enemyBulwarkMover, enemyActionRodManager);
    }
}
