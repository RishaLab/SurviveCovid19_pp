using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CrowdController : MonoBehaviourPunCallbacks, IPunObservable
{
    public Transform targetTransform;
    public Rigidbody rig;

    private float lookForTargetTimer;
    private float lookForTargetTimerMax = 5f;

    public ParticleSystem crowdDisinfect;
    public ParticleSystem crowdInfect;




    public float moveSpeed;

    public bool isInfected = false;
    public bool immune = false;

    public GameObject infectedHat;

    public bool found = false;

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange = 10f;

    public LayerMask playerMask;

    public Transform PlayerPower;

    public float patrolLimit = 5f;
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
        lookForTargetTimer = UnityEngine.Random.Range(0f, lookForTargetTimerMax);
    }

    // Update is called once per frame
    void Update()
    {
        if (isInfected)
        {
            infectedHat.SetActive(true);
            
        }
        else
        {
            infectedHat.SetActive(false);

            
        }
        HandleMovement();
        HandleTargetting();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!immune)
        {


            if (collision.gameObject.CompareTag("Crowd"))
            {
                if (!isInfected && collision.gameObject.GetComponent<CrowdController>().isInfected)
                {
                    photonView.RPC("GetInfected", RpcTarget.All);

                }

            }
            if (collision.gameObject.CompareTag("Player"))
            {
                if (!isInfected && collision.gameObject.GetComponent<PlayerController>().isInfected)
                {
                    photonView.RPC("GetInfected", RpcTarget.All);

                }

            }
            if (collision.gameObject.CompareTag("Virus"))
            {
                if (!isInfected)
                {
                    photonView.RPC("GetInfected", RpcTarget.All);
                    
                    GameManager.instance.photonView.RPC("Destroy", RpcTarget.All, collision.gameObject.GetPhotonView().ViewID, 0);
                }

            }
        }
    }


    [PunRPC]
    public void GetInfected()
    {
        isInfected = true;
        ParticleSystem power = Instantiate(crowdInfect, transform.position, Quaternion.Euler(transform.forward)) as ParticleSystem;
        GameManager.instance.photonView.RPC("AddInfected", RpcTarget.All);
        //Destroy(power, 3);
    }

    public void GetDisinfected()
    {
        ParticleSystem power = Instantiate(crowdDisinfect, transform.position, Quaternion.Euler(transform.forward)) as ParticleSystem;
        GameManager.instance.photonView.RPC("SubtractInfected", RpcTarget.All);

        Destroy(power, 3);
        immune = true;
        isInfected = false;
    }


    private void HandleTargetting()
    {
        


            PlayerPower = null;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10f, playerMask);

            //GameManager.instance.photonView.RPC("GetDisinfected", RpcTarget.All, id);

            foreach (Collider hitcol in hitColliders)
            {
                if (hitcol.GetComponent<Rigidbody>() != null)
                {

                    if (hitcol.gameObject.GetComponent<PlayerController>().curPower == 2)
                    {

                        PlayerPower = hitcol.gameObject.transform;


                    }

                    // Debug.Log(hitcol.gameObject.name);
                }
            }
        

        if(PlayerPower == null)
        {
            lookForTargetTimer -= Time.deltaTime;
            if (lookForTargetTimer < 0f)
            {
                lookForTargetTimer += lookForTargetTimerMax;
                if (!found)
                {
                    LookForTargets();
                }
            }
        }

        



    }

    private void HandleMovement()
    {
        if (PlayerPower != null)
        {
            Vector3 dirToPlayer = (transform.position - PlayerPower.transform.position);
            dirToPlayer.y = 0f;
            dirToPlayer = dirToPlayer.normalized;
            

            //Vector3 newPos = transform.position + dirToPlayer;
            
            
            float moveSpeed = 4f;
            rig.velocity = dirToPlayer * moveSpeed;


            transform.LookAt(PlayerPower.transform);
        }
        else
        {

            if (targetTransform != null)
            {
                if (Vector3.Distance(targetTransform.position, transform.position) > 10f)
                {

                    Vector3 moveDir = (targetTransform.position - transform.position).normalized;
                    float moveSpeed = 6f;
                    rig.velocity = moveDir * moveSpeed;

                }
                transform.LookAt(targetTransform.position + new Vector3(0f, 1 - targetTransform.position.y, 0f));

            }
            else
            {
                found = false;
                targetTransform = null;
                Patroling();
            }
        }
        
    }

    private void LookForTargets()
    {
        float targetMaxRadius = 50f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, targetMaxRadius);

        Transform max_index = null;
        float max_dist = 0;

        foreach (Collider col in colliderArray)
        {
            if (col.tag == "CrowdTarget")
            {


                Transform _temp = col.gameObject.transform;
                float dist = Vector3.Distance(transform.position, _temp.position);
                if (dist > max_dist)
                {
                    max_dist = dist;
                    max_index = col.gameObject.transform;
                }
            }
        }

        targetTransform = max_index;
        if (targetTransform != null)
        {
            found = true;
        }
        else
        {
            found = false;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isInfected);
            


        }
        else if (stream.IsReading)
        {
            isInfected = (bool)stream.ReceiveNext();
            

        }
    }

    private void Patroling()
    {

        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            transform.LookAt(walkPoint);
            try
            {
                if (patrolLimit < 0f)
                {
                    patrolLimit = 5f;
                    walkPointSet = false;

                }
                else
                {


                    Vector3 moveDir = (walkPoint - transform.position).normalized;
                    patrolLimit -= Time.deltaTime;
                    float moveSpeed = 3f;
                    rig.velocity = moveDir * moveSpeed;
                    transform.LookAt(walkPoint);
                }
            }
            catch
            {

            }
        }


        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;

    }

    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, 1.5f, transform.position.z + randomZ);


        walkPointSet = true;
    }
}
