using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class GameUI : MonoBehaviour
{
    public PlayerUIContainer[] playerContainers;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI loseText;
    public TextMeshProUGUI totalInfected;
    public TextMeshProUGUI TotalCollectible;


    
    public TextMeshProUGUI GroceriesCollected;
    public TextMeshProUGUI VirusDestroyed;
    public TextMeshProUGUI flagsDestroyed;



    public static GameUI instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitializePlayerUI();
    }

    void InitializePlayerUI()
    {

        for(int x = 0; x < playerContainers.Length; ++x)
        {
            PlayerUIContainer container = playerContainers[x];

            if(x < PhotonNetwork.PlayerList.Length)
            {
                container.obj.SetActive(true);
                container.nameText.text = PhotonNetwork.PlayerList[x].NickName;
                container.hatTimeSlider.maxValue = GameManager.instance.HealthMax;
                //container.ShootTimerSlider.maxValue = GameManager.instance.shootMaxTimer;

                container.numShootsLeft.text = "10";

            }
            else
            {
                container.obj.SetActive(false);
            }
        }
    }

    void Update()
    {
        UpdatePlayerUI();
    }

    void UpdatePlayerUI()
    {
        for (int x = 0; x < GameManager.instance.players.Length; ++x)
        {
            if (GameManager.instance.players[x] != null)
            {
                playerContainers[x].hatTimeSlider.value = GameManager.instance.players[x].health;
                

                playerContainers[x].coins.text = "" + GameManager.instance.players[x].coins;

                if(GameManager.instance.players[x].curPower == 0)
                {
                    playerContainers[x].numShootsLeft.text = "" + GameManager.instance.players[x].ammo0;
                } else if (GameManager.instance.players[x].curPower == 1)
                {
                    playerContainers[x].numShootsLeft.text = "" + GameManager.instance.players[x].ammo1;
                }
                else
                {
                    playerContainers[x].numShootsLeft.text = "-";
                }


                TotalCollectible.text = "" + GameManager.instance.players[x].totalCollected + "/" + GameManager.instance.players[x].maxCollected;
                totalInfected.text = "" + GameManager.instance.players[x].totalInfectedPeople; // Total People Left

                GroceriesCollected.text = "" + GameManager.instance.players[x].GroceriesCollected + "/" + GameManager.instance.total_Groceries;
                VirusDestroyed.text = "" + GameManager.instance.players[x].VirusDestroyed + "/" + GameManager.instance.total_Viruses;
                flagsDestroyed.text = "" + GameManager.instance.players[x].flagsDestroyed + "/2";


                if (GameManager.instance.players[x].isInfected)
                {
                    Color col = playerContainers[x].infection.color;
                    col.a = 1f;
                    playerContainers[x].infection.color = col;
                }
                else
                {
                    Color col = playerContainers[x].infection.color;
                    col.a = 0f;
                    playerContainers[x].infection.color = col;

                }
            }
            
        }
    }

    public void SetWinText()
    {
        winText.gameObject.SetActive(true);
        winText.text = "Congrats! You Won!";
    }

    public void SetLoseText()
    {
        loseText.gameObject.SetActive(true);
        //loseText.text = "Try Again";
    }

    public void SetNextLevel()
    {
        winText.gameObject.SetActive(true);
        winText.text = "Loading Next Level ... ";
    }
}

[System.Serializable]
public class PlayerUIContainer
{
    public GameObject obj;
    public TextMeshProUGUI nameText;
    public Slider hatTimeSlider;
    public TextMeshProUGUI coins;
    public Image infection;
    public TextMeshProUGUI numShootsLeft;
    //public Slider ShootTimerSlider;

}
