using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCutting : MonoBehaviour
{
    [SerializeField] private int maxHits = 5;
    [SerializeField] private GameObject slicedPrefab;
    [SerializeField] private int amountToSpawn = 1;
    private int _currentHit = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (_currentHit == maxHits)
        {
            for (int i = 0; i < amountToSpawn; i++)
            {
                Instantiate(slicedPrefab, transform.position, transform.rotation);

            }
            Destroy(gameObject);
        }

        if (other.tag.Equals("knife"))
        {
            _currentHit++;
        }
    }
}
