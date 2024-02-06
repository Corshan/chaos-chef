using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemCutting : NetworkBehaviour
{
    [SerializeField] private int maxHits = 5;
    [SerializeField] private GameObject slicedPrefab;
    [SerializeField] private int amountToSpawn = 1;
    private int _currentHit = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (_currentHit == maxHits)
        {
            for (int i = 0; i < amountToSpawn; i++)
            {
                GameObject go = Instantiate(slicedPrefab, transform.position, Quaternion.identity);
                go.GetComponent<NetworkObject>().Spawn();
            }
            Destroy(gameObject);
        }

        if (other.CompareTag("knife")) _currentHit++;

    }
}
