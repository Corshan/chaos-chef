using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkOwnerShip : NetworkBehaviour
{
    [SerializeField] private string leftControllerName, rightControlName;

    private void OnTriggerEnter(Collider other) {
        if(other.name.Equals(leftControllerName) || other.name.Equals(rightControlName)){
            GrabObjectServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void GrabObjectServerRpc(ulong clientId, ServerRpcParams serverRpcParams = default){
        GetComponent<NetworkObject>().ChangeOwnership(clientId);
    }
}
