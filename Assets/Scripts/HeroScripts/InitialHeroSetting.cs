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
        //ReadyDeaktivieren, Spin aktivieren
        OnDeactivateReadyButton?.Invoke(this, EventArgs.Empty);
        OnActivateSpinButton?.Invoke(this, EventArgs.Empty);

        InitializeEverythingServerRPC();
    }

    private void InitializeEverything()
    {
        OnGetPlayerComponents?.Invoke(this, EventArgs.Empty);
        OnGetHeroes?.Invoke(this, EventArgs.Empty);

        //Heroes setzen
        //OnSetHeroesInitially?.Invoke(playerOneId, playerOneSquareHero, playerOneDiamondHero, playerTwoSquareHero, playerTwoDiamondHero);
        //OnSetHeroesInitially?.Invoke(playerTwoId, playerTwoSquareHero, playerTwoDiamondHero, playerOneSquareHero, playerOneDiamondHero);

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

    [ServerRpc(RequireOwnership = true)]
    private void InitializeEverythingServerRPC()
    {
        
    }

    public IEnumerator InitializeReadynessLate()
    {
        yield return new WaitForEndOfFrame();

        OnInitializePlayerReadyness?.Invoke(this, EventArgs.Empty);
    }

    public void SetPlayerHeroes(ulong playerId, Hero square, Hero diamond)
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

    public void SetPlayerComponents(ulong playerId, HeroActions squareActions, HeroActions diamondActions, EvaluationManager evalManager)
    {
        playerComponentsDictionary[playerId] = (squareActions, diamondActions, evalManager);

        PopulatePlayerComponents();
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
