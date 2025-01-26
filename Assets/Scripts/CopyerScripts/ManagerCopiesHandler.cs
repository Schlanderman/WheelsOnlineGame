using Unity.Netcode;
using UnityEngine;

public abstract class ManagerCopiesHandler<TManager> : NetworkBehaviour where TManager: MonoBehaviour
{
    protected TManager originalManager;

    public void SetOriginalManager(TManager manager)
    {
        originalManager = manager;
        SetEvents();
    }

    protected abstract void SetEvents();
}
