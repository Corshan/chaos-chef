using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameMenuUI : NetworkBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private TextMeshProUGUI _buttontext;
    private NetworkVariable<bool> _text = new();

    void Start()
    {
        _text.OnValueChanged += TextUpdate;
    }

    private void TextUpdate(bool previousValue, bool newValue)
    {
        if(newValue) _buttontext.text = "End Shift";
        else _buttontext.text = "Start Shift";
        
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
