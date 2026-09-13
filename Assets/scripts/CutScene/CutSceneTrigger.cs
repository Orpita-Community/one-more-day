using UnityEngine;

public class CutSceneTrigger : MonoBehaviour
{
    [SerializeField] private int SceneID, CutsceneID;

    void OnTriggerStay2D(Collider2D collision)
    {
        // if a collider entered check if its the player - the only trigger is the player in the game if you want more add a tag var -
        if (collision.gameObject.CompareTag("Player"))
        {
            GameEventHandler.OnCutSceneTriggerEnter?.Invoke(SceneID, CutsceneID);
        }
    }


}
