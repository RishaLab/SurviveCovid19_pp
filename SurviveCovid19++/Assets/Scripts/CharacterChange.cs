using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CharacterChange : MonoBehaviourPunCallbacks
{

    

    //public int curPower = -1;
    private PlayerController player;

    

    private void Start()
    {
        player = GetComponent<PlayerController>();
        //curPower = -1;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Change0"))
        {
            // GameManager.instance.photonView.RPC("powerChangeM", RpcTarget.All, player.id, 0);
            GameManager.instance.photonView.RPC("RestoreAmmoM", RpcTarget.All, player.id, 0);
            GameManager.instance.photonView.RPC("Destroy", RpcTarget.All, collision.gameObject.GetPhotonView().ViewID, 0);
            //powerChange(0);

        }

        if (collision.gameObject.CompareTag("Change1"))
        {

            // GameManager.instance.photonView.RPC("powerChangeM", RpcTarget.All, player.id, 1);
            GameManager.instance.photonView.RPC("RestoreAmmoM", RpcTarget.All, player.id, 1);
            GameManager.instance.photonView.RPC("Destroy", RpcTarget.All, collision.gameObject.GetPhotonView().ViewID, 0);
            //powerChange(1);

        }

        /*if (collision.gameObject.CompareTag("Change2"))
        {

            GameManager.instance.photonView.RPC("powerChangeM", RpcTarget.All, player.id, 2);
            //powerChange(2);

        }*/

        
    }



}
