using System;
using UnityEngine;

public class ClaimItemManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) 
    {
        if(other.transform.tag == "coins"){
            Destroy(other.gameObject);
            UIManager.Instance.SetAmountCoins(1);
        }
    }
}