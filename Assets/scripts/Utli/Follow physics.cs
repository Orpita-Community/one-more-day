using UnityEngine;

public class Followphysics : Follow
{
    // this class goes further then the follow class using the rigidbody and it's velocity to calculate the target position
    [Space(8), SerializeField, Tooltip("Offset of the linear velocity in local space")] private Vector2 physicsOffset;
    [SerializeField] protected Rigidbody2D targetRigidbody;

    public override void track()
    {
        if (Weight == 0)
        {
            _playerTransform.position = Target.position + Offset;
        }
        else
        {
            _playerTransform.position = Vector3.Lerp(_playerTransform.position,
            Target.position + Offset + (Vector3)(targetRigidbody.linearVelocity.normalized + (Vector2)(Target.up * physicsOffset.y + Target.right * physicsOffset.x)),
            Time.deltaTime * Mathf.Max(Weight, 0.1f) * Speed);
        }
    }
}
