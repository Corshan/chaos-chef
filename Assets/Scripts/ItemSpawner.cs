using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemSpawner : NetworkBehaviour
{
    [SerializeField] private Transform _spawnTransform;
    [SerializeField] private GameObject _prefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            // GameObject clone = Instantiate(_prefab, _spawnTransform.position, Quaternion.identity);

            // clone.GetComponent<NetworkObject>().Spawn();
            SpawnItemServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnItemServerRpc()
    {
        GameObject go = Instantiate(_prefab, _spawnTransform.position, Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn();

    }

}
