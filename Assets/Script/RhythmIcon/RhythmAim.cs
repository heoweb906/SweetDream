using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmAim : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RhythmIcon"))
        {
            Debug.Log("RhythmIcon과 충돌함");
        }
    }
}
