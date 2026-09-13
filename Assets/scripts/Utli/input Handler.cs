using UnityEngine;
using System.Collections;

public class inputHandler : MonoBehaviour
{
    public static inputHandler Instance { get; private set; }
    //-----------------------------------------------------------------
    // a value to use from other classes
    public Vector2 InputDir { get; private set; }
    //-----------------------------------------------------------------
    private bool _diableInputAccess;
    private newInputSystem input;

    private void Awake()
    {
        input = new newInputSystem();
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("there is more than one input handler");
            return;
        }
        Instance = this;

        // if there is any cut scene disable the input form the user
        input.Player.Move.performed += MoveVector => { if (!_diableInputAccess) InputDir = MoveVector.ReadValue<Vector2>(); };
        input.Player.Move.canceled += _ => { if (!_diableInputAccess) InputDir = Vector2.zero; };

        // setting the player input access acordingly to the cutscene or not
        GameEventHandler.OnCutSceneTriggerEnter += (_, _) => DiableMovement();
        GameEventHandler.OnCutSceneEnd += () => EnableMovement();

        // if there is a dialoge on the screen disable input access
        GameEventHandler.OnDialogeCall += (_, _, _) => DiableMovement();
        GameEventHandler.OnDialogeEnd += EnableMovement;

    }

    private void DiableMovement()
    {
        _diableInputAccess = true;
        InputDir = Vector2.zero;
    }

    private void EnableMovement()
    {
        _diableInputAccess = false;
    }
}
