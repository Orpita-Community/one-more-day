using System.Collections;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class BVMbehavier : MonoBehaviour
{
    //private vars
    private DragableItems _objectDragScript;
    private SpriteResolver _spritePallete;
    private PatientState _patientState;
    private Coroutine _breath;
    void Start()
    {
        _objectDragScript = GetComponent<DragableItems>();
        _spritePallete = GetComponent<SpriteResolver>();

        GameEventHandler.OnPatientStateChanged += UpdatePatientState;
    }

    private void UpdatePatientState(PatientState state)
    {
        _patientState = state;
    }

    void Update()
    {
        // if moving
        if (_objectDragScript.IsMoving)
        {
            _spritePallete.SetCategoryAndLabel("OnPatient", "0");
            return;
        }
        else
        {
            // if the mask is on the tray
            if (!_objectDragScript.IsPlaced)
            {
                GameEventHandler.OnBVMOff?.Invoke();
                _spritePallete.SetCategoryAndLabel("Still", "0");
                return;
            }
        }

        // player put the mask on the patient face
        if (_objectDragScript.IsPlaced && !_objectDragScript.IsMoving)
        {
            GameEventHandler.OnBVMOn?.Invoke();
            _breath ??= StartCoroutine(breathCycle());
        }
    }

    private IEnumerator breathCycle()
    {
        float m_timeBetweenBreath = Mathf.Max(_patientState.oxygenLevel / PatientInfo.UPPER_LETHEL_OXYGRN_LEVEL, 0.01f) * 1f;

        _spritePallete.SetCategoryAndLabel("OnPatient", "0");
        yield return new WaitForSeconds(m_timeBetweenBreath);
        _spritePallete.SetCategoryAndLabel("OnPatient", "1");
        yield return new WaitForSeconds(m_timeBetweenBreath);

        _breath = null;
    }

}
