using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Singleton { get; private set; }
    [SerializeField][Range(60, 600)] private float roundTimer = 300;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _cashText;
    [SerializeField][Range(10, 100)] private int _burgerCashAmount = 10;
    [SerializeField] private List<OrderSystem> _orderSystem;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private GameMenuUI _gameMenuUI;
    private NetworkVariable<bool> _inRound = new(false);
    public bool InRound => _inRound.Value;
    private NetworkVariable<float> _timer = new(0);
    private NetworkVariable<int> _cash = new(0);

    private void Awake()
    {
        if (Singleton != null && Singleton != this) Destroy(this);
        else
        {
            Singleton = this;
            DontDestroyOnLoad(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _timer.Value = roundTimer;
        _timerText.text = ConvertTimer(_timer.Value);
        _cashText.text = _cash.Value + "";

        _timer.OnValueChanged += OnTimerChanged;
        _cash.OnValueChanged += OnCashChanged;
    }

    private void OnTimerChanged(float previousValue, float newValue)
    {
        _timerText.text = ConvertTimer(newValue);
    }

    private void OnCashChanged(int previousValue, int newValue)
    {
        _cashText.text = $"{newValue}";
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(timer);

        if (_inRound.Value)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                _timer.Value -= Time.deltaTime;
                if (_timer.Value < 0)
                {
                    _inRound.Value = false;
                    _orderSystem.ForEach(item => item.RoundOver());
                    _timer.Value = 0;
                    PlayEndGameBuzzerClientRpc();
                }
            }

            _timerText.text = ConvertTimer(_timer.Value);
            _cashText.text = _cash.Value + "";

        }
    }

    string ConvertTimer(float timer)
    {
        return TimeSpan.FromSeconds(timer).ToString("mm\\:ss");
    }

    public void AddCash()
    {
        _cash.Value += _burgerCashAmount;
    }

    public void StartGame()
    {
        if (_inRound.Value) return;

        _inRound.Value = true;
        _cash.Value = 0;
        _timer.Value = roundTimer;
        _orderSystem.ForEach(item => item.RoundStart());
    }

    public void EndGame()
    {
        if (!_inRound.Value) return;

        _inRound.Value = false;
        _timer.Value = roundTimer;
        _orderSystem.ForEach(item => item.RoundOver());
        PlayEndGameBuzzerClientRpc();
    }

    [ClientRpc]
    public void PlayEndGameBuzzerClientRpc()
    {
        _audioSource.PlayOneShot(_audioClip);
    }

    public void SetTimer(float timeAmount)
    {
        roundTimer = timeAmount;
        _timer.Value = roundTimer;
    }

    public void SetCashAmount(int amount)
    {
        Debug.Log($"[GameManager] Cash amount changed from {_burgerCashAmount} to {amount}");
        _burgerCashAmount = amount;
    }

    public void SetCashEarned(int amount)
    {
        Debug.Log($"[GameManager] Cash earned changed from {_cash.Value} to {amount}");
        _cash.Value = amount;
    }

    public void StartShift()
    {
        _gameMenuUI.StartShift();
    }

    public void EndShift()
    {
        _gameMenuUI.EndShift();
    }
}
