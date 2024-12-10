using UnityEngine;

public abstract class ManagerCopiesHandler<TManager> : MonoBehaviour where TManager: MonoBehaviour
{
    protected TManager originalManager;

    public void SetOriginalManager(TManager manager)
    {
        originalManager = manager;
        SetEvents();
    }

    protected abstract void SetEvents();
}
