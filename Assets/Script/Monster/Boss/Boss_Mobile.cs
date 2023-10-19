using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Mobile : MonoBehaviour
{
    public int damage;
    public float initialVelocity; // 초기 속도
    private Rigidbody rb;

    // #. 몬스터 생성 관련 변수
    public Transform[] spawnPosition;
    public GameObject[] randomMonster;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.down * initialVelocity; // 아래 방향으로 초기 속도 설정
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
        }

        if (other.CompareTag("Floor"))
        {
            SpawnRandomMonsters();
            Destroy(gameObject);
        }
    }

    // 랜덤 몬스터 생성 함수
    private void SpawnRandomMonsters()
    {
        for (int i = 0; i < spawnPosition.Length; i++)
        {
            // 랜덤한 몬스터 프리팹 선택
            int randomMonsterIndex = Random.Range(0, randomMonster.Length);
            GameObject selectedMonster = randomMonster[randomMonsterIndex];

            // 몬스터를 생성 위치에 생성
            Instantiate(selectedMonster, spawnPosition[i].position, Quaternion.identity);
        }
    }
}
