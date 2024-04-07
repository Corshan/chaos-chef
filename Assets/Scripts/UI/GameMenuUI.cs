using System;
using System.Collections;
using System.Collections.Generic;
using LobbySystem;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuUI : NetworkBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private TextMeshProUGUI _buttontext;
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _soundSlider;
    private NetworkVariable<bool> _text = new();
    private bool _isOpen = false;

    void Start()
    {
        _text.OnValueChanged += TextUpdate;
        _settingsMenu.SetActive(_isOpen);

        _musicSlider.minValue = AudioManager.Singleton.MinVolume;
        _musicSlider.maxValue = AudioManager.Singleton.MaxVolume;
        _musicSlider.value = AudioManager.Singleton.MusicVolume();

        _soundSlider.minValue = AudioManager.Singleton.MinVolume;
        _soundSlider.maxValue = AudioManager.Singleton.MaxVolume;
        _soundSlider.value = AudioManager.Singleton.SfxVolume();
    }

    private void TextUpdate(bool previousValue, bool newValue)
    {
        if (newValue) _buttontext.text = "End Shift";
        else _buttontext.text = "Start Shift";

    }

    public void ToggleSettingsScreen()
    {
        _isOpen = !_isOpen;
        _settingsMenu.SetActive(_isOpen);
    }

    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
        LobbyManager.Singleton.DisconnectFromLobby();
        SceneLoader.ChangeScene(SceneLoader.Scenes.MAIN_MENU);
    }

    public void QuitGame()
    {
        NetworkManager.Singleton.Shutdown();
        LobbyManager.Singleton.DisconnectFromLobby();
        Application.Quit();
    }

    public void ToggleGameState()
    {
        if (_gameManager.InRound) EndGameEvent();
        else StartGameEvent();
    }
    private void StartGameEvent()
    {
        StartGameEventServerRpc();
    }

    private void EndGameEvent()
    {
        EndGameEventServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartGameEventServerRpc()
    {
        StartShift();
    }

    [ServerRpc(RequireOwnership = false)]
    public void EndGameEventServerRpc()
    {
        EndShift();
    }

    public void OnMusicVolumeChanged(float value)
    {
        AudioManager.Singleton.ChangeMusicVolume(value);
    }

    public void OnSfxVolumeChanged(float value)
    {
        AudioManager.Singleton.ChangeSFXVolume(value);
    }

    public void EndShift()
    {
        _gameManager.EndGame();
        _buttontext.text = "Start Shift";
        _text.Value = false;
        Debug.Log($"[GameManager] Shift Ended");
    }

    public void StartShift()
    {
        _gameManager.StartGame();
        _buttontext.text = "End Shift";
        _text.Value = true;
        Debug.Log($"[GameManager] Shift Started");
    }
}
