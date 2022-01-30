using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
     private void Start()
    {
        if (Argument == null)
        {
            Argument = true;
        }

        EntityManager entityManager = ServiceLocator.GetEntityManager();

        _entity = entityManager.Create(transform.GetInstanceID());
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag != "Player")
            return;
        
        Debug.Log("123");

        EntityManager entityManager = ServiceLocator.GetEntityManager();

        Entity otherEntity = entityManager.GetByInstanceId(Interactable.transform.GetInstanceID());

        if (otherEntity == null)
            return;

        _entity.Interact(otherEntity, Convert.ToInt32(Argument));
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        EntityManager entityManager = ServiceLocator.GetEntityManager();

        Entity otherEntity = entityManager.GetByInstanceId(Interactable.transform.GetInstanceID());
        if (otherEntity == null)
        {
            return;
        }
        _entity.Interact(otherEntity, Convert.ToInt32(Argument));
    }

    public GameObject Interactable;

    // For static Interactables false is until stepped off while true is permanent off
    [SerializeField] private bool Argument;
    private Entity _entity;
}
