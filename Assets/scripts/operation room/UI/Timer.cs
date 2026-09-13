using System.Collections;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI minsText, secText;
    [SerializeField, Tooltip("in Secands")] private int DirationToServive;

    private float _mins, _secs;
    void Start()
    {
        _mins = DirationToServive / 60;
        _secs = DirationToServive % 60;
        StartCoroutine(timer());
    }

    private IEnumerator timer()
    {

        _secs--;
        if (_secs < 0)
        {
            _secs = 59;
            _mins--;
        }

        minsText.text = _mins < 10 ? $"0{Mathf.RoundToInt(_mins)}" : $"{Mathf.RoundToInt(_mins)}";
        secText.text = _secs < 10 ? $"0{Mathf.RoundToInt(_secs)}" : $"{Mathf.RoundToInt(_secs)}";

        if (_mins <= 0)
        {
            GameEventHandler.OnPatientMiniGameEnded?.Invoke();
        }
        yield return new WaitForSeconds(1);
        StartCoroutine(timer());
    }

}
