using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemCutting : NetworkBehaviour
{
    [SerializeField] private int maxHits = 5;
    [SerializeField] private GameObject slicedPrefab;
    [SerializeField] private int amountToSpawn = 1;
    [SerializeField] private bool isBurger = false;
    [SerializeField] private List<GameObject> _burgerBuns;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;
    private int _currentHit = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (_currentHit == maxHits)
        {
            if (isBurger)
            {
                foreach (var item in _burgerBuns)
                {
                    GameObject go = Instantiate(item, transform.position, Quaternion.identity);
                    go.GetComponent<NetworkObject>().Spawn();
                }
            }
            else
            {
                for (int i = 0; i < amountToSpawn; i++)
                {
                    GameObject go = Instantiate(slicedPrefab, transform.position, Quaternion.identity);
                    go.GetComponent<NetworkObject>().Spawn();
                }
            }
            PlayAudioClientRpc();
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }

        if (other.CompareTag("knife"))
        {
            _currentHit++;
            PlayAudioClientRpc();
        }

    }

    [ClientRpc]
    public void PlayAudioClientRpc()
    {
        _audioSource.PlayOneShot(_audioClip);
    }
}
