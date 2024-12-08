using UnityEngine;

public class EnemyScriptDebug : MonoBehaviour
{
    [SerializeField] private HeroActions heroActionsSquare;
    [SerializeField] private HeroActions heroActionsDiamond;
    [SerializeField] private EvaluationManager evaluationManager;
    [SerializeField] private HeroSelectionRotator selectionRotator;
    [SerializeField] private WheelManager wheelManager;

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
}
