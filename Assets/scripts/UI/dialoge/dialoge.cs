using System;
using System.Collections;
using UnityEngine;
using TMPro;
public class DialogeSystem : MonoBehaviour
{
    [SerializeField] private GameObject textContinear;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private GameObject letterPrefab;

    // animation for text to add charastristic to each letter
    [SerializeField] private float timeBetweenLetters;

    //-----------------------------------------------------------------
    private Transform _textContainerTransform;
    private Coroutine _textTypeingCoroutine;
    private TextAnimation _currentTextAnimation;
    private Action _OnDialogeSkip;
    private int _currentLetterIndex;
    private char[] _currentDialogeChars;
    //-----------------------------------------------------------------

    #region input
    private newInputSystem _input;

    void Awake()
    {
        _input = new newInputSystem();
    }

    void OnEnable()
    {
        _input.Enable();
    }

    void OnDisable()
    {
        _input.Disable();
    }

    #endregion

    void Start()
    {
        _textContainerTransform = textContinear.transform;

        GameEventHandler.OnDialogeCall += (dialogeString, CharacterName, TextAnimationMode) => ShowDialogeBox(dialogeString, CharacterName, TextAnimationMode, out _currentDialogeChars);
        _input.Player.Skiptext.performed += (keypress_data) => _OnDialogeSkip.Invoke();
        _OnDialogeSkip += () =>
        {
            // if the dialoge box is not there so nothing to do here
            if (!_textContainerTransform.parent.gameObject.activeSelf) return;

            // if the typeing is typeing complete the centance and if not check and hide
            if (_textTypeingCoroutine != null)
            {
                // stop the typeing effect and setting the flag to false
                StopCoroutine(_textTypeingCoroutine);
                _textTypeingCoroutine = null;

                // complete the centance and add its animations
                CompleteDialoge(_currentDialogeChars);
            }
            else
            {
                // hide the main Dialoge box if the text is aleardy typed
                if (_currentLetterIndex == _currentDialogeChars.Length - 1)
                {
                    _textContainerTransform.parent.gameObject.SetActive(false);

                    // alart the game system for dialoge end
                    GameEventHandler.OnDialogeEnd?.Invoke();
                }
            }
        };
    }


    private void ShowDialogeBox(string DialogeString, string charName, TextAnimation animation, out char[] DialogeWords)
    {

        // setting up global vars
        // dividing the text up and passing it to be typed
        char[] m_DialogeStringWords = DialogeString.ToCharArray();
        // reset the index for the new dialoge
        _currentLetterIndex = 0;
        // pass the words to be script global
        DialogeWords = m_DialogeStringWords;
        // setting the word animation mode
        _currentTextAnimation = animation;

        // if we are still typing return
        if (_textTypeingCoroutine != null)
            return;

        // show the main Dialoge box to type in
        _textContainerTransform.parent.gameObject.SetActive(true);

        // type the talking character name
        characterNameText.text = charName;
        // removing massage from the text container
        for (int childIndex = 0; childIndex < _textContainerTransform.childCount; childIndex++)
        {
            Destroy(_textContainerTransform.GetChild(childIndex).gameObject);
        }

        // if the _textTypingCor is null type the dialoge and if not just ignore
        if (_textTypeingCoroutine == null)
        {
            _textTypeingCoroutine = StartCoroutine(TypeTextMassge(m_DialogeStringWords));
        }
    }

    private IEnumerator TypeTextMassge(char[] dialogeWords)
    {
        // for evert letter type it
        for (int _letterIndex = 0; _letterIndex < dialogeWords.Length; _letterIndex++)
        {
            // getting the letter index to the global scope
            _currentLetterIndex = _letterIndex;

            // adding the letters to the grid and setting the animations
            Typeletter(dialogeWords[_letterIndex], out GameObject spawnedletter);
            SetLetterAnimation(_currentTextAnimation, spawnedletter);

            // wait the time between the letters
            yield return new WaitForSeconds(timeBetweenLetters);
        }

        // after complete typing set the flag to false
        _textTypeingCoroutine = null;
    }

    private void SetLetterAnimation(TextAnimation textAnimation, GameObject letter)
    {
        // just choosing the text animation mode and setting the character animator to it
        switch (textAnimation)
        {
            case TextAnimation.None:
                // no animation here
                break;

            // rest of the animations
            default:
                Animation m_letterAnimationComponant = letter.transform.GetChild(0).GetComponent<Animation>();

                // if the user uses the delay we wait otherwith we don't
                if (textAnimation.ToString().ToLower()[0] == 'r')
                {
                    // using random wait here to break the repatitivety if all animation played at once
                    m_letterAnimationComponant.gameObject.SetActive(false);
                    StartCoroutine(PlayAnimationWithRandomOffset(UnityEngine.Random.value * 0.25f, m_letterAnimationComponant, textAnimation.ToString()));
                    break;
                }

                m_letterAnimationComponant.Play(textAnimation.ToString());
                break;
        }
    }

    private IEnumerator PlayAnimationWithRandomOffset(float offset, Animation animation, string clip)
    {
        yield return new WaitForSeconds(offset);
        animation.gameObject.SetActive(true);

        // arr[1..] is a range operator 1.. means from the second element to the end
        animation.Play(clip[1..]);
    }

    private void Typeletter(char letter, out GameObject Spawnedletter)
    {
        // spawning the letter and setting its text to the letter that we are typeing 
        GameObject m_letter = Instantiate(letterPrefab, _textContainerTransform);

        // first child is the text element
        m_letter.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = letter.ToString();

        // returing the typed letter to work with it
        Spawnedletter = m_letter;
    }

    private void CompleteDialoge(char[] DialogeWords)
    {
        // if we want to skip the typing effect just pick up from the last letter we were in and type the centance quick
        for (int _letterIndex = _currentLetterIndex + 1; _letterIndex < DialogeWords.Length; _letterIndex++)
        {
            Typeletter(DialogeWords[_letterIndex], out GameObject spawnedletter);
            SetLetterAnimation(_currentTextAnimation, spawnedletter);
            _currentLetterIndex++;
        }
    }
}

// text animation modes
// add ((R)) before the animation if you want to use the random play after sec
public enum TextAnimation
{
    None,
    RWiggle,
    JumpOut
}