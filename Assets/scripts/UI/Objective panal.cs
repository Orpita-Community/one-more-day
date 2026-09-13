using System;
using TMPro;
using UnityEngine;

public class Objectivepanal : MonoBehaviour
{
    [SerializeField] private GameObject ObjectiveTextPrefab;
    [SerializeField] private GameObject ObjectivePanal;

    void Start()
    {
        GameEventHandler.OnObjectiveCall += AddToObjectivePanal;
    }

    // if the event fire to add an objective to the ui this func will take the text and spawn one
    // UI is seprate from the logic
    private void AddToObjectivePanal(string ObjectiveString)
    {
        TextMeshProUGUI m_ObjectiveTextComponant = Instantiate(ObjectiveTextPrefab, ObjectivePanal.transform).GetComponent<TextMeshProUGUI>();
        m_ObjectiveTextComponant.text = ObjectiveString;
    }
}
