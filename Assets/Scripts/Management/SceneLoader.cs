using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum Scenes {
        MAIN_MENU = 0,
        KITCHEN = 1
    }
    
    public static void ChangeScene(Scenes scene){
        SceneManager.LoadScene((int) scene);
    }
}
