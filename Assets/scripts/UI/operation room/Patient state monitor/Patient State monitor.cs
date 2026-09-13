using UnityEngine;
using UnityEngine.UI;

public class PatientStatemonitor : MonoBehaviour
{
    // inspactor vars
    [SerializeField] private Image heartRateBar, bloodPressurBar, oxygenLevelBar;
    [SerializeField] private AudioSource BeepingSource;
    //------------------------------------
    // private var
    private Animation _heartBarAnimation, _bloodPressurBarAnimation, _oxygenLevelBarAnimation;
    //------------------------------------
    void Start()
    {
        _heartBarAnimation = heartRateBar.gameObject.GetComponent<Animation>();
        _bloodPressurBarAnimation = bloodPressurBar.gameObject.GetComponent<Animation>();
        _oxygenLevelBarAnimation = oxygenLevelBar.gameObject.GetComponent<Animation>();
        GameEventHandler.OnPatientStateChanged += UpdateUI;

    }

    private void UpdateUI(PatientState state)
    {
        heartRateBar.fillAmount = Mathf.Lerp(heartRateBar.fillAmount, (float)state.heartRate / PatientInfo.UPPER_LETHEL_HEART_RATE, Time.deltaTime * 2f);
        BeepingSource.pitch = ((float)state.heartRate / PatientInfo.UPPER_LETHEL_HEART_RATE) - 0.5f + 1f;
        if (state.IsHeartRateCritical())
            _heartBarAnimation.Play();
        else
        {
            _heartBarAnimation.Stop();
            _heartBarAnimation.clip.SampleAnimation(heartRateBar.gameObject, 0);
        }

        bloodPressurBar.fillAmount = Mathf.Lerp(bloodPressurBar.fillAmount, state.bloodPrusser / PatientInfo.UPPER_LETHEL_BLOOD_PRUSSER, Time.deltaTime * 2f);
        if (state.IsBloodPressurCritical())
            _bloodPressurBarAnimation.Play();
        else
        {
            _bloodPressurBarAnimation.Stop();
            _bloodPressurBarAnimation.clip.SampleAnimation(bloodPressurBar.gameObject, 0);
        }

        oxygenLevelBar.fillAmount = Mathf.Lerp(oxygenLevelBar.fillAmount, state.oxygenLevel / PatientInfo.UPPER_LETHEL_OXYGRN_LEVEL, Time.deltaTime * 2f);
        if (state.IsOxygenLevelCritical())
            _oxygenLevelBarAnimation.Play();
        else
        {
            _oxygenLevelBarAnimation.Stop();
            _oxygenLevelBarAnimation.clip.SampleAnimation(oxygenLevelBar.gameObject, 0);
        }
    }
}
