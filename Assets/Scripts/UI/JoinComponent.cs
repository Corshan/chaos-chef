using System.Collections;
using System.Collections.Generic;
using LobbySystem;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class JoinComponent : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI joinCodeText;
    public Lobby lobby {get; set;}

    // Start is called before the first frame update
    void Start()
    {
        joinCodeText.SetText(lobby.Data["JOIN_CODE"].Value);
        Debug.Log(lobby.Data["JOIN_CODE"].Value);
    }

    public void JoinGame(){
        LobbyManager.Singleton.Join(lobby);
    }
}
