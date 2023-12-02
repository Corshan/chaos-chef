using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner: MonoBehaviour
{
    [SerializeField] private Transform _spawnTransform;
    [SerializeField] private Object _prefab;

    private void OnTriggerEnter(Collider other)
    {
        // if (other.tag.Equals("Player"))
        // {
            Instantiate(_prefab, _spawnTransform.position, _spawnTransform.rotation);
        // }
    }

}
