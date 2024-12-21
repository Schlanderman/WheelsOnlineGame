using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class HeroSpawnDummy : NetworkBehaviour
{
    private const string PLAYER_SQUARE_HERO_SPAWN_KEYWORD = "PlayerSquareHeroSpawn";
    private const string PLAYER_DIAMOND_HERO_SPAWN_KEYWORD = "PlayerDiamondHeroSpawn";
    private const string ENEMY_SQUARE_HERO_SPAWN_KEYWORD = "EnemySquareHeroSpawn";
    private const string ENEMY_DIAMOND_HERO_SPAWN_KEYWORD = "EnemyDiamondHeroSpawn";

    private const string PATH_TO_PLAYER_SQUARE_HERO_SPAWN = "FigureCurbSelfSquareSpawn/FigureCurbSelfSquare/FigureStandSquare/SquareHeroSpawn";
    private const string PATH_TO_PLAYER_DIAMOND_HERO_SPAWN = "FigureCurbSelfDiamondSpawn/FigureCurbSelfDiamond/FigureStandDiamond/DiamondHeroSpawn";
    private const string PATH_TO_ENEMY_SQUARE_HERO_SPAWN = "FigureCurbCopySquareSpawn/FigureCurbCopySquare/FigureStandSquare/SquareHeroSpawn";
    private const string PATH_TO_ENEMY_DIAMOND_HERO_SPAWN = "FigureCurbCopyDiamondSpawn/FigureCurbCopyDiamond/FigureStandDiamond/DiamondHeroSpawn";


    public enum PlayerSideKey { Player, Enemy }
    public enum HeroSideKey { Square, Diamond }

    public PlayerSideKey playerSide { get; private set; }
    public HeroSideKey heroSideKey { get; private set; }

    public override void OnNetworkSpawn()
    {
        //playerSide = PlayerSideKey.Player;
        //heroSideKey = HeroSideKey.Square;
        //UpdateTagForPosition();
    }

    public void SetPositionForHeroSpawn(PlayerSideKey newPlayerSide, HeroSideKey newHeroSide, GameObject parentTransformRoot)
    {
        playerSide = newPlayerSide;
        heroSideKey = newHeroSide;
        UpdateTagForPosition();

        Transform heroObjectParentTransform = GetTransformFromRoot(parentTransformRoot);

        if (heroObjectParentTransform != null)
        {
            transform.position = heroObjectParentTransform.position;
            transform.rotation = heroObjectParentTransform.rotation;
        }
        else
        {
            Debug.LogError($"Objekt '{heroObjectParentTransform}' konnte nicht gefunden werden.");
        }
    }

    public void SetParentForHeroSpawn(GameObject parentTransformRoot)
    {
        transform.parent = parentTransformRoot.transform;
    }

    private Transform GetTransformFromRoot(GameObject root)
    {
        Transform outputTransform = null;

        switch (gameObject.tag)
        {
            case PLAYER_SQUARE_HERO_SPAWN_KEYWORD:
                outputTransform = root.transform.Find(PATH_TO_PLAYER_SQUARE_HERO_SPAWN);
                break;
            case PLAYER_DIAMOND_HERO_SPAWN_KEYWORD:
                outputTransform = root.transform.Find(PATH_TO_PLAYER_DIAMOND_HERO_SPAWN);
                break;
            case ENEMY_SQUARE_HERO_SPAWN_KEYWORD:
                outputTransform = root.transform.Find(PATH_TO_ENEMY_SQUARE_HERO_SPAWN);
                break;
            case ENEMY_DIAMOND_HERO_SPAWN_KEYWORD:
                outputTransform = root.transform.Find(PATH_TO_ENEMY_DIAMOND_HERO_SPAWN);
                break;
            default:
                Debug.LogError($"Tag '{gameObject.tag}' konnte nicht gefunden werden.");
                break;
        }

        return outputTransform;
    }

    private void UpdateTagForPosition()
    {
        if (playerSide == PlayerSideKey.Player)
        {
            if (heroSideKey == HeroSideKey.Square)
            {
                gameObject.tag = PLAYER_SQUARE_HERO_SPAWN_KEYWORD;
            }
            else
            {
                gameObject.tag = PLAYER_DIAMOND_HERO_SPAWN_KEYWORD;
            }
        }
        else
        {
            if (heroSideKey == HeroSideKey.Square)
            {
                gameObject.tag = ENEMY_SQUARE_HERO_SPAWN_KEYWORD;
            }
            else
            {
                gameObject.tag = ENEMY_DIAMOND_HERO_SPAWN_KEYWORD;
            }
        }
    }
}
