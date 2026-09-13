using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private CircleCollider2D interactionArea;
    //--------------------------------------------------
    #region input
    private newInputSystem _input;
    void Awake()
    {
        _input = new newInputSystem();
    }

    void OnEnable()
    {
        _input.Enable();
    }

    void OnDisable()
    {
        _input.Disable();
    }
    #endregion

    void Start()
    {
        _input.Player.Interact.performed += Interacte;
    }

    private void Interacte(InputAction.CallbackContext context)
    {
        // make a new interactables list and get the object around the player
        List<Collider2D> m_objectsInInteractionArea = new();
        interactionArea.Overlap(m_objectsInInteractionArea);

        // foreach object if it interactable, interacte with it and if not dont do anything
        foreach (Collider2D interactionCollider in m_objectsInInteractionArea)
        {
            try
            {
                interactionCollider.gameObject.GetComponent<Iinteractable>().Interacte();
            }
            catch (NullReferenceException)
            {
                // isn't interactable
                continue;
            }
        }
    }
}
