using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Menu : MonoBehaviourPunCallbacks
{
    public GameObject BG;
    public GameObject ErrorMessage;

    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject lobbyScreen;
    public GameObject entryScreen;
    public GameObject helpScreen;

    [Header("Main Screen")]
    public Button createRoomButton;
    public Button joinRoomButton;

    [Header("Lobby Screen")]
    public Button leaveRoomButton;
    public Button startGameButton;

    public TextMeshProUGUI playerListText;


    private void Start()
    {
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
    }

    public void Mute(bool x)
    {
        BG.SetActive(x);
    }

    void SetScreen(GameObject screen)
    {
        ErrorMessage.SetActive(false);
        // Deactivate all screenss
        mainScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        entryScreen.SetActive(false);
        helpScreen.SetActive(false);

        // Enable screen
        screen.SetActive(true);
    }

    public void OnCreateRoomButton (TMP_InputField roomNameInput)
    {
        NetworkManager.instance.CreateRoom(roomNameInput.text);
    }

    public void OnJoinRoomButton(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.JoinRoom(roomNameInput.text);
    }

    public void OnPlayerNameUpdate(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("" + PhotonNetwork.PlayerList.Length);
        if(PhotonNetwork.PlayerList.Length >= 5)
        {
            PhotonNetwork.LeaveRoom();
            ErrorMessage.SetActive(true);
            ErrorMessage.gameObject.GetComponent<TextMeshProUGUI>().text = "Room capacity is full.";
        }
        else
        {
            SetScreen(lobbyScreen);
            photonView.RPC("UpdateLobbyUI", RpcTarget.All);
        }


    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        ErrorMessage.SetActive(true);
        ErrorMessage.gameObject.GetComponent<TextMeshProUGUI>().text = "Could not join Room. Check Network connection.";
        //no room available
        //create a room (null as a name means "does not matter")
        //PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 20 });
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        ErrorMessage.SetActive(true);
        ErrorMessage.gameObject.GetComponent<TextMeshProUGUI>().text = "Could not Create Room. Try changing the Room name.";
        //Debug.Log(message);
        //base.OnCreateRoomFailed(returnCode, message);
        //TriesToConnectToRoom = false;
    }

    // Somewhere here error handling should be done.

    // This is called on every computer by default
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUI();
    }

    // Update the lobby UI
    [PunRPC]
    public void UpdateLobbyUI()
    {
        playerListText.text = "";

        // Display all players in lobby
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            playerListText.text += player.NickName + "\n";
        }

        // Only host can start the game
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.interactable = true;
        }
        else
        {
            startGameButton.interactable = false;
        }

    }

    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }

    public void OnStartGameButton()
    {
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "GameMain");
    }

    public void OnStartButton()
    {
        SetScreen(mainScreen);
    }

    public void OnBackButton()
    {
        SetScreen(entryScreen);
    }

    public void OnQuit()
    {
        Application.Quit();
    }

}
