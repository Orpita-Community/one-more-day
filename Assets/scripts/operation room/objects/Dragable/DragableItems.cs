using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragableItems : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float distanceToClick = 1, maxAngle = -180;
    [SerializeField] private Vector2 UpDiraction = Vector2.up;
    [SerializeField] private float movingEasing = 30, rotationEasing = 20;
    [SerializeField] private bool goBackOnRelease = true;
    //------------------------------------------------------
    public bool IsPlaced { get; private set; }
    public bool IsMoving { get; private set; }

    //-------------------------------------------------------
    private Vector3 _worldMousePos, _startPos, _lastFramePos, _pickupOffset;
    private Transform _objectTransform;
    private int _sprite_layer;
    private SpriteRenderer _spriteRenderer;

    //-------------------------------------------------------

    void Start()
    {
        _objectTransform = transform;
        _startPos = _objectTransform.position;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _sprite_layer = _spriteRenderer.sortingOrder;
    }

    void Update()
    {
        if (target && !target.gameObject.activeSelf && !IsMoving)
            ResetObjectPosition();

        _lastFramePos = _objectTransform.position;
        _worldMousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value) + new Vector3(0, 0, -10);

        if (IsMoving)
        {
            // if the mouse drag it move the object and set fields to its porper values
            if (target && Vector2.Distance(_worldMousePos, target.position) < distanceToClick && target.gameObject.activeSelf)
            {
                _objectTransform.position = target.position;
                return;
            }
            _objectTransform.position = Vector2.Lerp(_objectTransform.position, _worldMousePos + _pickupOffset, Time.deltaTime * movingEasing);
        }

        // makes the object tellte to the moving diraction
        _objectTransform.rotation = Quaternion.Lerp(_objectTransform.rotation, Quaternion.Euler(0, 0, Mathf.Clamp(Vector2.SignedAngle(UpDiraction, (_objectTransform.position - _lastFramePos).normalized), -maxAngle, maxAngle)), Time.deltaTime * rotationEasing);

    }

    private void ResetObjectPosition()
    {
        if (!goBackOnRelease) return;
        IsPlaced = false;
        _objectTransform.position = _startPos;
    }

    void OnMouseDrag()
    {
        IsMoving = true;
    }

    void OnMouseDown()
    {
        IsPlaced = false;
        _spriteRenderer.sortingOrder = 100;
        _pickupOffset = _worldMousePos - _objectTransform.position;
    }

    void OnMouseUp()
    {
        if (target && _objectTransform.position == target.position)
        {
            IsPlaced = true;
        }
        else
        {
            ResetObjectPosition();
        }
        IsMoving = false;
        _spriteRenderer.sortingOrder = _sprite_layer;
    }
}
