using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUiEvents : MonoBehaviour
{
    public void JoinServer(){
        NetworkManager.Singleton.StartClient();

        SceneLoader.ChangeScene(SceneLoader.Scenes.Sandbox);
    }

    public void QuitGame(){
        Application.Quit();
    }
}
