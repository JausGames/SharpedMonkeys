using MLAPI;
using UnityEngine;
using System.Collections.Generic;
using MLAPI.Messaging;
using MLAPI.Connection;
using System.Text;

public class OnlinePlayerManager : MonoBehaviour
{
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();

            //SubmitNewPosition();
        }

        GUILayout.EndArea();
    }

    void StartButtons()
    {
        if (GUILayout.Button("Host")) Host();
        if (GUILayout.Button("Client")) Client();
        if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
    }
    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;

    }

    private void OnDestroy()
    {
        // Prevent error in the editor
        if (NetworkManager.Singleton == null) { return; }

        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }
    public void Host()
    {
        // Hook up password approval check
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost(PlayerManager.GetInstance().GetSpawnPosition()[0], PlayerManager.GetInstance().GetSpawnRotation()[0]);
    }

    public void Client()
    {
        // Set password ready to send to the server to validate
        NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes("put credential here");
        NetworkManager.Singleton.StartClient();
    }
    public void Leave()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.StopHost();
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.StopClient();
        }

        //passwordEntryUI.SetActive(true);
        //leaveButton.SetActive(false);
    }

    private void HandleServerStarted()
    {
        // Temporary workaround to treat host as client
        if (NetworkManager.Singleton.IsHost)
        {
            HandleClientConnected(NetworkManager.Singleton.ServerClientId);
        }
    }
    private void HandleClientConnected(ulong clientId)
    {
        // Are we the client that is connecting?
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            var list = NetworkManager.Singleton.ConnectedClientsList;
            var listId = new List<ulong>();
            foreach (MLAPI.Connection.NetworkClient client in list)
            {
                listId.Add(client.ClientId);
            }
            NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out NetworkClient networkClient);
            networkClient.PlayerObject.TryGetComponent<Player>(out Player newPlayer);
            Debug.Log("HandleClientConnected : team nb = " + ((NetworkManager.Singleton.ConnectedClients.Count + 1) % 2), this);


            //PlayerManager.GetInstance().FindPlayers(listId);
            SubmitAddPlayerServerRpc(clientId);

            // passwordEntryUI.SetActive(false);
            //leaveButton.SetActive(true);
        }
    }
    [ServerRpc]
    void SubmitAddPlayerServerRpc(ulong clientId, ServerRpcParams rpcParams = default)
    {
        AddPlayerClientRpc(clientId);
    }
    [ClientRpc]
    void AddPlayerClientRpc(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId) return;
        PlayerManager.GetInstance().AddPlayerToList(clientId);
    }
    private void HandleClientDisconnect(ulong clientId)
    {
        // Are we the client that is disconnecting?
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            //passwordEntryUI.SetActive(true);
            //leaveButton.SetActive(false);
        }
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId, MLAPI.NetworkManager.ConnectionApprovedDelegate callback)
    {
        //string password = Encoding.ASCII.GetString(connectionData);

        //bool approveConnection = password == passwordInputField.text;
        bool approveConnection = true;

        var pos = NetworkManager.Singleton.ConnectedClients.Count % 4;

        Debug.Log("ConnectionApproval, ApprovalCheck : position = " + pos);
        callback(true, null, approveConnection, PlayerManager.GetInstance().GetSpawnPosition()[pos], PlayerManager.GetInstance().GetSpawnRotation()[pos]);
    }
    static void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }
    /*static void SubmitNewPosition()
    {
        if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request Position Change"))
        {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId,
                out var networkedClient))
            {
                var controller = networkedClient.PlayerObject.GetComponent<PlayerController>();
                if (controller)
                {
                    controller.Move();
                }
            }
        }
    }*/

}

