using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Bullet : MonoBehaviour
{
    public int damage = 1;
    public bool b_colloerBullet;
    public int damageMonster;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject playerObject = other.gameObject;
            Player playerScript = playerObject.GetComponent<Player>();

            // 플레이어 스크립트가 존재하면 플레이어의 체력을 감소시킴
            if (playerScript != null)
            {
                playerScript.OnDamage(damage);
            }

            Destroy(gameObject);
        }

        if (other.CompareTag("Boss"))
        {
            if(b_colloerBullet)
            {
                GameObject monsterObject = other.gameObject;
                Boss_Swan monster = monsterObject.GetComponent<Boss_Swan>();

                // 플레이어 스크립트가 존재하면 플레이어의 체력을 감소시킴
                if (monster != null)
                {
                    monster.TakeDamage(damageMonster);
                }
                Destroy(gameObject);
            }
        }

        if (other.CompareTag("Floor") || other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

}