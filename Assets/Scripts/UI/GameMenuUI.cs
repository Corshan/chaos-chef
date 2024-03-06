using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameMenuUI : NetworkBehaviour
{
    [SerializeField] private GameManager _gameManager;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGameEvent()
    {
        StartGameEventServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartGameEventServerRpc()
    {
        _gameManager.StartGame();
    }
}
