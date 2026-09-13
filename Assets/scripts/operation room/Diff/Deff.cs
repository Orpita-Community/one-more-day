using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class Deff : MonoBehaviour
{
    [SerializeField] private GameObject warningScreen, powerScreen;
    [SerializeField] private DragableItems leftPad, rightPad;
    [SerializeField] private float maxChargeIn = 4f;
    [SerializeField] private int powerIncraments = 50, minPower = 200, maxPower = 400;

    [Header("UI"), Space(8)]
    [SerializeField] private Image chargeBar;
    [SerializeField] private TextMeshProUGUI powerText;

    [Header("sound"), Space(8)]
    [SerializeField] private AudioSource ChargedAudio;
    [SerializeField] private AudioSource ShockAudio;
    [SerializeField] private AudioSource ChargingAudio;
    //-------------------------------------------------------
    private int _currentPower;
    private bool _isPadsPlaced, _charged;
    private Coroutine _charge;
    private Animation _chargeBarAnimation;

    void Start()
    {
        _currentPower = minPower;
        _chargeBarAnimation = chargeBar.transform.parent.GetComponent<Animation>();
    }

    void Update()
    {
        if (leftPad.IsPlaced && rightPad.IsPlaced)
        {
            warningScreen.SetActive(false);
            powerScreen.SetActive(true);
            _isPadsPlaced = true;
            powerText.text = $"{_currentPower} J";
        }
        else
        {
            warningScreen.SetActive(true);
            powerScreen.SetActive(false);
            _isPadsPlaced = false;
        }

        ChargedAudio.mute = !_charged;
    }

    public void IncreasePower()
    {
        if (_charge == null && _isPadsPlaced)
            _currentPower = Mathf.Clamp(_currentPower + powerIncraments, minPower, maxPower);
    }

    public void DecreasePower()
    {
        if (_charge == null && _isPadsPlaced)
            _currentPower = Mathf.Clamp(_currentPower - powerIncraments, minPower, maxPower);
    }

    public void Charge()
    {
        if (!_isPadsPlaced) return;

        if (_charged)
        {
            GameEventHandler.OnDiffShock?.Invoke(_currentPower);
            _chargeBarAnimation.Stop();
            _chargeBarAnimation.clip.SampleAnimation(chargeBar.gameObject, 0);
            _chargeBarAnimation.clip.SampleAnimation(powerText.gameObject, 0);

            chargeBar.gameObject.SetActive(false);

            ShockAudio.Play();

            _charged = false;
            _charge = null;
        }
        else
        {
            _charge ??= StartCoroutine(charge());
            ChargingAudio.Play();
        }
    }

    private IEnumerator charge()
    {
        float m_fillInSec = (Mathf.Max(maxPower - _currentPower, 10) / (float)maxPower) * maxChargeIn;

        chargeBar.gameObject.SetActive(true);
        chargeBar.fillAmount = 0;
        yield return new WaitUntil(
            () =>
            {
                chargeBar.fillAmount += m_fillInSec * Time.deltaTime;
                return chargeBar.fillAmount >= 0.95;
            }
        );
        chargeBar.fillAmount = 1;
        _chargeBarAnimation.Play();

        _charged = true;
    }

}
