using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class HospitalSceneManager : MonoBehaviour
{
    [Tooltip("in seconds"), SerializeField] private float TimeToCallEmergency = 120f;
    [SerializeField] private GameObject OperationRoomDoorTrigger;
    [SerializeField] private UnityEvent OnEmergencyTimerCall;
    //--------------------------------------------------------------
    private bool _IsInOperationRoom;

    void Start()
    {
        StartCoroutine(StartEmergencyTimer());
    }

    private IEnumerator StartEmergencyTimer()
    {
        // first timer to wait to initalize the objective system
        yield return new WaitForSeconds(1f);
        GameEventHandler.OnObjectiveCall?.Invoke("Explore the hospital and help patients");

        // i know it's abit sketchy but deadline is pressing
        // second one to wait for the actual event 
        yield return new WaitForSeconds(TimeToCallEmergency);
        GameEventHandler.OnObjectiveCall?.Invoke("Head to operation room");
        GameEventHandler.OnDialogeCall?.Invoke("Dr.Zain, please head to the operation room", "Emergency room", TextAnimation.RWiggle);
        OnEmergencyTimerCall?.Invoke();
    }

    public void EnterOperationRoom()
    {
        if (!_IsInOperationRoom)
        {
            print("go to operation");
            PlayerMovement.PlayerInstance.transform.position = new Vector3(-16.47f, -7.73f, 0); // operation room Enterance
            Destroy(OperationRoomDoorTrigger);
        }
        _IsInOperationRoom = true;
    }

    public void EnterMiniGame()
    {
        GameEventHandler.CutOldConnection();
        SceneManager.LoadScene("PatientMiniGame");
    }
}
