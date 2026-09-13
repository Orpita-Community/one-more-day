using System;


public static class GameEventHandler
{
    // Game events
    public static Action<int, int> OnCutSceneTriggerEnter;
    public static Action OnCutSceneEnd;
    public static Action OnCutSceneContinue;
    public static Action<string> OnObjectiveCall;
    public static Action<string, string, TextAnimation> OnDialogeCall;
    public static Action OnDialogeEnd;

    // patient system

    /// <summary>
    /// fires when the patient state changes -no effect on the patient system-
    /// </summary>
    public static Action<PatientState> OnPatientStateChanged;

    /// <summary>
    /// used for changeing the patient states
    /// </summary>
    public static Action<PatientState> OnPatientStateAdjusted;
    public static Action OnPatientDeath, OnPatientRevive, OnPatientStable, OnPatientCritical;
    public static Action OnBVMOn, OnBVMOff;

    public static Action<int> OnDiffShock;
    public static Action<effect[]> OnNeedleInject;
    public static Action<NeedleType> OnNeedleUse;

    public static Action OnPatientMiniGameEnded;
    public static Action OnPatientMiniGameFialed;

    /// <summary>
    /// this func cut all the old connection berfore changeing the scene to avoid null referancing - the object is destroyed when loading a scene so the referance to the subescribed funcs is all null -
    /// </summary>
    public static void CutOldConnection()
    {
        OnCutSceneTriggerEnter = null;
        OnCutSceneContinue = null;
        OnCutSceneEnd = null;

        OnObjectiveCall = null;
        OnDialogeCall = null;
        OnDialogeEnd = null;

        OnPatientStateAdjusted = null;
        OnPatientStateChanged = null;

        OnPatientDeath = null;
        OnPatientRevive = null;
        OnPatientStable = null;
        OnPatientCritical = null;

        OnBVMOff = null;
        OnBVMOn = null;

        OnDiffShock = null;
        OnNeedleInject = null;
        OnNeedleUse = null;

        OnPatientMiniGameEnded = null;
        OnPatientMiniGameFialed = null;
    }
}
