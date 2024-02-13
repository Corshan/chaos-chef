using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Grill : NetworkBehaviour
{
    private readonly List<BurgerHealth> _burgers = new();
    [SerializeField] private float _maxCookedAmount;
    [SerializeField] private float _speed;
    [SerializeField] private bool _debug;

    private void Update()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        foreach (var burger in _burgers)
        {
            if (burger.cookedAmount < 1)
            {
                burger.cookedAmount += _speed / _maxCookedAmount * Time.deltaTime;
                if(_debug) Debug.Log(burger.gameObject.name + " => " + burger.cookedAmount);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Burger")) return;

        var burger = other.GetComponentInChildren<BurgerHealth>();

        if(burger == null) return;

        _burgers.Add(burger);
        burger.IsVisable = true;
        
        if(_debug) Debug.Log("Trigger Enter " + name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Burger")) return;

        var burger = other.GetComponentInChildren<BurgerHealth>();

        if(burger == null) return;

        _burgers.Remove(burger);
        burger.IsVisable = false;

        if(_debug) Debug.Log("Trigger Exit " + name);
    }
}
