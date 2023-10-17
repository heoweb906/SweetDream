using DG.Tweening;
using DG.Tweening.Core.Easing;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Boss_Swan : MonoBehaviour
{
    private GameManager gameManager;
    private StageManager stagemanager;
    public Player player;

    [Header("테스트용 변수")]
    public GameObject[] patternSphere;

    [Header("몬스터 정보")]
    public int currentHealth; // 체력
    public int monsterColor; // 컬러 넘버 / 파랑 - 1 / 오렌지 - 1 / 보라 - 1
    public int damage = 1; // 부딥혔을 때 대미지
    public bool doDie;  // 죽었는지 살았는지

    [Header("색상 관련 변수들")]
    public new Renderer renderer; // 렌더러 컴포넌트
    public Color originalColor; // 원래 색상
    public Color newColor; // 변경하려는 색상
    private Material monsterMaterial; // 최초 실행 시 머테리얼
    public Material[] newMaterial; // 적용할 새로운 머티리얼

    // #.깜박임 관련
    private Tweener colorTween;
    private bool isFlashing;

    [Header("보스 관련 프리펩 / 정보들")]
    public Transform[] position_nest;
    public Transform position_Mouse;

    // #. 기본 공격
    public GameObject bullet_Simple;
    public float bulletSpeed = 10.0f;

    // #. 색 탄환 공격
    public GameObject[] bullet_Color;
    public float bulletSpeed_Color = 10.0f;


    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>(); // GameManager를 찾아서 할당
        stagemanager = FindObjectOfType<StageManager>();

        if (renderer != null)
        {
            monsterMaterial = renderer.material;
            originalColor = monsterMaterial.GetColor("_BaseColor"); // 원래 색상 저장
        }
        else
        {
            Debug.LogError("Renderer 컴포넌트를 찾을 수 없습니다.");
        }
    }



    public void Update()
    {
        // #. 패턴 테스트용
        if (Input.GetKeyDown(KeyCode.P))
        {
            ShootBullet();
        }
    }



    // #. 데미지 받음 함수
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (renderer != null)
        {
            if (colorTween != null)
            {
                colorTween.Kill(); 
            }
            colorTween = DOTween.To(() => renderer.material.GetColor("_BaseColor"), 
                x => renderer.material.SetColor("_BaseColor", x), newColor, 0.1f)
                .OnComplete(() => ColorBack());

            isFlashing = true;
            StartCoroutine(FlashAfterColorChange());

            if (currentHealth <= 0)
            {
                Invoke("Die", 3.0f);
            } 
        }
    }
    private IEnumerator FlashAfterColorChange()
    {
        yield return new WaitForSeconds(0.1f); // 약간의 딜레이 (색상 변경 후 깜박임 시작)

        // 색상을 깜박이게 하려면 여기에서 원래 색상과 다른 색상으로 교체합니다.
        if (isFlashing)
        {
            renderer.material.color = Color.black;
        }
    }
    private void ColorBack()
    {
        if (monsterMaterial != null)
        {
            monsterMaterial.SetColor("_BaseColor", originalColor);
        }
        isFlashing = false; // 깜박임 중지
    }

    // #. 보스 색상 변동 함수
    private void ChangeMaterial(int index)
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null && newMaterial != null)
        {
            monsterColor = index + 1;
            renderer.material = newMaterial[index];
            monsterMaterial = renderer.material;
            originalColor = monsterMaterial.GetColor("_BaseColor"); // 원래 색상 저장
        }
    }

    // #. 죽음 함수
    private void Die()
    {
        stagemanager.MonsterCount--;

        Destroy(gameObject);
    }






    // #. 기본 공격
    public void ShootBullet()
    {
        if (player != null && bullet_Simple != null && position_Mouse != null)
        {
            Vector3 bossMouthPosition = position_Mouse.position;
            Vector3 playerPosition = player.transform.position;

            // 방향 벡터 계산
            Vector3 direction = (playerPosition - bossMouthPosition).normalized;

            // 각도 간격 설정
            float angleInterval = 20f;

            // 생성할 총알 개수
            for (int i = -2; i <= 2; i++)
            {
                // 현재 각도 계산
                float currentAngle = i * angleInterval;

                GameObject newBullet = Instantiate(bullet_Simple, bossMouthPosition, Quaternion.identity);
                Rigidbody bulletRigidbody = newBullet.GetComponent<Rigidbody>();
                if (bulletRigidbody != null)
                {
                    Vector3 bulletVelocity = Quaternion.Euler(0, currentAngle, 0) * direction;
                    bulletRigidbody.velocity = bulletVelocity * bulletSpeed;
                }
            }
        }
    }

    // #. 컬러 공격
    public void ShootColorBullet()
    {
        if (player != null && bullet_Color != null && position_Mouse != null)
        {
            // 보스 입 위치 (position_Mouse)
            Vector3 bossMouthPosition = position_Mouse.position;

            // 총알을 보스의 입 위치에 랜덤한 색상의 총알을 생성
            int randomNum = Random.Range(0, 3);
            GameObject newBullet = Instantiate(bullet_Color[randomNum], bossMouthPosition, Quaternion.identity);

            // 총알을 플레이어 방향으로 발사
            if (player.transform != null)
            {
                Vector3 direction = (player.transform.position - bossMouthPosition).normalized;
                Rigidbody bulletRigidbody = newBullet.GetComponent<Rigidbody>();

                if (bulletRigidbody != null)
                {
                    // 총알에 힘을 가해 속도 조절
                    bulletRigidbody.velocity = direction * bulletSpeed;
                }
            }
        }
    }




















    // #. 패턴 확인용으로 어떤 패턴을 쓰고 있는지 확인하기 위한 함수임
    // 구체 1개 - 공격관련     구체 2개 - 이동 관련      구체 3개 - 색상 변동
    public void PatternCheckSphere(int index)
    {
        for (int i = 0; i < patternSphere.Length; i++)
        {
            if (i <= index)
            {
                // 인덱스까지 활성화
                patternSphere[i].SetActive(true);
            }
            else
            {
                // 나머지는 비활성화
                patternSphere[i].SetActive(false);
            }
        }
    }


    // #. 랜덤한 패턴을 실행시키는 함수
    private void RandomPattern()
    {
        int randomNum = Random.Range(0, 5);
        switch (randomNum)
        {
            case 0:
                
                break;
            //case 1:
            //    CallFunction2();
            //    break;
            //case 2:
            //    CallFunction3();
            //    break;
        }
    }



    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        GameObject playerObject = other.gameObject;
    //        Player playerScript = playerObject.GetComponent<Player>();

    //        // 플레이어 스크립트가 존재하면 플레이어의 체력을 감소시킴
    //        if (playerScript != null)
    //        {
    //            playerScript.OnDamage(damage);
    //        }
    //    }
    //}


}