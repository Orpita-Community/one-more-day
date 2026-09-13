using UnityEngine;
using UnityEngine.U2D.Animation;

public class Patientcoat : MonoBehaviour
{
    // inspector vars
    [SerializeField] private GameObject leftPad, rightPad;
    //-----------------------------------------------------
    // private vars
    private bool _isCoatOpen = false;
    private SpriteResolver _patientCoatStates;
    //-----------------------------------------------------

    void Start()
    {
        _patientCoatStates = GetComponent<SpriteResolver>();
        UpdateVisuls();
    }

    public void ToggleCoatState()
    {
        _isCoatOpen = !_isCoatOpen;
        UpdateVisuls();
    }

    private void UpdateVisuls()
    {
        leftPad.SetActive(_isCoatOpen);
        rightPad.SetActive(_isCoatOpen);

        _patientCoatStates.SetCategoryAndLabel("Patient", _isCoatOpen ? "open" : "close");
    }
}
