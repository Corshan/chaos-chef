using System;
using System.Collections;
using System.Collections.Generic;
using LobbySystem;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUiEvents : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void JoinServer()
    {
        NetworkManager.Singleton.StartClient();

        SceneLoader.ChangeScene(SceneLoader.Scenes.Sandbox);
    }

    public void PlayGame()
    {
        try
        {
            LobbyManager.Singleton.QuickJoin();

            // SceneLoader.ChangeScene(SceneLoader.Scenes.Sandbox);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void OpenLobbyMenu()
    {
        anim.SetBool("openLobbyMenu", true);
    }

    public void CloseLobbyMenu()
    {
        anim.SetBool("openLobbyMenu", false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
