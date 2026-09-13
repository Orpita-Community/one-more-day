using UnityEngine;
using UnityEngine.Events;
public class NpcAntraction : MonoBehaviour, Iinteractable
{
    [SerializeField] private string CharacterName;
    [SerializeField] private bool RandomPickMode;
    [SerializeField] private InteractionString[] interactions;
    [SerializeField] private GameObject interactionKeySprite;
    [System.Serializable]
    private struct InteractionString
    {
        [TextArea] public string interactionString;
        public TextAnimation StringAnimation;
        public UnityEvent OnInteraction;
    }
    //-----------------------------------------------
    private int _currentStringIndex;
    private bool _isInDialoge;

    void Start()
    {
        GameEventHandler.OnDialogeCall += (_, _, _) => _isInDialoge = true;
        GameEventHandler.OnDialogeEnd += () => _isInDialoge = false;
    }
    public void Interacte()
    {
        // if another dialoge is in display return until it disappear
        if (_isInDialoge)
            return;
        InteractionString m_currentInteraction;

        if (RandomPickMode)
        {
            m_currentInteraction = interactions[Random.Range(0, interactions.Length)];
            GameEventHandler.OnDialogeCall?.Invoke(m_currentInteraction.interactionString, CharacterName, m_currentInteraction.StringAnimation);
            return;
        }

        // get the current interaction text
        m_currentInteraction = interactions[_currentStringIndex];
        // display it
        GameEventHandler.OnDialogeCall?.Invoke(m_currentInteraction.interactionString, CharacterName, m_currentInteraction.StringAnimation);
        // move to the next text
        _currentStringIndex++;
        // bound the index to 0 and the end of interactions
        _currentStringIndex = Mathf.Clamp(_currentStringIndex, 0, interactions.Length - 1);
        // fire the interaction event
        GameEventHandler.OnDialogeEnd += () =>
        {
            FireInteractionEvent(m_currentInteraction);
            GameEventHandler.OnDialogeEnd -= () => FireInteractionEvent(m_currentInteraction);
        };
    }

    private void FireInteractionEvent(InteractionString interaction)
    {
        interaction.OnInteraction?.Invoke();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            interactionKeySprite.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            interactionKeySprite.SetActive(false);
    }
}
