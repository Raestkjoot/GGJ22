using System;
using UnityEngine;

public delegate void BootstrapEventHandler();
public class Bootstrap : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(this);

        GameHandler gameHandler = new GameHandler();
        gameHandler.Initialize(this);
        ServiceLocator.SetGameHandler(gameHandler);
        
        EntityManager entityManager = new EntityManager();
        entityManager.Initialize(this);
        ServiceLocator.SetEntityManager(entityManager);
    }
    private void Update()
    {
        GameHandler gameHandler = ServiceLocator.GetGameHandler();

        if (Input.GetKeyDown(KeyCode.R))
        {
            gameHandler.ResetMap();
        }
    }

#pragma warning disable CS0067
    public event BootstrapEventHandler OnDestroy;
    public event BootstrapEventHandler OnApplicationQuit;
#pragma warning restore CS0067
}
