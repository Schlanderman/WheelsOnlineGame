using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{
    public static event EventHandler OnAnyPlayerSpawned;

    public static void ResetStaticData()
    {
        OnAnyPlayerSpawned = null;
    }

    public static PlayerScript LocalInstance { get; private set; }

    [SerializeField] private HeroActions heroActionsSquare;
    [SerializeField] private HeroActions heroActionsDiamond;
    [SerializeField] private EvaluationManager evaluationManager;
    [SerializeField] private HeroSelectionRotator selectionRotator;
    [SerializeField] private WheelManager wheelManager;
    [SerializeField] private GameObject heroSpawnPosition;
    [SerializeField] private GameObject playerUIElements;

    [SerializeField] private GameObject[] gameboardPositions;
    private Camera mainCamera;
    private GameObject[] cameraPositions;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
            SpawnParentsForHeroesRpc();
        }

        OnAnyPlayerSpawned?.Invoke(this, new EventArgs());
        //(Wichtiger Code bei 1:17:29!!!)

        mainCamera = Camera.main;
        cameraPositions = GameObject.FindGameObjectsWithTag("CameraPositions");
        gameboardPositions = GameObject.FindGameObjectsWithTag("GameboardPositions");
        transform.position = gameboardPositions[(int)OwnerClientId].transform.position;

        //Wenn der erste Spieler ist, dann Kameraposition beibehalten
        if (IsServer) { return; }

        mainCamera.transform.position = cameraPositions[1].transform.position;
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

    public void ChangePlayerUIElements(bool state)
    {
        if (IsOwner)
        {
            playerUIElements.SetActive(state);
        }
    }



    [Rpc(SendTo.Server)]
    private void SpawnParentsForHeroesRpc()
    {
        GameObject heroSpawnPositionSquare = Instantiate(heroSpawnPosition);
        GameObject heroSpawnPositionDiamond = Instantiate(heroSpawnPosition);

        heroSpawnPositionSquare.GetComponent<HeroSpawnDummy>().SetPositionForHeroSpawn(HeroSpawnDummy.PlayerSideKey.Player, HeroSpawnDummy.HeroSideKey.Square, gameObject);
        heroSpawnPositionDiamond.GetComponent<HeroSpawnDummy>().SetPositionForHeroSpawn(HeroSpawnDummy.PlayerSideKey.Player, HeroSpawnDummy.HeroSideKey.Diamond, gameObject);

        heroSpawnPositionSquare.GetComponent<NetworkObject>().Spawn(true);
        heroSpawnPositionDiamond.GetComponent<NetworkObject>().Spawn(true);

        heroSpawnPositionSquare.GetComponent<HeroSpawnDummy>().SetParentForHeroSpawn(gameObject);
        heroSpawnPositionDiamond.GetComponent<HeroSpawnDummy>().SetParentForHeroSpawn(gameObject);
    }
}
