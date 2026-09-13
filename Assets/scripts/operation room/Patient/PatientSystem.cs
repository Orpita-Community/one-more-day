using UnityEngine;
using System.Collections;

public class PatientSystem : MonoBehaviour
{
    // Public vars -user settings-
    [SerializeField] private PatientStateEnum _patientStateMachine;
    [SerializeField] private float changeStateAfter, timeTillDeath = 10f;
    //----------------------------------------------------------
    // private vars
    private PatientState _patientState;
    private bool _isMaskOn = false;
    //--------------------------------------------------------------

    // initializing the patient states
    void Awake()
    {
        _patientState = new(
            heartRate: Random.Range(PatientInfo.LOW_HEART_RATE, PatientInfo.HIGH_HEART_RATE),
            oxygenLevel: Random.Range(PatientInfo.LOW_OXYGEN_LEVEL, PatientInfo.HIGH_OXYGEN_LEVEL),
            bloodPrusser: Random.Range(PatientInfo.LOW_BLOOD_PRUSSER, PatientInfo.HIGH_BLOOD_PRUSSER),
            oxygenloseRate: 2,
            deffWorkChance: 0.6f
        );
    }

    void Start()
    {
        // adjust the states if this event fires - another scripts tries to change the patient-
        GameEventHandler.OnPatientStateAdjusted += AdjustState;
        GameEventHandler.OnBVMOn += () => _isMaskOn = true;
        GameEventHandler.OnBVMOff += () => _isMaskOn = false;
        GameEventHandler.OnDiffShock += ShockPatient;
        GameEventHandler.OnNeedleInject += InjectNeedle;

        // change the patient state on a timer
        StartCoroutine(ChangeStateTimer());
    }

    private void InjectNeedle(effect[] effects)
    {
        foreach (effect effect in effects)
        {
            switch (effect.NeedleEffect)
            {
                default:
                case effect.effectType.None:
                    return;

                case effect.effectType.HeartRate:
                    if (_patientState.heartRate == 0)
                        break;
                    GameEventHandler.OnPatientStateAdjusted?.Invoke(new()
                    {
                        heartRate = Mathf.RoundToInt(effect.amount)
                    });
                    break;

                case effect.effectType.BloodPressure:
                    GameEventHandler.OnPatientStateAdjusted(new()
                    {
                        bloodPrusser = effect.amount
                    });
                    break;

                case effect.effectType.OxygenLevel:
                    GameEventHandler.OnPatientStateAdjusted?.Invoke(new()
                    {
                        oxygenLevel = effect.amount
                    });
                    break;

                case effect.effectType.DeffWorkingChance:
                    GameEventHandler.OnPatientStateAdjusted?.Invoke(new()
                    {
                        deffWorkChance = effect.amount
                    });
                    break;
            }
        }
    }

    private void ShockPatient(int power)
    {
        // if (Random.value < (1 - _patientState.deffWorkChance))
        // {
        //     print("no heart restrat for you :P");
        //     return;
        // }

        if (_patientState.IsHeartRateCritical())
        {
            GameEventHandler.OnPatientStateAdjusted?.Invoke(new()
            {
                // (current heart rate / avrage heart rate) - 1 
                // range now from -1 to 1                               0 -> 2 | -1 -> 1 | power 0.0 -> -power 0.0
                // invert the range to make the arrow point to the center
                // and apply to the heart rate
                heartRate = ((_patientState.heartRate / ((PatientInfo.LOWER_LETHEL_HEART_RATE + PatientInfo.UPPER_LETHEL_HEART_RATE) / 2)) - 1) * (-power / 5)
            });
            _patientState.deffWorkChance = 0.6f;
        }
        else
        {
            if (Random.value > 0.6f)
            {
                // stop the heart complitily
                GameEventHandler.OnPatientStateAdjusted?.Invoke(new() { heartRate = -_patientState.heartRate });
                return;
            }
            GameEventHandler.OnPatientStateAdjusted?.Invoke(new() { heartRate = (int)(Mathf.Sign(Random.value - 0.5f) * (power / 5f)) });
        }
    }

    void Update()
    {
        GameEventHandler.OnPatientStateAdjusted?.Invoke(new()
        {
            oxygenLevel = (_isMaskOn ? +1 : -1) * _patientState.oxygenloseRate * Time.deltaTime
        });
    }

    private IEnumerator ChangeStateTimer()
    {
        // wait
        yield return new WaitForSeconds(changeStateAfter);

        // random adjust
        GameEventHandler.OnPatientStateAdjusted?.Invoke(new(
            heartRate: Random.Range(-30, 30),
            oxygenLevel: Random.Range(-40, 40),
            bloodPrusser: Random.Range(-35, 35),
            oxygenloseRate: Random.Range(2, 7)
        ));

        StartCoroutine(ChangeStateTimer());
    }

    private PatientState _oldPatientState;
    private void AdjustState(PatientState state)
    {
        _oldPatientState = _patientState;
        // add the current states and the input states
        _patientState += state;

        // fire the change event
        GameEventHandler.OnPatientStateChanged?.Invoke(_patientState);
        UpdateState();
    }

    private void UpdateState()
    {
        if (_patientState.IsDead())
            GameEventHandler.OnPatientDeath?.Invoke();

        if (!_oldPatientState.IsAlive() && _patientState.IsAlive())
            GameEventHandler.OnPatientRevive?.Invoke();

        if (_oldPatientState.IsCriticl() && !_patientState.IsCriticl())
        {
            _patientStateMachine = PatientStateEnum.Stable;
            GameEventHandler.OnPatientStable?.Invoke();
        }

        if (!_oldPatientState.IsCriticl() && _patientState.IsCriticl())
        {
            _patientStateMachine = PatientStateEnum.critical;
            GameEventHandler.OnPatientCritical?.Invoke();
        }
    }
}

public enum PatientStateEnum
{
    None,
    Stable,
    critical
}

public struct PatientInfo
{
    public const int LOW_HEART_RATE = 30, HIGH_HEART_RATE = 180, LOWER_LETHEL_HEART_RATE = 0, UPPER_LETHEL_HEART_RATE = 200;
    public const float LOW_OXYGEN_LEVEL = 20, HIGH_OXYGEN_LEVEL = 180, LOWER_LETHEL_OXYGEN_LEVEL = 0, UPPER_LETHEL_OXYGRN_LEVEL = 200;
    public const float LOW_BLOOD_PRUSSER = 50, HIGH_BLOOD_PRUSSER = 180, LOWER_LETHEL_BLOOD_PRUSSER = 0, UPPER_LETHEL_BLOOD_PRUSSER = 200;
}

public struct PatientState
{
    public int heartRate;
    public float oxygenLevel, bloodPrusser, oxygenloseRate, deffWorkChance;

    public PatientState(int heartRate = 0, float oxygenLevel = 0, float bloodPrusser = 0, float oxygenloseRate = 0, float deffWorkChance = 0.6f)
    {
        this.bloodPrusser = bloodPrusser;
        this.oxygenLevel = oxygenLevel;
        this.heartRate = heartRate;
        this.oxygenloseRate = oxygenloseRate;
        this.deffWorkChance = deffWorkChance;
    }

    public readonly bool IsHeartRateCritical()
    {
        return heartRate <= PatientInfo.LOW_HEART_RATE || heartRate >= PatientInfo.HIGH_HEART_RATE;
    }
    public readonly bool IsBloodPressurCritical()
    {
        return bloodPrusser <= PatientInfo.LOW_BLOOD_PRUSSER || bloodPrusser >= PatientInfo.HIGH_BLOOD_PRUSSER;
    }
    public readonly bool IsOxygenLevelCritical()
    {
        return oxygenLevel <= PatientInfo.LOW_OXYGEN_LEVEL || oxygenLevel >= PatientInfo.HIGH_OXYGEN_LEVEL;
    }

    public readonly bool IsCriticl()
    {
        return bloodPrusser <= PatientInfo.LOW_BLOOD_PRUSSER || bloodPrusser >= PatientInfo.HIGH_BLOOD_PRUSSER || heartRate <= PatientInfo.LOW_HEART_RATE || heartRate >= PatientInfo.HIGH_HEART_RATE || oxygenLevel <= PatientInfo.LOW_OXYGEN_LEVEL || oxygenLevel >= PatientInfo.HIGH_OXYGEN_LEVEL;
    }
    public readonly bool IsDead()
    {
        return (bloodPrusser == PatientInfo.UPPER_LETHEL_BLOOD_PRUSSER || bloodPrusser == PatientInfo.LOWER_LETHEL_BLOOD_PRUSSER) && (heartRate == PatientInfo.UPPER_LETHEL_HEART_RATE || heartRate == PatientInfo.LOWER_LETHEL_HEART_RATE) && (oxygenLevel == PatientInfo.UPPER_LETHEL_OXYGRN_LEVEL || oxygenLevel == PatientInfo.LOWER_LETHEL_OXYGEN_LEVEL);
    }
    public readonly bool IsAlive()
    {
        return !(bloodPrusser == PatientInfo.UPPER_LETHEL_BLOOD_PRUSSER || bloodPrusser == PatientInfo.LOWER_LETHEL_BLOOD_PRUSSER) && !(heartRate == PatientInfo.UPPER_LETHEL_HEART_RATE || heartRate == PatientInfo.LOWER_LETHEL_HEART_RATE) && !(oxygenLevel == PatientInfo.UPPER_LETHEL_OXYGRN_LEVEL || oxygenLevel == PatientInfo.LOWER_LETHEL_OXYGEN_LEVEL);
    }

    public static PatientState operator +(PatientState A, PatientState B)
    {
        PatientState results = new()
        {
            bloodPrusser = Mathf.Clamp(A.bloodPrusser + B.bloodPrusser, PatientInfo.LOWER_LETHEL_BLOOD_PRUSSER, PatientInfo.UPPER_LETHEL_BLOOD_PRUSSER),
            heartRate = Mathf.Clamp(A.heartRate + B.heartRate, PatientInfo.LOWER_LETHEL_HEART_RATE, PatientInfo.UPPER_LETHEL_HEART_RATE),
            oxygenLevel = Mathf.Clamp(A.oxygenLevel + B.oxygenLevel, PatientInfo.LOWER_LETHEL_OXYGEN_LEVEL, PatientInfo.UPPER_LETHEL_OXYGRN_LEVEL),
            oxygenloseRate = A.oxygenloseRate,
            deffWorkChance = A.deffWorkChance
        };
        return results;
    }
}