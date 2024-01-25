using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobbySystem
{
    public class LobbyManager : MonoBehaviour, ILobbyManager
    {
        public static LobbyManager Singleton { get; private set; }
        public static string joinCode { get; private set; }

        private void Awake()
        {
            if (Singleton != null && Singleton != this) Destroy(this);
            else Singleton = this;
        }

        public List<string> GetAllLobbies()
        {
            throw new System.NotImplementedException();
        }
    }
}

