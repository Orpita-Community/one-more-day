using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static GameObject PlayerInstance { get; private set; }
    //-----------------------------------------------------------------
    [SerializeField] private float MoveSpeed;
    [SerializeField] private Animator PlayerAnimator;
    //-----------------------------------------------------------------
    private Rigidbody2D _PlayerRigidbody2D;

    void Awake()
    {
        if (PlayerInstance != null)
        {
            Debug.LogError("there is more than one player");
            return;
        }
        PlayerInstance = gameObject;
    }

    void Start()
    {
        _PlayerRigidbody2D = GetComponent<Rigidbody2D>();

    }

    // doing physics stuff in update becouse a do whatever i want - your opinion don't matter :) -
    void Update()
    {
        Vector2 PlayerMoveDir = inputHandler.Instance.InputDir;

        // moving the player using the rigidbody -the chracter controler doesn't work-
        // player move diraction comes pre normalized from new input system
        _PlayerRigidbody2D.AddForce(PlayerMoveDir * MoveSpeed, ForceMode2D.Force);

        // Setting player animation atrs
        // all the rounding and miltplying for removing the velocity noise
        PlayerAnimator.SetInteger("x", (int)(Mathf.Round(_PlayerRigidbody2D.linearVelocityX) * 1000));
        PlayerAnimator.SetInteger("y", (int)(Mathf.Round(_PlayerRigidbody2D.linearVelocityY) * 1000));
    }
}
