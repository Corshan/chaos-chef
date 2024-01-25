using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobbySystem
{
    public interface ILobbyManager
    {
        List<string> GetAllLobbies();
    }
}

