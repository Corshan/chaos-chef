using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ItemCheck : NetworkBehaviour
{
    [SerializeField] private OrderSystem _orderSystem;
    [SerializeField] private GameManager _gameManager;
    private GameObject _currentPlate;
    // Start is called before the first frame update
    void Start()
    {
     
    }


    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (other.CompareTag("plate"))
        {
            List<PlateItemTags> items = other.GetComponentInChildren<PlateItems>().GetItems();

            bool result = _orderSystem.CheckOrder(items);

            Debug.Log(result);

            if (result)
            {
                other.GetComponent<NetworkObject>().Despawn();
                _orderSystem.TriggerOrderDone();
                _gameManager.AddCash();
            }
        }
    }
}
