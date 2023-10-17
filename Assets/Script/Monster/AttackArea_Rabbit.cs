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

            // �÷��̾� ��ũ��Ʈ�� �����ϸ� �÷��̾��� ü���� ���ҽ�Ŵ
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