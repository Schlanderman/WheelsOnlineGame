using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private HeroActions heroActionsSquare;
    [SerializeField] private HeroActions heroActionsDiamond;
    [SerializeField] private EvaluationManager evaluationManager;
    [SerializeField] private HeroSelectionRotator selectionRotator;
    [SerializeField] private WheelManager wheelManager;

    private HPScripts hpScriptsSelf;
    private HPScripts hpScriptsEnemy;

    public ulong playerId;

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
