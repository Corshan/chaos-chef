using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum Scenes {
        MainMenu = 0,
        Sandbox = 1
    }
    
    public static void ChangeScene(Scenes scene){
        SceneManager.LoadScene((int) scene);
    }
}
