using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{

    public Transform targetTransform;
    public Rigidbody rig;

    private float lookForTargetTimer;
    private float lookForTargetTimerMax = 0.2f;
    public float moveSpeed = 6f;
    public bool isBoss = false;
    public float howClose;

    public bool found = false;

    public float patrolLimit = 5f;

    public int strain_num = 0;



    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange = 100f;


    private void Start()
    {
        if (isBoss)
        {
            moveSpeed = 4f;
        }
        else
        {
            moveSpeed = 6f;
        }
        rig = GetComponent<Rigidbody>();
        lookForTargetTimer = UnityEngine.Random.Range(0f, lookForTargetTimerMax);
    }

    private void Update()
    {
        HandleMovement();
        HandleTargetting();
    }

    private void HandleMovement()
    {
        if (targetTransform != null)
        {
            if (Vector3.Distance(targetTransform.position, transform.position) > 100f)
            {
                found = false;
                targetTransform = null;
                rig.velocity = Vector3.zero;
            }
            else
            {
                Vector3 moveDir = (targetTransform.position - transform.position).normalized;


                rig.velocity = moveDir * moveSpeed;
                transform.LookAt(targetTransform.position);
            }

        }
        else
        {
            Patroling();
            //rig.velocity = Vector3.zero;
        }
    }

    private void HandleTargetting()
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

    private void LookForTargets()
    {
        float targetMaxRadius = 20f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, targetMaxRadius);

        Transform max_index = null;
        float max_dist = 0;

        foreach (Collider col in colliderArray)
        {
            if (col.tag == "Player")
            {
                if (strain_num == 0)
                {
                    if (!col.gameObject.GetComponent<PlayerController>().isInfected && !col.gameObject.GetComponent<PlayerController>().hasMask)
                    {
                        Transform _temp = col.gameObject.transform;
                        float dist = Vector3.Distance(transform.position, _temp.position);
                        if (dist > max_dist)
                        {
                            max_dist = dist;
                            max_index = col.gameObject.transform;
                        }
                    }
                }else if(strain_num == 1)
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
        }

        targetTransform = max_index;
        if(targetTransform != null)
        {
            found = true;
        }
        else
        {
            found = false;
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
        //float randomY = UnityEngine.Random.Range(-walkPointRange/10f, walkPointRange/10f);

        walkPoint = new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ);


        walkPointSet = true;
    }

}
