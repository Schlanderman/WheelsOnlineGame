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

    private HPScripts hpScripts;

    public ulong playerId = 0;

    private void Awake()
    {
        Instance = this;
    }

    //public override void OnNetworkSpawn()
    //{
    //    playerId = NetworkManager.Singleton.LocalClientId;
    //}

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

    public HPScripts GetHPScripts()
    {
        return hpScripts;
    }

    public void SetHPScripts(HPScripts script)
    {
        hpScripts = script;
    }
}
