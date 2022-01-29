using UnityEngine;

public delegate void BootstrapEventHandler();
public class Bootstrap : MonoBehaviour
{
    public event BootstrapEventHandler OnDestroy;
    public event BootstrapEventHandler OnApplicationQuit;

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
}
