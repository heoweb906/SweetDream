using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Boss_Bullet_Color : Boss_Bullet
{
    public int numColor;
    public int damageMonster;

    public void SetNewDirection(Vector3 direction)
    {
        Rigidbody bulletRigidbody = gameObject.GetComponent<Rigidbody>();

        if (bulletRigidbody != null)
        {
            bulletRigidbody.velocity = Vector3.zero;
            Debug.Log("실행되었습니다");
            bulletRigidbody.velocity = direction * 80f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss"))
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

}
