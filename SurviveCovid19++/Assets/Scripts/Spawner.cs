using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class Spawner : MonoBehaviourPunCallbacks
{
    public string enemy_prefab;
    public int spawn_Num;
    public int max_spawn;
    public float radius = 10f;

    public float counter;


    float randX;
    float randZ;

    Vector3 spawnPoint;

    private void Start()
    {
        if (enemy_prefab == "Crowd")
        {
            GameManager.instance.total_People += max_spawn;
        }

        if (enemy_prefab == "Enemy")
        {
            GameManager.instance.total_Viruses += max_spawn;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (max_spawn > 0)
        {
            
            float targetMaxRadius = 30f;
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, targetMaxRadius);

            //Transform max_index = null;
            //float max_dist = 0;

            foreach (Collider col in colliderArray)
            {
                if (col.tag == "Player")
                {
                    Spawn();
                }
            }

            
            
        }
        

    }


    public void Spawn()
    {
        max_spawn -= 3;

        for (int i = 0; i < 3; ++i)
        {
            
            randX = Random.Range(-radius, radius);
            randZ = Random.Range(-radius, radius);
            spawnPoint = new Vector3(transform.position.x + randX, 1f, transform.position.z + randZ);
            PhotonNetwork.Instantiate(enemy_prefab, spawnPoint, Quaternion.identity);



        }
        
    }
        
    
}
