using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;

    private void Awake()
    {
        // If an instance already exists destroy this
        if(instance!=null && instance != this)
        {
            gameObject.SetActive(false);
        }
        else
        {
            // set this instance
            // Carry this instance to all levels.
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
       
    }

    // Connect to master server;
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    // Creates a room
    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
    }

    // Joins a room
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
    
    // Change scene 
    // We want to call this on all computers
    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}
