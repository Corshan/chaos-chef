using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace LobbySystem
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Singleton { get; private set; }
        public static string joinCode { get; private set; }
        public static Lobby currentLobby;
        private static UnityTransport transport;
        private int maxConnection = 20;

        // public static List<Lobby>

        private async void Awake()
        {
            if (Singleton != null && Singleton != this) Destroy(this);
            else
            {
                Singleton = this;
                DontDestroyOnLoad(this);

                await UnityServices.InitializeAsync();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                transport = GameObject.Find("Network Manager").GetComponent<UnityTransport>();
            }
        }

        public async Task<QueryResponse> GetAllLobbiesAsync()
        {
            try
            {
                return await Lobbies.Instance.QueryLobbiesAsync();
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return null;
            }
        }

        public async void Join(Lobby lobby)
        {
            currentLobby = lobby;
            string replayJoinCode = currentLobby.Data["JOIN_CODE"].Value;

            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(replayJoinCode);

            transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);
            NetworkManager.Singleton.StartClient();
        }

        public async void QuickJoin()
        {
            try
            {
                currentLobby = await Lobbies.Instance.QuickJoinLobbyAsync();
                string replayJoinCode = currentLobby.Data["JOIN_CODE"].Value;

                JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(replayJoinCode);

                transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
                    allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);
                NetworkManager.Singleton.StartClient();
            }catch(RelayServiceException e){
                Debug.Log(e);
                throw e;
            }
        }

        public async void CreateServer()
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);
            string newJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false;
            lobbyOptions.Data = new Dictionary<string, DataObject>();
            DataObject dataObject = new DataObject(DataObject.VisibilityOptions.Public, newJoinCode);
            lobbyOptions.Data.Add("JOIN_CODE", dataObject);

            currentLobby = await Lobbies.Instance.CreateLobbyAsync("Lobby name", maxConnection, lobbyOptions);

            Debug.Log(newJoinCode);
            NetworkManager.Singleton.StartServer();
        }
    }


}

