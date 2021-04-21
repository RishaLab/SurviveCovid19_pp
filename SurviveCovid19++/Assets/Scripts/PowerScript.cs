using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PowerScript : MonoBehaviour
{
    [Header("Power Type Selection")]
    public int PowerType;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Virus"))
        {
            GameManager.instance.photonView.RPC("Destroy", RpcTarget.All, collision.gameObject.GetPhotonView().ViewID);
            try
            {
                GameManager.instance.photonView.RPC("Destroy", RpcTarget.All, gameObject.GetPhotonView().ViewID);
            }
            catch
            {

            }
            
        }
    }
}
