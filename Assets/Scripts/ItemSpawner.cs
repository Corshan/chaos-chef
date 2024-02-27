using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemSpawner : NetworkBehaviour
{
    [SerializeField] private Transform _spawnTransform;
    [SerializeField] private GameObject _prefab;

    private int _counter = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SpawnItemServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnItemServerRpc()
    {
        GameObject go = Instantiate(_prefab, _spawnTransform.position, Quaternion.identity);
        go.name += _counter;
        _counter++;
        go.GetComponent<NetworkObject>().Spawn();
    }
}
