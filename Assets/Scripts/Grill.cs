using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Grill : NetworkBehaviour
{
    private readonly List<BurgerHealth> _burgers = new();
    private NetworkVariable<bool> _isPlaying = new(false);
    [SerializeField][Range(1, 100)] private float _maxCookedAmount = 100, _speed = 10;
    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;

    private void Start()
    {
        _audioSource.mute = true;
    }

    private void Update()
    {
        _audioSource.mute = _isPlaying.Value;


        if (!NetworkManager.Singleton.IsServer) return;

        foreach (var burger in _burgers)
        {
            if (burger.cookedAmount < 1)
            {
                burger.cookedAmount += _speed / _maxCookedAmount * Time.deltaTime;
            }
        }

        _isPlaying.Value = _burgers.Count <= 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Burger")) return;

        var burger = other.GetComponentInChildren<BurgerHealth>();

        if (burger == null) return;

        _burgers.Add(burger);
        burger.IsVisable = true;
        // if (NetworkManager.Singleton.IsServer) _isPlaying.Value = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Burger")) return;

        var burger = other.GetComponentInChildren<BurgerHealth>();

        if (burger == null) return;

        _burgers.Remove(burger);
        burger.IsVisable = false;

        // if (NetworkManager.Singleton.IsServer) _isPlaying.Value = false;
    }
}
