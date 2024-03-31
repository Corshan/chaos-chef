using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField][Range(60, 600)] private float roundTimer = 300;
    [SerializeField] private NetworkVariable<bool> _inRound = new(false);
    // private float timer = 0;
    private NetworkVariable<float> _timer = new(0);
    private NetworkVariable<int> _cash = new(0);

    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _cashText;
    [SerializeField][Range(10, 100)] private int _burgerCashAmount = 10;
    [SerializeField] private List<OrderSystem> _orderSystem;

    // Start is called before the first frame update
    void Start()
    {
        _timer.Value = roundTimer;
        _timerText.text = ConvertTimer(_timer.Value);
        _cashText.text = _cash.Value + "";
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
                    // _orderSystem.ClearOrderAndDisplay();
                    _orderSystem.ForEach(item => item.RoundOver());
                    _timer.Value = 0;
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

}
