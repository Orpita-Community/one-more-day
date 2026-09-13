using UnityEngine;
using UnityEngine.SceneManagement;

public class Btnsactions : MonoBehaviour
{
    public void OnPlaybtnPress()
    {
        GameEventHandler.CutOldConnection();
        SceneManager.LoadScene("hospital");
    }

    public void OnQuitbtnPress()
    {
        Application.Quit();
    }

}
