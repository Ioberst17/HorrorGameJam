using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour
{
    public void LoadScene(int BuildIndex)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(BuildIndex);
    }
}
