using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField] protected Transform Target;
    [SerializeField] protected Vector3 Offset;
    [SerializeField] protected float Speed;
    [SerializeField, Range(0, 1)] protected float Weight;
    protected Transform _playerTransform;

    protected void Start()
    {
        _playerTransform = transform;
    }

    void Update()
    {
        track();
    }

    public virtual void track()
    {
        if (Weight == 0) _playerTransform.position = Target.position + Offset;
        else _playerTransform.position = Vector3.Lerp(_playerTransform.position, Target.position + Offset, Time.deltaTime * Mathf.Max(Weight, 0.1f) * Speed);
    }

}
