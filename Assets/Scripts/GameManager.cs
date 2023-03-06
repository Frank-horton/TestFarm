using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameManager
{
    public static void LoadScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID); 
    }
}
