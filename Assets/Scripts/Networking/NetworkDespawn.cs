using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkDespawn : NetworkBehaviour
{
    [SerializeField] private GameObject _prefab;

    public override void OnNetworkDespawn(){
        _prefab.SetActive(false);
        
        base.OnNetworkDespawn();
    }
}
