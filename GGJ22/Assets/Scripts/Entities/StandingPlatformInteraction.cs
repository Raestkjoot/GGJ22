using UnityEngine;

public class StandingPlatformInteraction : MonoBehaviour
{
    private void Start()
    {
        EntityManager entityManager = ServiceLocator.GetEntityManager();

        _entity = entityManager.Create(transform.GetInstanceID());
        _entity.OnInteract += Entity_OnInteract;
    }

    private void Entity_OnInteract(Entity me, Entity other, object arg)
    {
        gameObject.SetActive(false);
    }

    private Entity _entity = null;
}
