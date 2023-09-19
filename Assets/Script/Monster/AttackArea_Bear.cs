using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea_Bear : MonoBehaviour
{
    public int damage = 1;
    public float pushBackForce = 10f; 
    public float upwardForce = 5f;   

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject playerObject = other.gameObject;
            Player playerScript = playerObject.GetComponent<Player>();
            Rigidbody playerRigidbody = playerObject.GetComponent<Rigidbody>();

            if (playerScript != null)
            {
                playerScript.OnDamage(damage);

                if (playerRigidbody != null)
                {
                    Vector3 pushDirection = (playerObject.transform.position - transform.position).normalized;
                    playerRigidbody.AddForce(pushDirection * pushBackForce, ForceMode.Impulse);
                    playerRigidbody.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);
                }
            }
        }
    }
}