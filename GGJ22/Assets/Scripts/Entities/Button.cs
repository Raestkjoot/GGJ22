using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    private void Start()
    {
        EntityManager entityManager = ServiceLocator.GetEntityManager();

        _entity = entityManager.Create(transform.GetInstanceID());
    }
    private void OnMouseDown() 
    {
        EntityManager entityManager = ServiceLocator.GetEntityManager();

        Entity otherEntity = entityManager.GetByInstanceId(Interactable.transform.GetInstanceID());
        if (otherEntity == null)
        {
            return;
        }
        _entity.Interact(otherEntity, 0);
    }

    public GameObject Interactable;

    private Entity _entity;
}
