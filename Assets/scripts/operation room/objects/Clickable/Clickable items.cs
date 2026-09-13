using UnityEngine;
using UnityEngine.Events;

public class Clickableitems : MonoBehaviour
{
    [SerializeField] private UnityEvent OnClick;

    void OnMouseDown()
    {
        OnClick?.Invoke();
    }
}
