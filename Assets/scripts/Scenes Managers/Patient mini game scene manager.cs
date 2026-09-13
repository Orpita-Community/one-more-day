using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Patientminigamescenemanager : MonoBehaviour
{
    void Start()
    {
        GameEventHandler.OnPatientMiniGameEnded += OnPatientSaved;
        GameEventHandler.OnPatientMiniGameFialed += OnPatientDead;
    }

    private void OnPatientDead()
    {
        SceneManager.LoadScene("afterMiniGame-faild");
    }

    private void OnPatientSaved()
    {
        SceneManager.LoadScene("afterMiniGame-saved");
    }
}
