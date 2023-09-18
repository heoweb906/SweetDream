using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Long : Monster
{
    [Header("관련 오브젝트 / 변수")]
    public bool isAttack;
    public GameObject bulletPrefab; // 총알 프리팹을 연결해야 합니다.

    private Animator anim;
    private Transform player;
    private Rigidbody rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (currentHealth <= 0 && !doDie)
        {
            anim.SetBool("boolDie",true);
            doDie = true;
            anim.SetTrigger("doDie");
        }

        if (!isAttack && !doDie && gameManager.bpmCount % 4 == 0 && gameManager.bpmCount != 0)  // 4번째 bpm 마다 한번씩 공격
        {
            isAttack = true;
              StartCoroutine(AttackAfterDelay(0.35f));  // 0.35초 뒤에 공격 코루틴 시작 - 애니메이션 속도를 생각해서 공격과 BPM을 일치시키기 위해서
        }

        LookAtPlayer(); // 플레이어 방향으로 회전시키는 함수
    }

    private IEnumerator AttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 0.4초 대기
        yield return StartCoroutine(Attack()); // Attack 코루틴 시작
    }
    IEnumerator Attack()
    {
        anim.SetTrigger("doAttack");
        yield return new WaitForSeconds(1.5f);
        isAttack = false;
    }


    public void LookAtPlayer()
    {
        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0f; // y축 회전을 고려하지 않음
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation;
        }
    }


    public void ShootAtPlayer() // 애니메이션 이벤트로 연결해놨음
    {
        if (bulletPrefab != null)
        {
            // 플레이어 위치를 향해 회전
            Vector3 playerPosition = player.position;
            Vector3 offset = new Vector3(0f, -1f, 0f); // 아래로 1 단위만큼 내리는 오프셋
            Vector3 direction = (playerPosition + offset) - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);

            // 총알을 생성하고 발사
            GameObject bullet = Instantiate(bulletPrefab, transform.position, rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

            // 총알에 힘을 가해 발사 (원하는 힘과 방향으로 수정해야 함)
            float bulletSpeed = 10f; // 총알 발사 속도
            bulletRb.velocity = direction.normalized * bulletSpeed;

            // 총알을 발사한 후 몇 초 후에 자동으로 삭제 (원하는 시간으로 수정 가능)
            float bulletDestroyDelay = 5f; // 몇 초 후에 삭제할지 설정
            Destroy(bullet, bulletDestroyDelay);
        }
    }
}