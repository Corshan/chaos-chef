using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ItemCheck : NetworkBehaviour
{
    [SerializeField] private OrderSystem _orderSystem;
    private GameObject _currentPlate;
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.OnServerStarted += OnSelectEntered;
    }

    private void OnSelectEntered()
    {
        // GetComponent<XRSocketInteractor>().showInteractableHoverMeshes = false;
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
                _orderSystem.Innit();
            }
        }
    }

    [System.Obsolete]
    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        OnSelectEnteredServerRpc(args.interactable.gameObject.name);
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnSelectEnteredServerRpc(string name)
    {
        // Debug.Log("Check burger " + name);
        // // _currentPlate.SetActive(false);

        // List<PlateItemTags> items = _currentPlate.GetComponent<PlateItems>().GetItems();

        // bool result = _orderSystem.CheckOrder(items);

        // Debug.Log(result);

        // if (result)
        // {
        //     _currentPlate.GetComponent<NetworkObject>().Despawn();
        //     _orderSystem.Innit();
        // }
    }
}
