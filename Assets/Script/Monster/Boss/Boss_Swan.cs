using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Swan : MonoBehaviour
{
    public GameManager gameManager;
    public StageManager stagemanager;

    [Header("몬스터 정보")]
    public int currentHealth;
    public int monsterColor;
    public bool doDie;


    [Header("관련 변수들")]

    // #. 일단 색상을 변경시키기 위해 넣어놨음
    public new Renderer renderer; // 렌더러 컴포넌트
    public Color originalColor; // 원래 색상
    public Color newColor; // 변경하려는 색상
    private Material monsterMaterial; // 몬스터의 머티리얼 추가


    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>(); // GameManager를 찾아서 할당
        stagemanager = FindObjectOfType<StageManager>();

        monsterMaterial = renderer.material;
        originalColor = monsterMaterial.GetColor("_BaseColor"); // 원래 색상 저장  


    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        renderer.material.color = Color.black;

        if (monsterMaterial != null)
        {
            monsterMaterial.SetColor("_BaseColor", newColor);
        }

        if (currentHealth <= 0)
        {

            Invoke("Die", 3.0f);
        }

        Invoke("ColorBack", 0.1f);
    }
    private void ColorBack()
    {
        if (monsterMaterial != null)
        {
            monsterMaterial.SetColor("_BaseColor", originalColor);
        }
    }

    private void Die()
    {
        stagemanager.MonsterCount--;

        FixPosition(transform.position); // 현재 위치로 고정
        Destroy(gameObject);
    }

    public void FixPosition(Vector3 desiredPosition)
    {
        transform.position = desiredPosition;
    }
}
