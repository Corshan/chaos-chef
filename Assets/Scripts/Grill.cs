using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Grill : NetworkBehaviour
{
    private readonly List<BurgerHealth> _burgers = new();
    [SerializeField] private float _maxCookedAmount;
    [SerializeField] private float _speed;

    private void Update() {
        if(!IsServer) return;
        
        foreach(var burger in _burgers){
            burger.cookedAmount += _speed / _maxCookedAmount * Time.deltaTime;
        }    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Burger")) return;

        var burger = other.GetComponentInChildren<BurgerHealth>();

        _burgers.Add(burger);
        burger.IsVisable = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Burger")) return;

        var burger = other.GetComponentInChildren<BurgerHealth>();

        _burgers.Remove(burger);
        burger.IsVisable = false;
    }
}
