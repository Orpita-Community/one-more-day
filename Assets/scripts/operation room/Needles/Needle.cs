using UnityEngine;

public class Needle : MonoBehaviour
{
    [SerializeField] private NeedleType NeedleType;
    [SerializeField] private effect[] NeedleEffects;

    private void Inject()
    {
        GameEventHandler.OnNeedleInject?.Invoke(NeedleEffects);
        GameEventHandler.OnNeedleUse?.Invoke(NeedleType);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Inject();
            Destroy(gameObject);
        }
    }
}

[System.Serializable]
public struct effect
{
    public enum effectType
    {
        None, HeartRate, BloodPressure, OxygenLevel, DeffWorkingChance
    }
    public effectType NeedleEffect;
    public float amount;
}


public enum NeedleType
{
    None,
    Adrenaline,
    Amiodarone,
    Vasopressors
}