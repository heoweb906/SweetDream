using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Boss_ThornAlrm : MonoBehaviour
{
    public float timeToDestroy;

    private void Start()
    {
        Invoke("DeleteThorn", timeToDestroy);
    }

    private void DeleteThorn()
    {
        Destroy(gameObject);
    }
}
