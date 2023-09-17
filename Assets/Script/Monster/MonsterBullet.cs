using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBullet : MonoBehaviour
{
    public int damage = 1;
    public float destroyDelay = 15f;

    private void Start()
    {
        // 일정 시간이 지난 후에 자동으로 삭제
        Destroy(gameObject, destroyDelay);
    }

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
    }
}