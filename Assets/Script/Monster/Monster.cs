using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public GameManager gameManager;
    public StageManager stagemanager;

    [Header("몬스터 정보")]
    public int currentHealth;
    public int monsterColor;
    public bool doDie; 

    // 일단 색상을 변경시키기 위해 넣어놨음
    public new Renderer renderer; // 렌더러 컴포넌트
    public Color originalColor; // 원래 머티리얼

    // 맞는지 확인용 테스트 변수
    private Vector3 originalPosition;


    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>(); // GameManager를 찾아서 할당
        stagemanager = FindObjectOfType<StageManager>();

        originalPosition = transform.position; // 몬스터의 원래 위치 저장
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        renderer.material.color = Color.black;

        // 맞을 때마다 위로 이동하게 해놨음 테스트용임 @@@@@@@@@@@@@@@@@@@@@@@@@@@@
        // 지금 이거 때문에 몬스터들이 맞으면 이상한 곳으로 이동함 @@@@@@@@@@@@@@@@@@@@@@
        transform.position = originalPosition + Vector3.up * 0.1f; // 원래 위치에서 약간 위로 이동
        originalPosition = transform.position;

        if (currentHealth <= 0)
        {
            Invoke("Die", 1.5f);
        }

        Invoke("ColorBack", 0.1f);
    }
    private void ColorBack()
    {
        renderer.material.color = originalColor;
    }

    private void Die()
    {
        stagemanager.MonsterCount--;
        Destroy(gameObject);
    }
}