using UnityEngine;
using System.Collections;
using TMPro;

public class DeathTimer : MonoBehaviour
{
    [SerializeField] private GameObject deathTimer;
    [SerializeField] private TextMeshProUGUI daethTiemrText;
    [Tooltip("in secands"), SerializeField] private float timeTillDeath;

    private Coroutine _updateTimer;
    private float _remaningTimer;
    void Start()
    {
        GameEventHandler.OnPatientCritical += ShowAndUpdateTimer;
        GameEventHandler.OnPatientStable += HideAndResetTimer;
    }

    private void HideAndResetTimer()
    {
        deathTimer.SetActive(false);
        StopCoroutine(_updateTimer);
        _updateTimer = null;
    }

    private void ShowAndUpdateTimer()
    {
        _remaningTimer = timeTillDeath;
        deathTimer.SetActive(true);
        _updateTimer ??= StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        while (_remaningTimer > 0)
        {
            yield return new WaitForSeconds(0.01f);
            _remaningTimer -= 0.01f;
            daethTiemrText.text = (Mathf.RoundToInt(_remaningTimer) < 10 ? $"0{Mathf.RoundToInt(_remaningTimer)}" : $"{Mathf.RoundToInt(_remaningTimer)}") + " : " + $"{_remaningTimer.ToString()[2..4]}";

        }
        _updateTimer = null;
        GameEventHandler.OnPatientMiniGameFialed?.Invoke();
    }
}
