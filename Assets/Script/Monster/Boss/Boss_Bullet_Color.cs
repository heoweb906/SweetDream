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
            Debug.Log("����Ǿ����ϴ�");
            bulletRigidbody.velocity = direction * 80f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss"))
        {
            GameObject monsterObject = other.gameObject;
            Boss_Swan monster = monsterObject.GetComponent<Boss_Swan>();

            // �÷��̾� ��ũ��Ʈ�� �����ϸ� �÷��̾��� ü���� ���ҽ�Ŵ
            if (monster != null)
            {
                monster.TakeDamage(damageMonster);
            }
            Destroy(gameObject);
        }
    }

}
