using UnityEngine;
using System.Collections;

public class CutScenePlayer : MonoBehaviour
{
    [SerializeField] private CutScenesData CutscenesDataObject;
    [SerializeField] private float CutsceneTherahold = 0.01f;
    //-----------------------------------------------------------------
    private Coroutine _PlayingCutScene;
    private bool _hold;
    void Start()
    {
        // if the continue event fire the hold will be false and the cutScene will be continued
        GameEventHandler.OnCutSceneContinue += () => _hold = false;

        // if the player enters a trigger for a spacific scene the event will fire with SceneID and CutSceneID
        // play this cutscene on event fire using a coroutine
        GameEventHandler.OnCutSceneTriggerEnter += (SceneID, CutSceneID) =>
        {
            CutScene _currentPlayingCutScene = CutscenesDataObject.SceneCutscenesData[SceneID].CutScenes[CutSceneID];

            if (_currentPlayingCutScene == null)
            {
                Debug.LogError("there is no cutscene with this ID");
                return;
            }

            // ??= equals if(_PlayingCutScene == null) _PlayingCutScene = coroutine
            _PlayingCutScene ??= StartCoroutine(PlayCutScene(_currentPlayingCutScene));
        };
    }

    private IEnumerator PlayCutScene(CutScene cutScene)
    {
        for (int step = 0; step < cutScene.Steps.Length; step++)
        {
            // iterating over every step of the cutscene
            Step m_currentStep = cutScene.Steps[step];

            // getting the animation object transform for a little bit of preformance
            Transform m_AnimationGameObject = m_currentStep.GameobjectToAnimate.transform;

            // setting the input diraction to the diraction of the distanation and normalizeing it - player movement scripts will see the new input diraction and move accordingly
            yield return new WaitUntil(() =>
            {
                // getting the moving vector from the delta position of the main object --> distanation
                Vector2 m_MovingDiraction = (m_currentStep.Distanation - (Vector2)m_AnimationGameObject.position).normalized;

                // moving main object to the distanation using the provided speed
                // checking if the object uses a rigidbody and act
                try
                {
                    // if it has a rigidbody use it to move
                    Rigidbody2D m_AnimationObjectRigitbody = m_AnimationGameObject.gameObject.GetComponent<Rigidbody2D>();
                    m_AnimationObjectRigitbody.AddForce(m_MovingDiraction * m_currentStep.MoveSpeed, ForceMode2D.Force);
                }
                catch (System.Exception) // if any thing gone wrong
                {
                    // if it hasn't use the position to move it
                    m_AnimationGameObject.position += (Vector3)(m_currentStep.MoveSpeed * Time.deltaTime * m_MovingDiraction);
                }

                // if animation gameobject arrived to it's distanation the step will end - of course with some error -
                return Vector2.Distance(m_currentStep.Distanation, (Vector2)m_AnimationGameObject.position) < CutsceneTherahold;
            });

            // after reaching the final step position fire the event too make something happen
            m_currentStep.OnDistanationReach?.Invoke();

            //fire the dialoge with the provided dialoge
            if (m_currentStep.Dialog.Length > 0)
                GameEventHandler.OnDialogeCall?.Invoke(m_currentStep.Dialog, m_currentStep.CharName, TextAnimation.None);

            // if the step req's to stop or wait we stop executing
            // we go out of the wait state using the OnCutSceneContinue event from a custom script - we trigger this script from the step OnDistanationReach event -
            if (m_currentStep.StopOnArrival)
                _hold = true;

            // if the _hold == true the step will be pined in the state 
            // if the _hold == false the step will pass through and containe
            yield return new WaitUntil(() => { return !_hold; });
        }

        // trigger the cutscene end event 
        GameEventHandler.OnCutSceneEnd?.Invoke();

        // setting the flag for the next cutscene
        _PlayingCutScene = null;
    }
}
