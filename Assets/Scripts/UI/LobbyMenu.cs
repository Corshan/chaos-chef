using System.Collections;
using System.Collections.Generic;
using LobbySystem;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject joinComponent;

    private List<Lobby> lobbies;
    // Start is called before the first frame update
    async void Start()
    {
        var response = await LobbyManager.Singleton.GetAllLobbiesAsync();
        lobbies = response.Results;

        for (int i = 0; i < lobbies.Count; i++)
        {
            GameObject clone = Instantiate(joinComponent, this.transform);

            clone.GetComponent<JoinComponent>().lobby = lobbies[i];
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
