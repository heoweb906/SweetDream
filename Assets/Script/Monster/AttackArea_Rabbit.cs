using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea_Rabbit : MonoBehaviour
{
    public Monster monster;
    public int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject playerObject = other.gameObject;
            Player playerScript = playerObject.GetComponent<Player>();

            // 플레이어 스크립트가 존재하면 플레이어의 체력을 감소시킴
            if (playerScript != null)
            {
                if(!monster.doDie)
                {
                    playerScript.OnDamage(damage);
                }
               
            }
        }
    }
}