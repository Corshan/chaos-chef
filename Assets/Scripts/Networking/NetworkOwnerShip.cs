using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkOwnerShip : NetworkBehaviour
{
    private string leftControllerName = "Right Controller";
    private string rightControlName = "Left Controller";
    private NetworkObject networkObject;

    private void Start()
    {
        networkObject = GetComponent<NetworkObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Equals(leftControllerName) || other.name.Equals(rightControlName))
        {
            GrabObjectServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void GrabObjectServerRpc(ulong clientId, ServerRpcParams serverRpcParams = default)
    {
        if (networkObject.OwnerClientId != clientId) networkObject.ChangeOwnership(clientId);
    }
}
