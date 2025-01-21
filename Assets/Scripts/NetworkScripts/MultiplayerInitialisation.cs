using System;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerInitialisation : NetworkBehaviour
{
    public static MultiplayerInitialisation Instance { get; private set; }

    private bool IsInMenu = true;

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void SetIsInMenu(bool state)
    {
        IsInMenu = state;
    }
}
