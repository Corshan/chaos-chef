using System;
using System.Collections;
using System.Collections.Generic;
using LobbySystem;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies;
using UnityEngine;

public class GameMenuUI : NetworkBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private TextMeshProUGUI _buttontext;
    [SerializeField] private GameObject _settingsMenu;
    private NetworkVariable<bool> _text = new();
    private bool _isOpen = false;

    void Start()
    {
        _text.OnValueChanged += TextUpdate;
        _settingsMenu.SetActive(_isOpen);
    }

    private void TextUpdate(bool previousValue, bool newValue)
    {
        if (newValue) _buttontext.text = "End Shift";
        else _buttontext.text = "Start Shift";

    }

    public void ToggleSettingsScreen()
    {
        _settingsMenu.SetActive(!_isOpen);
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
        _gameManager.StartGame();
        _buttontext.text = "End Shift";
        _text.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void EndGameEventServerRpc()
    {
        _gameManager.EndGame();
        _buttontext.text = "Start Shift";
        _text.Value = false;
    }
}
