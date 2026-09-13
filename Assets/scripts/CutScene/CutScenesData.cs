using UnityEngine;
using UnityEngine.Events;

public class CutScenesData : MonoBehaviour
{
    // finally an object for holding all the scenes and it's cutscenes
    public SceneCutScenes[] SceneCutscenesData;
}

// a scene cutscene for holding each scene cutscenes
[System.Serializable]
public class SceneCutScenes
{
    public CutScene[] CutScenes;
}

// a cutscene struct for holding each cutscene steps
[System.Serializable]
public class CutScene
{
    public Step[] Steps;
}

// step struct for holding the final world space distanation and the arrival even
[System.Serializable]
public class Step
{
    public string CharName;
    [TextArea] public string Dialog;

    [Tooltip("position in world space")]
    public Vector2 Distanation;
    public GameObject GameobjectToAnimate;
    public float MoveSpeed;
    public UnityEvent OnDistanationReach;
    public bool StopOnArrival;
}