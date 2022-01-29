public static class ServiceLocator
{
    public static void SetGameHandler(GameHandler gameHandler) { _gameHandler = gameHandler; }
    public static GameHandler GetGameHandler() { return _gameHandler; }

    public static void SetEntityManager(EntityManager entityManager) { _entityManager = entityManager; }
    public static EntityManager GetEntityManager() { return _entityManager;}


    private static GameHandler _gameHandler = null;
    private static EntityManager _entityManager = null;
}