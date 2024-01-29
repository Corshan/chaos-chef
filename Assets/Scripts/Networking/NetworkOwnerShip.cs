using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkOwnerShip : NetworkBehaviour
{
    private readonly string _leftControllerName = "Left Controller";
    private readonly string _rightControlName = "Right Controller";
    private NetworkObject _networkObject;

    private void Start()
    {
        _networkObject = GetComponent<NetworkObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Equals(_leftControllerName) || other.name.Equals(_rightControlName))
        {
            GrabObjectServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void GrabObjectServerRpc(ulong clientId, ServerRpcParams serverRpcParams = default)
    {
        if (_networkObject.OwnerClientId != clientId) _networkObject.ChangeOwnership(clientId);
    }
}
