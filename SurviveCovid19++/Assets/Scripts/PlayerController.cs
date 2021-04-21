using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    

    public GameObject textObject;
    public int maxMessages = 25;

    public InputField chatInput;
    public GameObject ChatPanel;

    [SerializeField]
    List<Message> messageList = new List<Message>();

    public int id;
    public Vector3 offset;

    public string ChatName;
    
    [Header("Info")]
    public float moveSpeed;
    public float jumpForce;
    public GameObject hatObject;

    public GameObject virusObject;
    public int coins = 0;
    public int totalCoins;

    // Masks and Sanitizers

    public bool hasMask = false;
    public bool hasSanitizer = false;

    public GameObject _Mask;
    public GameObject _Sanitizer;

    // GameObjectives
    public int totalCollected = 0;
    public int flagsDestroyed = 0;
    public int GroceriesCollected = 0;
    public int VirusDestroyed = 0;
    public int PeopleDisinfected = 0;


    // Should Be completed


    public int maxCollected = 10;
    public bool isInfected = false;
    public int curPower;
    public int health = 100;
    public ParticleSystem crowdDisinfect;
    public int curLevel;
    public int totalInfectedPeople;
    public GameObject Arrow;



    [HideInInspector]
    //public float curHatTime;
    private Vector3 lookPosition;


    [Header("Components")]
    public Rigidbody rig;
    public Player photonPlayer;
    public ParticleSystem power0_prefab;
    public ParticleSystem power1_prefab;
    public ParticleSystem power2_prefab;

    private ParticleSystem DestroyOnReset;


    public LayerMask virusL, crowdL, targetVaccine, targetHospital;

    public ParticleSystem EnemyDeath;

    public ParticleSystem flagDestroy;
    public ParticleSystem CollectCoin;
    public ParticleSystem CollectVaccine;
    public ParticleSystem InfectCrowd;

    public Transform PowerTransform;

    public GameObject pauseScreen;


    [Header("Costumes")]
    public GameObject costume_Sanitizer;
    public GameObject costume_Enforcer;
    public GameObject costume_Doctor;

    private CharacterChange characterChange;
    public ParticleSystem powerGain;

    public AudioSource audio;
    [Header("Audio")]
    public AudioClip coinCollectSound;
    public AudioClip vaccineCollectSound;
    public AudioClip powerCollectSound;
    public AudioClip InfectSound;

    public AudioClip DestroyFlag;

    public AudioClip VirusPop;

    public AudioClip PowerSound;

    public AudioClip FootStep;

    public float footstepTimer;
    public float footstepTimerMax = 0.1f;


    public float healthTimer = 5f;

    public bool inPause = false;



    // Animation
    private Animator playerAnimator;

    [Header("AmmoAndCoolDown")]
    public int ammo0 = 10;
    public int ammo1 = 10;
    public float ShootTimer = .8f;
    public bool canShoot = true;

    //public bool characterSelectActive = false;



    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;

        //Debug.Log(id);
        //Debug.Log(GameManager.instance.players.Length);



        GameManager.instance.players[id - 1] = this;

        //give the first player the hat
        /*if(id == 1)
        {
            GameManager.instance.GiveHat(id, true);
        }*/
        if (!photonView.IsMine)
        {
            rig.isKinematic = true;
        }

    }

    private void Start()
    {
        // This has to be selected from the screen
        //curPower = -1;

        // This has to be in the function of spawning
        characterChange = gameObject.GetComponent<CharacterChange>();
        playerAnimator = GetComponent<Animator>();

        InputField[] firstList = GameObject.FindObjectsOfType<InputField>();
        //List<Object> finalList = new List<Object>();

        for(var i = 0; i < firstList.Length; i++)
        {
            if (firstList[i].gameObject.CompareTag("ChatInput"))
            {
                chatInput = firstList[i];
            }
        }

        ChatPanel = GameObject.FindGameObjectWithTag("ChatPanel");
        //List<Object> finalList = new List<Object>();

        offset = new Vector3(-45f, 27f, -45f);

        curLevel = GameManager.instance.CurLevel;


        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.instance.photonView.RPC("SendMessageToChatSys", RpcTarget.All, id, "WASD-To Move. Spave-To Jump.");
            GameManager.instance.photonView.RPC("SendMessageToChatSys", RpcTarget.All, id, "Left Mouse Button - To Shoot.");
            GameManager.instance.photonView.RPC("SendMessageToChatSys", RpcTarget.All, id, "Follow the Golden arrow to find all 10 vaccine parts.");
            GameManager.instance.photonView.RPC("SendMessageToChatSys", RpcTarget.All, id, "Complete individual goals.");
        }

            //offset = new Vector3(0f, 4.58f, -3.9f);
            if (photonView.IsMine)
        {
            audio = GetComponent<AudioSource>();
            //cams = Camera.main.transform;
            Camera.main.transform.position = this.transform.position + offset;

            try
            {
                pauseScreen = GameObject.Find("Canvas").transform.GetChild(4).gameObject;
            }
            catch
            {

            }

            //powerChange(curPower);
            //Camera.main.transform.parent = this.transform;

        }
        else
        {
            Arrow.SetActive(false);
        }
        
    }

    private void LateUpdate()
    {
        if (photonView.IsMine)
        {
            //gob.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            /*Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, 1000))
            {
                lookPosition = hit.point;
                lookPosition.y = transform.position.y;
                
                transform.LookAt(lookPosition);
            }*/
            //transform.LookAt(gob.transform);
            Camera.main.transform.position = this.transform.position + offset;
        }
    }

    

    private void Update()
    {

        //Chat System


        /*if (!chatInput.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //GameManager.instance.photonView.RPC("AnimatePlayer", RpcTarget.All, id, 0);
                SendMessageToChat("You Pressed the space bar!");
                Debug.Log("Space");
            }
        }*/
        if (photonView.IsMine)
        {
            if (chatInput.text != "")
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    GameManager.instance.photonView.RPC("SendMessageToChat", RpcTarget.All, id, chatInput.text);
                    //SendMessageToChat(chatInput.text);
                    //Debug.Log("Enter Text");
                    chatInput.text = "";
                }
            }
        }
        


        if (PhotonNetwork.IsMasterClient)
        {
            if(totalCollected >= maxCollected && GroceriesCollected >= GameManager.instance.total_Groceries && flagsDestroyed >= 2 && totalInfectedPeople <= 0 && VirusDestroyed >= GameManager.instance.total_Viruses && !GameManager.instance.gameEnded)
            {
                GameManager.instance.gameEnded = true;
                if(curLevel == 1)
                {
                    GameManager.instance.photonView.RPC("WinGame", RpcTarget.All);
                    
                    //GameManager.instance.photonView.RPC("NextLevel", RpcTarget.All);
                }
                else
                {
                    GameManager.instance.photonView.RPC("WinGame", RpcTarget.All);
                   
                }
                
            }
        }

        CheckHealth();

        if (photonView.IsMine)
        {
            
            if (isInfected)
            {
                if (healthTimer < 0f)
                {
                    ParticleSystem death = Instantiate(InfectCrowd, transform.position, Quaternion.Euler(transform.up)) as ParticleSystem;
                    Destroy(death, 2);
                    health -= 20;
                    healthTimer = 5f;
                }
                healthTimer -= Time.deltaTime;
            }

            if (!chatInput.isFocused && !GameManager.instance.CharacterSelectScreenInGame.activeSelf)
            {
                if(GameManager.instance.players.Length < 4)
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        
                        GameManager.instance.CharacterSelectScreenInGame.SetActive(true);
                        //characterSelectActive = true;
                        

                    }
                    
                    
                }

                if(totalCollected < maxCollected)
                {
                    FindNextTarget();
                }
                else
                {
                    Arrow.SetActive(false);
                }

                if (isInfected)
                {
                    FindHospital();
                }
                
                Move();

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    TryJump();
                }


                if (Input.GetMouseButton(0))
                {
                    if (curPower != 2)
                    {
                        GameManager.instance.photonView.RPC("Power", RpcTarget.All, id, curPower);
                    }
                }
            }



           


                
            
        }

        
    }

    

    public void RestoreHealth()
    {
        if (isInfected)
        {
            Disinfect();
        }
        health = 100;
    }

    public void CheckHealth()
    {
        if (PhotonNetwork.IsMasterClient && !GameManager.instance.gameEnded)
        {
            if (health <= 0)
            {
                GameManager.instance.gameEnded = true;
                GameManager.instance.photonView.RPC("LoseGame", RpcTarget.All);
                
            }  
        }

    }

    void FindNextTarget()
    {
        if (photonView.IsMine)
        {


            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 50000f, targetVaccine);

            //GameManager.instance.photonView.RPC("GetDisinfected", RpcTarget.All, id);

            if (hitColliders == null)
            {
                Arrow.SetActive(false);
            }
            else
            {
                //Arrow.SetActive(true);
                float minDist = 10000f;
                int index = 0;
                int i = 0;
                foreach (Collider hitcol in hitColliders)
                {


                    float dist = Vector3.Distance(hitcol.gameObject.transform.position, transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        index = i;
                    }
                    i += 1;
                    //hitcol.gameObject.GetComponent<CrowdController>().GetDisinfected();
                    //GameManager.instance.photonView.RPC("Destroy", RpcTarget.All, hitcol.gameObject.GetPhotonView().ViewID, 5);


                    // Debug.Log(hitcol.gameObject.name);

                }
                try
                {
                    Arrow.transform.LookAt(hitColliders[index].gameObject.transform);
                }
                catch
                {

                }

            }
        }

        
    }

    void FindHospital()
    {
        if (photonView.IsMine)
        {


            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 50000f, targetHospital);

            //GameManager.instance.photonView.RPC("GetDisinfected", RpcTarget.All, id);

            if (hitColliders == null)
            {
                Arrow.SetActive(false);
            }
            else
            {
                //Arrow.SetActive(true);
                float minDist = 10000f;
                int index = 0;
                int i = 0;
                foreach (Collider hitcol in hitColliders)
                {


                    float dist = Vector3.Distance(hitcol.gameObject.transform.position, transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        index = i;
                    }
                    i += 1;
                    //hitcol.gameObject.GetComponent<CrowdController>().GetDisinfected();
                    //GameManager.instance.photonView.RPC("Destroy", RpcTarget.All, hitcol.gameObject.GetPhotonView().ViewID, 5);


                    // Debug.Log(hitcol.gameObject.name);

                }
                try
                {
                    Arrow.transform.LookAt(hitColliders[index].gameObject.transform);
                }
                catch
                {

                }

            }
        }
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal") * moveSpeed;
        float z = Input.GetAxis("Vertical") * moveSpeed;

        Vector3 moveDir = new Vector3(x, 0f, z);
        moveDir.Normalize();
        if (moveDir!=Vector3.zero)
        {

            Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720f * Time.deltaTime);
            //transform.forward = moveDir;

            // Run Animation
            try
            {
                GameManager.instance.photonView.RPC("AnimatePlayer", RpcTarget.All, id, 1);

            }
            catch
            {

            }
            
            //playerAnimator.SetBool("isRunning", true);
        }
        else
        {
            if (photonView.IsMine)
            {
                try
                {
                    GameManager.instance.photonView.RPC("AnimatePlayer", RpcTarget.All, id, 0);
                }
                catch
                {

                }
                
            }
        }

        //Idle Animation

        
        rig.velocity = new Vector3(x, rig.velocity.y, z);

    }

    public void SetAnimation(int x)
    {
        try
        {


            if (x == 0)
            {
                playerAnimator.SetBool("isRunning", false);
            }
            else if (x == 1)
            {
                playerAnimator.SetBool("isRunning", true);
            }
        }
        catch
        {

        }
        
    }

    void TryJump()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if(Physics.Raycast(ray, 0.7f))
        {
            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        }
    }

    /*public void SetHat(bool hasHat)
    {
        hatObject.SetActive(hasHat);
    }*/
    public void GetCoin_player(int points)
    {
        coins += points;
        //ParticleSystem death = Instantiate(CollectCoin, transform.position, Quaternion.Euler(transform.forward)) as ParticleSystem;
        //Destroy(death, 3);
    }

    public void Add_Collectible()
    {
        totalCollected += 1;
        ParticleSystem death = Instantiate(CollectVaccine, transform.position, Quaternion.Euler(transform.up)) as ParticleSystem;
        Destroy(death, 3);
    }

    public void Add_Individual_Goal(int x)
    {
        if (x == -1)
        {
            GroceriesCollected++;
        }
        else if(x == 0)
        {
            PeopleDisinfected++; 
        }
        else if(x == 1)
        {
            VirusDestroyed++;
        }
        else if (x == 2)
        {
            flagsDestroyed++;
        }

    }

    public void Add_Infected_Total()
    {
        totalInfectedPeople += 1;
    }

    public void Subtract_Infected_Total()
    {
        if(totalInfectedPeople > 0)
        {
            totalInfectedPeople -= 1;
        }
    }

    public void GetMaskSan(int x)
    {
        if(x == 0)
        {
            hasMask = true;
            _Mask.SetActive(true);
            
        }
        else if(x == 1)
        {
            hasSanitizer = true;
            _Sanitizer.SetActive(true);
        }
    }

    public void DeleteMaskSan(int x)
    {
        if (x == 0)
        {
            Invoke("DeleteMask", 8f);
        }
        else if (x == 1)
        {
            Invoke("DeleteSanitizer", 5f);
        }
    }

    public void DeleteMask()
    {
        hasMask = false;
        _Mask.SetActive(false);
    }
    
    public void DeleteSanitizer()
    {
        hasSanitizer = false;
        _Sanitizer.SetActive(false);
    }


    public void Infect()
    {
        isInfected = true;
        jumpForce = 4f;
        moveSpeed = 6f;
        if (photonView.IsMine)
        {
            audio.PlayOneShot(InfectSound, 1f);
        }
        ParticleSystem death = Instantiate(InfectCrowd, transform.position, Quaternion.Euler(transform.up)) as ParticleSystem;
        death.transform.parent = gameObject.transform;
        Destroy(death, 3);
        virusObject.SetActive(true);
    }

    public void Disinfect()
    {
        ParticleSystem power = Instantiate(crowdDisinfect, transform.position, Quaternion.Euler(transform.forward)) as ParticleSystem;
        Destroy(power, 3);

        isInfected = false;
        jumpForce = 8f;
        moveSpeed = 10f;
        virusObject.SetActive(false);
        healthTimer = 5f;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        // hit a player
        if (collision.gameObject.CompareTag("Player"))
        {
            

            if (GameManager.instance.GetPlayer(collision.gameObject).isInfected)
            {
                if (!isInfected && curPower != 2)
                {
                    GameManager.instance.photonView.RPC("GetInfectedM", RpcTarget.All, id);
                    GameManager.instance.photonView.RPC("GetCoin", RpcTarget.All, id, -5);
                }
            }

        } 
        else if (collision.gameObject.CompareTag("Coin"))
        {
            if (photonView.IsMine)
            {
                audio.PlayOneShot(coinCollectSound, 1F);

            }//collision.gameObject.GetComponent<AudioSource>().Play();
            GameManager.instance.photonView.RPC("GetCoin", RpcTarget.All, id);
            GameManager.instance.photonView.RPC("Destroy", RpcTarget.All, collision.gameObject.GetPhotonView().ViewID, 0);

        }
        else if (collision.gameObject.CompareTag("Hospital"))
        {
            if (health < 100)
            {


                if (photonView.IsMine)
                {
                    audio.PlayOneShot(coinCollectSound, 1F);

                }//collision.gameObject.GetComponent<AudioSource>().Play();
                GameManager.instance.photonView.RPC("RestoreHealthM", RpcTarget.All, id);
                //GameManager.instance.photonView.RPC("Destroy", RpcTarget.All, collision.gameObject.GetPhotonView().ViewID, 0);
            }

        }
        else if (collision.gameObject.CompareTag("Collect"))
        {
            if (photonView.IsMine)
            {
                audio.PlayOneShot(vaccineCollectSound, 1F);
            }
            
            GameManager.instance.photonView.RPC("AddCollectible", RpcTarget.All);
            GameManager.instance.photonView.RPC("GetCoin", RpcTarget.All, id, 10);
            GameManager.instance.photonView.RPC("Destroy", RpcTarget.All, collision.gameObject.GetPhotonView().ViewID, 0);

            float targetMaxRadius = 20f;
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, targetMaxRadius);

            //Transform max_index = null;
            //float max_dist = 0;

            foreach (Collider col in colliderArray)
            {
                if (col.tag == "Virus")
                {
                    GameManager.instance.photonView.RPC("Add_Individual_Goal", RpcTarget.All, 1);
                    GameManager.instance.photonView.RPC("Destroy", RpcTarget.All, col.gameObject.GetPhotonView().ViewID, 0);
                    audio.PlayOneShot(VirusPop, 1f);
                }


                if (col.tag == "Divide")
                {
                    GameManager.instance.photonView.RPC("Destroy", RpcTarget.All, col.gameObject.GetPhotonView().ViewID, 0);
                    //SendMessageToChat();
                    GameManager.instance.photonView.RPC("SendMessageToChatSys", RpcTarget.All, id, "Stage Cleared. Proceed to the next area.");
                }

                if (col.tag == "Player")
                {
                    if (col.gameObject.GetComponent<PlayerController>().isInfected)
                    {
                        GameManager.instance.photonView.RPC("GetDisinfectedM", RpcTarget.All, col.gameObject.GetComponent<PlayerController>().id);
                        //col.gameObject.GetComponent<PlayerController>().Disinfect();
                    }
                }
            }


        }
        else if (collision.gameObject.CompareTag("Grocery"))
        {
            if (curPower == -1)
            {


                if (photonView.IsMine)
                {
                    audio.PlayOneShot(vaccineCollectSound, 1F);
                }

                GameManager.instance.photonView.RPC("Add_Individual_Goal", RpcTarget.All, -1);
                GameManager.instance.photonView.RPC("Destroy", RpcTarget.All, collision.gameObject.GetPhotonView().ViewID, 0);
            }
        }
        else if (collision.gameObject.CompareTag("Virus"))
        {
            if (collision.gameObject.GetComponent<EnemyFollow>().strain_num == 0)
            {

                if (hasSanitizer)
                {
                    GameManager.instance.photonView.RPC("GetCoin", RpcTarget.All, id, 5);
                    GameManager.instance.photonView.RPC("Add_Individual_Goal", RpcTarget.All, 1);
                    GameManager.instance.photonView.RPC("Destroy", RpcTarget.All, collision.gameObject.GetPhotonView().ViewID, 0);
                    if (photonView.IsMine)
                    {
                        audio.PlayOneShot(VirusPop, 1f);
                    }
                }
                else if (!isInfected && !hasMask && !hasSanitizer)
                {
                    GameManager.instance.photonView.RPC("GetInfectedM", RpcTarget.All, id);
                    GameManager.instance.photonView.RPC("GetCoin", RpcTarget.All, id, -5);
                    GameManager.instance.photonView.RPC("Destroy", RpcTarget.All, collision.gameObject.GetPhotonView().ViewID, 0);

                }
            }else if(collision.gameObject.GetComponent<EnemyFollow>().strain_num == 1)
            {
                if (!isInfected)
                {
                    GameManager.instance.photonView.RPC("GetInfectedM", RpcTarget.All, id);
                    GameManager.instance.photonView.RPC("GetCoin", RpcTarget.All, id, -5);
                    GameManager.instance.photonView.RPC("Destroy", RpcTarget.All, collision.gameObject.GetPhotonView().ViewID, 0);
                }
                
            }

        }
        else if (collision.gameObject.CompareTag("CrowdTarget"))
        {
             

            if (curPower == 2)
            {


                GameManager.instance.photonView.RPC("Add_Individual_Goal", RpcTarget.All, 2);
                // secret code 99
                GameManager.instance.photonView.RPC("Destroy", RpcTarget.All, collision.gameObject.GetPhotonView().ViewID, id, 0);
                GameManager.instance.photonView.RPC("GetCoin", RpcTarget.All, id, 20);

                //float targetMaxRadius = 40f;
                //Collider[] colliderArray = Physics.OverlapSphere(transform.position, targetMaxRadius);

                //Transform max_index = null;
                //float max_dist = 0;

                /*foreach (Collider col in colliderArray)
                {
                    if (col.tag == "Crowd")
                    {
                        GameManager.instance.photonView.RPC("Destroy", RpcTarget.All, col.gameObject.GetPhotonView().ViewID, 0);
                    }

                }*/

            }


        }
        else if (collision.gameObject.CompareTag("Crowd"))
        {


            if (curPower != 2)
            {

                if (!isInfected && collision.gameObject.GetComponent<CrowdController>().isInfected)
                {
                    GameManager.instance.photonView.RPC("GetInfectedM", RpcTarget.All, id);
                    GameManager.instance.photonView.RPC("GetCoin", RpcTarget.All, id, -5);

                }

                // secret code 99


            }


        }
        else if (collision.gameObject.CompareTag("Mask"))
        {
            if (!hasMask)
            {

                if (photonView.IsMine)
                {
                    audio.PlayOneShot(vaccineCollectSound, 1F);
                }
                GameManager.instance.photonView.RPC("Destroy", RpcTarget.All, collision.gameObject.GetPhotonView().ViewID, 0);
                GameManager.instance.photonView.RPC("GetMaskSanM", RpcTarget.All, id, 0);
                GameManager.instance.photonView.RPC("DeleteMaskSanM", RpcTarget.All, id, 0);
                GameManager.instance.photonView.RPC("GetCoin", RpcTarget.All, id, 15);

            }


        }
        else if (collision.gameObject.CompareTag("Sanitizer"))
        {
            if (!hasSanitizer)
            {
                if (photonView.IsMine)
                {
                    audio.PlayOneShot(vaccineCollectSound, 1F);
                }
                GameManager.instance.photonView.RPC("Destroy", RpcTarget.All, collision.gameObject.GetPhotonView().ViewID, 0);
                GameManager.instance.photonView.RPC("GetMaskSanM", RpcTarget.All, id, 1);
                GameManager.instance.photonView.RPC("DeleteMaskSanM", RpcTarget.All, id, 1);
                GameManager.instance.photonView.RPC("GetCoin", RpcTarget.All, id, 15);

            }


        }
    }

    public void DestroyFlagEffect()
    {
        if (photonView.IsMine)
        {
            audio.PlayOneShot(DestroyFlag, 1f);
        }
        
        ParticleSystem death = Instantiate(flagDestroy, transform.position, Quaternion.Euler(transform.up)) as ParticleSystem;
        Destroy(death, 3);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
            stream.SendNext(ammo0);
            stream.SendNext(ammo1);
            stream.SendNext(canShoot);

            stream.SendNext(totalInfectedPeople);
            stream.SendNext(totalCollected);



            stream.SendNext(flagsDestroyed);
            stream.SendNext(GroceriesCollected);
            stream.SendNext(VirusDestroyed);
            stream.SendNext(PeopleDisinfected);

            stream.SendNext(coins);
            stream.SendNext(isInfected);

            
        }
        else if (stream.IsReading)
        {
            health = (int)stream.ReceiveNext();
            ammo0 = (int)stream.ReceiveNext();
            ammo1 = (int)stream.ReceiveNext();
            canShoot = (bool)stream.ReceiveNext();

            totalInfectedPeople = (int)stream.ReceiveNext();
            totalCollected = (int)stream.ReceiveNext();

            flagsDestroyed = (int)stream.ReceiveNext();
            GroceriesCollected = (int)stream.ReceiveNext();
            VirusDestroyed = (int)stream.ReceiveNext();
            PeopleDisinfected = (int)stream.ReceiveNext();

            coins = (int)stream.ReceiveNext();
            isInfected = (bool)stream.ReceiveNext();

        }
    }

    public void Power(int curPower)
    {
        

        if (curPower == 0 && ammo0 > 0 && canShoot)
        {
            
            if (photonView.IsMine)
            {
                audio.PlayOneShot(PowerSound, 1f);
                ammo0 -= 1;
                canShoot = false;
                StartCoroutine("ResetShoot");


            }

                ParticleSystem power = Instantiate(power0_prefab, transform.position, transform.rotation) as ParticleSystem;
                Destroy(power, 2f);

                Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f, crowdL);

                //GameManager.instance.photonView.RPC("GetDisinfected", RpcTarget.All, id);

                foreach (Collider hitcol in hitColliders)
                {
                    if (hitcol.GetComponent<Rigidbody>() != null)
                    {

                        if (hitcol.gameObject.GetComponent<CrowdController>().isInfected)
                        {
                        if (photonView.IsMine)
                        {
                            audio.PlayOneShot(VirusPop, 1f);
                        }
                            
                            hitcol.gameObject.GetComponent<CrowdController>().GetDisinfected();

                            if (photonView.IsMine)
                            {
                                GameManager.instance.photonView.RPC("Add_Individual_Goal", RpcTarget.All, 0);
                            }
                            
                            GameManager.instance.photonView.RPC("Destroy", RpcTarget.All, hitcol.gameObject.GetPhotonView().ViewID, 5);
                            GameManager.instance.photonView.RPC("GetCoin", RpcTarget.All, id, 5);
                        }

                        // Debug.Log(hitcol.gameObject.name);
                    }
                }


            //power.transform.parent = transform;
            //GameObject power = Instantiate(power0_prefab, PowerTransform.position, Quaternion.identity) as GameObject;
            //power.GetComponent<Rigidbody>().AddForce(transform.forward * 20f, ForceMode.Impulse);
            Destroy(power, 3);


            }

            if (curPower == 1 && ammo1 > 0 && canShoot)
            {

                
                if (photonView.IsMine){
                        audio.PlayOneShot(PowerSound, 1f);
                        ammo1 -= 1;
                        canShoot = false;
                        StartCoroutine("ResetShoot");

                    }
            
                ParticleSystem power = Instantiate(power1_prefab, transform.position, transform.rotation) as ParticleSystem;
                Destroy(power, 2f);
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, 4f, virusL);

                foreach (Collider hitcol in hitColliders)
                {
                    if (hitcol.GetComponent<Rigidbody>() != null)
                    {
                        if (photonView.IsMine)
                        {
                            GameManager.instance.photonView.RPC("Add_Individual_Goal", RpcTarget.All, 1);
                            audio.PlayOneShot(VirusPop, 1f);
                        }
                        
                        GameManager.instance.photonView.RPC("Destroy", RpcTarget.All, hitcol.gameObject.GetPhotonView().ViewID, 0);
                        GameManager.instance.photonView.RPC("GetCoin", RpcTarget.All, id, 8);

                        ParticleSystem death = Instantiate(EnemyDeath, transform.position, Quaternion.Euler(transform.up)) as ParticleSystem;

                        Destroy(power, 3);

                        // Debug.Log(hitcol.gameObject.name);
                    }
                }

                //GameObject power = Instantiate(power0_prefab, PowerTransform.position, Quaternion.identity) as GameObject;
                //power.GetComponent<Rigidbody>().AddForce(transform.forward * 20f, ForceMode.Impulse);
                //power.transform.parent = transform;
                Destroy(power, 3);

            }

            if (curPower == 2)
            {




            //ParticleSystem power = Instantiate(power2_prefab, transform.position, transform.rotation) as ParticleSystem;
            DestroyOnReset = Instantiate(power2_prefab, transform.position, transform.rotation) as ParticleSystem;
            //GameObject power = Instantiate(power0_prefab, PowerTransform.position, Quaternion.identity) as GameObject;
            //power.GetComponent<Rigidbody>().AddForce(transform.forward * 20f, ForceMode.Impulse);
            DestroyOnReset.transform.parent = transform;
                moveSpeed = 8f;

                transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                Destroy(DestroyOnReset, 10f);

                //Invoke("WrapperReset", 10f);



            }
        

    }

    public IEnumerator ResetShoot()
    {
        if (photonView.IsMine)
        {


            yield return new WaitForSeconds(ShootTimer);
            canShoot = true;

        }
    }
    public void WrapperReset()
    {
        GameManager.instance.photonView.RPC("ResetMovementM", RpcTarget.All, id);
    }

    public void Speed()
    {
        GameManager.instance.photonView.RPC("Power", RpcTarget.All, id, 2);
    }


    public void ResetMovement()
    {
        try
        {
            costume_Enforcer.SetActive(false);
            Destroy(DestroyOnReset);
        }
        catch
        {

        }
            
            moveSpeed = 10f;
            //moveSpeed /= .8f;
            transform.localScale = new Vector3(1f, 1f, 1f);
            curPower = -1;
        
    }


    // remove if conditions, add citizen power (-1)
    [PunRPC]
    public void powerChange(int x)
    {

        if(x == -1)
        {
            costume_Sanitizer.SetActive(false);
            costume_Doctor.SetActive(false);
            costume_Enforcer.SetActive(false);


            ResetMovement();
            curPower = -1;
            ParticleSystem death = Instantiate(powerGain, transform.position, Quaternion.Euler(transform.up)) as ParticleSystem;
            Destroy(death, 3);
            if (photonView.IsMine)
            {
                try
                {
                    audio.PlayOneShot(powerCollectSound, 1f);
                }
                catch
                {

                }
            }
        } else if (x == 0)
        {
            
                costume_Sanitizer.SetActive(false);
                costume_Doctor.SetActive(true);
                costume_Enforcer.SetActive(false);
            ResetMovement();
            curPower = 0;
                ParticleSystem death = Instantiate(powerGain, transform.position, Quaternion.Euler(transform.up)) as ParticleSystem;
                Destroy(death, 3);
                if (photonView.IsMine)
                {
                    try
                    {
                        audio.PlayOneShot(powerCollectSound, 1f);
                    }
                    catch
                    {

                    }
                }
            
        }
        else if (x == 1)
        {
            ResetMovement();
            curPower = 1;
                costume_Sanitizer.SetActive(true);
                costume_Doctor.SetActive(false);
                costume_Enforcer.SetActive(false);
                if (photonView.IsMine)
                {
                    try
                    {
                        audio.PlayOneShot(powerCollectSound, 1f);
                    }
                    catch
                    {

                    }
                    
                }
                ParticleSystem death = Instantiate(powerGain, transform.position, Quaternion.Euler(transform.up)) as ParticleSystem;
                Destroy(death, 3);
            
        }
        else if (x == 2)
        {
            
                costume_Sanitizer.SetActive(false);
                costume_Doctor.SetActive(false);
                costume_Enforcer.SetActive(true);
                if (photonView.IsMine)
                {
                    try
                    {
                        audio.PlayOneShot(powerCollectSound, 1f);
                    }
                    catch
                    {

                    }
                }
                curPower = 2;
                Speed();
                ParticleSystem death = Instantiate(powerGain, transform.position, Quaternion.Euler(transform.up)) as ParticleSystem;
                Destroy(death, 3);
            

        }
    }

    public void RestoreAmmo(int x)
    {

        if(x == 0)
        {
            ammo0 = 10;
            ParticleSystem death = Instantiate(powerGain, transform.position, Quaternion.Euler(transform.up)) as ParticleSystem;
            Destroy(death, 3);
            if (photonView.IsMine)
            {
                try
                {
                    audio.PlayOneShot(powerCollectSound, 1f);
                }
                catch
                {

                }
            }
        }
        else if(x == 1)
        {
            ammo1 = 10;
            ParticleSystem death = Instantiate(powerGain, transform.position, Quaternion.Euler(transform.up)) as ParticleSystem;
            Destroy(death, 3);
            if (photonView.IsMine)
            {
                try
                {
                    audio.PlayOneShot(powerCollectSound, 1f);
                }
                catch
                {

                }
            }
        }
    } 

    public void SendMessageToChat(string text)
    {
        if (messageList.Count >= maxMessages)
        {
            Destroy(messageList[0].textObject);
            messageList.Remove(messageList[0]);
        }

        Message newMessage = new Message();
        newMessage.text = text;


        GameObject newText = Instantiate(textObject, ChatPanel.transform);
        newMessage.textObject = newText;

        newMessage.textObject.GetComponent<TextMeshProUGUI>().text = newMessage.text;

        messageList.Add(newMessage);
    }

}


[System.Serializable]
public class Message
{
    public string text;
    public GameObject textObject;
}