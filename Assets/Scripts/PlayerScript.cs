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

    private HPScripts hpScriptsSelf;
    private HPScripts hpScriptsEnemy;

    public ulong playerId = 0;

    private void Awake()
    {
        Instance = this;
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
        playerId = newId;
    }
}
