using System;
using System.Collections;
using System.Collections.Generic;
using LobbySystem;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUiEvents : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _connectingScreen;
    [SerializeField] private TextMeshProUGUI _message;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _soundSlider;
    private Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        ActivateMainMenu(true);
        _message.text = "Connecting...";
        _message.color = Color.white;

        _musicSlider.minValue = AudioManager.Singleton.MinVolume;
        _musicSlider.maxValue = AudioManager.Singleton.MaxVolume;
        _musicSlider.value = AudioManager.Singleton.MusicVolume();

        _soundSlider.minValue = AudioManager.Singleton.MinVolume;
        _soundSlider.maxValue = AudioManager.Singleton.MaxVolume;
        _soundSlider.value = AudioManager.Singleton.SfxVolume();
    }

    public void JoinServer()
    {
        NetworkManager.Singleton.StartClient();

        SceneLoader.ChangeScene(SceneLoader.Scenes.KITCHEN);
    }

    public async void PlayGame()
    {
        try
        {
            _message.text = "Connecting...";
            _message.color = Color.white;
            ActivateMainMenu(false);

            await LobbyManager.Singleton.QuickJoin();

            // SceneLoader.ChangeScene(SceneLoader.Scenes.Sandbox);
        }
        catch (Exception e)
        {
            _message.color = Color.red;
            _message.text = "Connection Failed";

            Invoke(nameof(InvokeMainMenu), 3);
        }
    }

    public void OpenLobbyMenu() => _anim.SetBool("isOpen", true);
    public void CloseLobbyMenu() => _anim.SetBool("isOpen", false);

    private void InvokeMainMenu()
    {
        ActivateMainMenu(true);
    }

    private void ActivateMainMenu(bool active)
    {
        _mainMenu.SetActive(active);
        _connectingScreen.SetActive(!active);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnMusicVolumeChanged(float value)
    {
        AudioManager.Singleton.ChangeMusicVolume(value);
    }

    public void OnSfxVolumeChanged(float value)
    {
        AudioManager.Singleton.ChangeSFXVolume(value);
    }
}
