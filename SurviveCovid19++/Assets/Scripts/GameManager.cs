using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject MuteObject;
    // Change in Editor
    [Header("Total Amounts")]
    public int total_People = 0;
    public int total_Viruses = 0;
    public int total_Groceries = 14;
    public int total_Flags = 2;

    [Header("Stats")]
    public bool gameEnded;
    //public float timeToWin;
    public float invincibleDuration;
    //private float hatPickupTime;

    [Header("Players")]
    public string playerPrefabLocation;
    public Transform[] spawnPoints;
    public PlayerController[] players;
    public int playerWithHat;
    private int playersInGame;
    private int playerSelected;
    public float startTime;

    public int HealthMax = 100;
    public float shootMaxTimer = .8f;

    public int CurLevel;



    public InputField chatInput;
    public GameObject ChatPanel;
    public Transform ScrollView;
    public bool isMinimized;



    // instance
    public static GameManager instance;

    public GameObject CharacterSelectScreen;
    public GameObject CharacterSelectScreenInGame;
    public GameObject GameInstructions;

    [Header("Lobby Screen")]
    public Button civ;
    public Button doc;
    public Button san;
    public Button pol;

    public bool[] sel;



    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        sel = new bool[4];
        for (int i = 0; i < sel.Length; i++)
        {
            sel[i] = false;
        }

        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);

    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;
        if (playersInGame == PhotonNetwork.PlayerList.Length)
        {
            CharacterSelectScreen.SetActive(true);
            // Once all players have joined, a character selection screen should be shown to them.
            // Then spawn player method will have an integer input.
            // Then the spawn player will be connected to the character buttons.
            //SpawnPlayer(1);
        }
    }

    [PunRPC]
    void CharacterSelected()
    {
        playerSelected++;
        if (playerSelected == PhotonNetwork.PlayerList.Length)
        {
            FinalSpawn();
            // Once all players have joined, a character selection screen should be shown to them.
            // Then spawn player method will have an integer input.
            // Then the spawn player will be connected to the character buttons.
            //SpawnPlayer(1);
        }
    }

    [PunRPC]
    void WhichCharacterSelected(int x)
    {
        sel[x] = true;
    }

    public void SetGameInstructionsScreen(bool x)
    {
        GameInstructions.SetActive(x);
    }

    public void SetInGameCharacterSelect()
    {
        CharacterSelectScreenInGame.SetActive(false);
        
    }

    public void Mute()
    {
        MuteObject.SetActive(false);
    }

    public void UnMute()
    {
        MuteObject.SetActive(true);
    }

    public void MinimizeChat()
    {
       

            if (isMinimized)
            {
                Vector3 temp = ScrollView.localScale;
                temp.y = 1;
                ScrollView.localScale = temp;
                isMinimized = false;

            }
            else
            {
                Vector3 temp = ScrollView.localScale;
                temp.y = 0;
                ScrollView.localScale = temp;
                isMinimized = true;
            }
        
    }

    [PunRPC]
    public void DeactivateButton(int x)
    {
        if (x == -1)
        {
            //photonView.RPC("DeactivateButton", RpcTarget.AllBuffered, civ);
            civ.interactable = false;
        }
        if (x == 0)
        {
            //photonView.RPC("DeactivateButton", RpcTarget.AllBuffered, doc);
            doc.interactable = false;
            
        }
        if (x == 1)
        {
            san.interactable = false;
            //photonView.RPC("DeactivateButton", RpcTarget.AllBuffered, san);

        }
        if (x == 2)
        {
            //photonView.RPC("DeactivateButton", RpcTarget.AllBuffered, pol);
            pol.interactable = false;
        }
    }

    public void FinalSpawn()
    {
        CharacterSelectScreen.SetActive(false);
    }

    public void SpawnPlayer(int x)
    {
        if (sel[x] == false)
        {


            photonView.RPC("WhichCharacterSelected", RpcTarget.AllBuffered, x);

            photonView.RPC("CharacterSelected", RpcTarget.AllBuffered);

            photonView.RPC("DeactivateButton", RpcTarget.AllBuffered, x);



            //GameObject playerObject = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);

            GameObject playerObject = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[x + 1].position, Quaternion.identity);
            PlayerController playerScript = playerObject.GetComponent<PlayerController>();



            // Initialize the player
            playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);

            if (photonView.IsMine)
            {


                playerScript.ChatName = PhotonNetwork.LocalPlayer.NickName;
            }
                //playerScript.chatInput = chatInput;
            //playerScript.powerChange(x);
            playerScript.photonView.RPC("powerChange", RpcTarget.All, x);
        }


        //playerScript.chatInput = chatInput;
        //playerScript.ChatPanel = ChatPanel;
        //powerChangeM()
        //playerScript.photonView.RPC("powerChange", RpcTarget.All, playerScript.id, x);
    }

    public void ScreenPowerChange(int x)
    {
        photonView.RPC("powerChangeM", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, x);
        photonView.RPC("GetCoin", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, -50);
        CharacterSelectScreenInGame.SetActive(false);
    }

    public PlayerController GetPlayer(int playerId)
    {
            
        return players.First(x => x.id == playerId);
        
    }

    public PlayerController GetPlayer (GameObject playerObj)
    {
        return players.First(x => x.gameObject == playerObj);
    }

    [PunRPC]
    public void RestoreHealthM(int playerId)
    {
        GetPlayer(playerId).RestoreHealth();
    }
    /*[PunRPC]
    public void GiveHat(int playerId, bool initialGive)
    {
        if (!initialGive)
        {
            GetPlayer(playerWithHat).SetHat(false);
        }

        playerWithHat = playerId;
        GetPlayer(playerId).SetHat(true);
        hatPickupTime = Time.time;
    }*/

    [PunRPC]
    public void Add_Individual_Goal(int x)
    {
        foreach(PlayerController player in players)
        {
            player.Add_Individual_Goal(x);
        }
    }

    [PunRPC]
    public void AddCollectible()
    {
        foreach (PlayerController player in players)
        {
            player.Add_Collectible();
        }
    }

    [PunRPC]
    public void AddInfected()
    {
        foreach (PlayerController player in players)
        {
            player.Add_Infected_Total();
        }
    }

    [PunRPC]
    public void SubtractInfected()
    {
        foreach (PlayerController player in players)
        {
            player.Subtract_Infected_Total();
        }
    }

    [PunRPC]
    public void GetCoin(int playerId)
    {
        GetPlayer(playerId).GetCoin_player(1);
    }

    [PunRPC]
    public void GetCoin(int playerId, int num)
    {
        GetPlayer(playerId).GetCoin_player(num);
    }

    [PunRPC]
    public void powerChangeM(int playerId, int x)
    {
        try
        {

            GetPlayer(playerId).powerChange(x);
        }
        catch
        {

        }
    }

    [PunRPC]
    public void RestoreAmmoM(int playerId, int x)
    {
        try
        {

            GetPlayer(playerId).RestoreAmmo(x);
        }
        catch
        {

        }
    }

    [PunRPC]
    public void GetInfectedM(int playerId)
    {
        GetPlayer(playerId).Infect();
        
    }

    [PunRPC]
    public void GetDisinfectedM(int playerId)
    {
        GetPlayer(playerId).Disinfect();

    }

    [PunRPC]
    public void GetMaskSanM(int playerId, int x)
    {
        GetPlayer(playerId).GetMaskSan(x);

    }

    [PunRPC]
    public void DeleteMaskSanM(int playerId, int x)
    {
        GetPlayer(playerId).DeleteMaskSan(x);

    }

    [PunRPC]
    public void Power(int playerId, int powerId)
    {

         GetPlayer(playerId).Power(powerId);

    }

    [PunRPC]
    public void AnimatePlayer(int playerId, int AnimationId)
    {
        try
        {
            PlayerController pl = GetPlayer(playerId);
            pl.SetAnimation(AnimationId);
        }
        catch
        {

        }


    }

    [PunRPC]
    public void Destroy(int viewId, int time)
    {
       
        try
        {
            Destroy(PhotonView.Find(viewId).gameObject, time);
        }
        catch
        {

        }
       
        
    }

    [PunRPC]
    public void Destroy(int viewId, int playerId, int time)
    {

        GetPlayer(playerId).DestroyFlagEffect();
        Destroy(PhotonView.Find(viewId).gameObject, time);
    }


    [PunRPC]
    public void DisinfectCrowd(int viewId)
    {
        PhotonView.Find(viewId).gameObject.GetComponent<CrowdController>().GetDisinfected();
    }

    [PunRPC]
    public void ResetMovementM(int viewId)
    {
        //Invoke("ResetMovementMain", time);
        GetPlayer(viewId).ResetMovement();
        //PhotonView.Find(viewId).gameObject.GetComponent<PlayerController>().ResetMovement();
    }


    public void LeaveMatch()
    {

        gameEnded = true;
        Invoke("GoBackToMenu", 1.0f);
    }

    /*public bool CanGetHat()
    {
        if(Time.time > hatPickupTime + invincibleDuration)
        {
            return true;
        }
        else
        {
            return false;
        }
    }*/

    [PunRPC]
    void WinGame()
    {
        gameEnded = true;
        //PlayerController player = GetPlayer(playerId);

        // UI
        GameUI.instance.SetWinText();

        Invoke("GoBackToMenu", 3.0f);
    }

    [PunRPC]
    void LoseGame()
    {
        gameEnded = true;
        //PlayerController player = GetPlayer(playerId);

        // UI
        GameUI.instance.SetLoseText();

        Invoke("Level1", 3.0f);
    }

    [PunRPC]
    void NextLevel()
    {
        gameEnded = true;
        //PlayerController player = GetPlayer(playerId);
        
        // UI
        GameUI.instance.SetNextLevel();

        Invoke("Level2", 3.0f);
    }

    [PunRPC]
    public void SendMessageToChat(int playerId, string text)
    {
        string message = playerId + " > " + text;
        GetPlayer(playerId).SendMessageToChat(message);

    }

    [PunRPC]
    public void SendMessageToChatSys(int playerId, string text)
    {
        string message = "System > " + text;
        try
        {
            GetPlayer(playerId).SendMessageToChat(message);
        }
        catch
        {

        }
        

    }

    /*[PunRPC]
    public void SendMessageToAllChat(int playerId, string text)
    {
        foreach (PlayerController player in players)
        {
            player.SendMessageToChat("System> " +text);
        }
        //string message = GetPlayer(playerId).ChatName + "> " + text;
        //GetPlayer(playerId).SendMessageToChat(message);

    }*/

    public void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");
    }

    void Level2()
    {
        NetworkManager.instance.ChangeScene("GameMain");
    }

    void Level1()
    {
        NetworkManager.instance.ChangeScene("GameMain");
    }



    //


}

