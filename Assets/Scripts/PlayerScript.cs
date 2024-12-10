using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{
    public static PlayerScript Instance { get; private set; }

    [SerializeField] private HeroActions heroActionsSquare;
    [SerializeField] private HeroActions heroActionsDiamond;
    [SerializeField] private EvaluationManager evaluationManager;
    [SerializeField] private HeroSelectionRotator selectionRotator;
    [SerializeField] private WheelManager wheelManager;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform playerTwoCameraPosition;

    private HPScripts hpScriptsSelf;
    private HPScripts hpScriptsEnemy;

    [SerializeField] private ulong _playerId = 0;

    public ulong playerId => _playerId;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        _playerId = OwnerClientId;

        mainCamera = FindAnyObjectByType<Camera>();
        playerTwoCameraPosition = GameObject.FindGameObjectWithTag("CameraPositionPlayerTwo").GetComponent<Transform>();

        if (!IsServer)
        {
            mainCamera.gameObject.transform.position = playerTwoCameraPosition.position;
        }
        MultiplayerGamaManager.Instance.OnGetPlayerScriptWithId += MultiplayerGameManager_OnGetPlayerScriptWithId;
    }

    private void MultiplayerGameManager_OnGetPlayerScriptWithId(ulong clientId)
    {
        if (clientId == playerId)
        {
            MultiplayerGamaManager.Instance.SetPlayerScriptFromClientId(playerId, this);
        }
    }

    public HeroActions GetSquareHeroActions()
    {
        return heroActionsSquare;
    }

    public HeroActions GetDiamondHeroActions()
    {
        return heroActionsDiamond;
    }

    public EvaluationManager GetEvaluationManager()
    {
        return evaluationManager;
    }

    public HeroSelectionRotator GetSelectionRotator()
    {
        return selectionRotator;
    }

    public WheelManager GetWheelManager()
    {
        return wheelManager;
    }

    public HPScripts GetHPScriptsSelf()
    {
        return hpScriptsSelf;
    }

    public HPScripts GetHPScriptsEnemy()
    {
        return hpScriptsEnemy;
    }

    public void SetHPScripts(HPScripts scriptSelf, HPScripts scriptEnemy)
    {
        hpScriptsSelf = scriptSelf;
        hpScriptsEnemy = scriptEnemy;
    }

    public void SetPlayerId(ulong newId)
    {
        _playerId = newId;
    }
}
