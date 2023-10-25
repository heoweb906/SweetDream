using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Bullet : MonoBehaviour
{
    public int damage = 1;
    public bool b_colloerBullet;
    public int damageMonster;

    public bool b_PlayerHit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject playerObject = other.gameObject;
            Player playerScript = playerObject.GetComponent<Player>();

            // �÷��̾� ��ũ��Ʈ�� �����ϸ� �÷��̾��� ü���� ���ҽ�Ŵ
            if (playerScript != null)
            {
                playerScript.OnDamage(damage);
            }

            Destroy(gameObject);
        }

        if (other.CompareTag("Boss"))
        {
            if (b_colloerBullet && b_PlayerHit)
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

        if (other.CompareTag("Floor") || other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

}