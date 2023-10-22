using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boos_DiePlateAlrm : MonoBehaviour
{
    private void OnTriggerStay(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            Player player = collider.GetComponent<Player>();
            player.isSafeZone = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            Player player = collider.GetComponent<Player>();
            player.isSafeZone = false;
        }
    }
}
