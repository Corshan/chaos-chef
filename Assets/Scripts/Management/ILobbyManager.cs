using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace LobbySystem
{
    public interface ILobbyManager
    {
        public Task<QueryResponse> GetAllLobbiesAsync();
    }
}

