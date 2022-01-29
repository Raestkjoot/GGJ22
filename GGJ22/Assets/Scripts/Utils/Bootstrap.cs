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
        else if (Input.GetKeyDown(KeyCode.N))
        {
            gameHandler.LoadNextMap();
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            gameHandler.LoadPrevMap();
        }
    }

    public event BootstrapEventHandler OnDestroy;
    public event BootstrapEventHandler OnApplicationQuit;
}
