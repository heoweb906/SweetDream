using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public GameManager gameManager;

    [Header("몬스터 정보")]
    public int currentHealth;
    public int monsterColor;
    public bool doDie; 


    // 일단 색상을 변경시키기 위해 넣어놨음
    public new Renderer renderer; // 렌더러 컴포넌트
    public Color originalColor; // 원래 머티리얼

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>(); // GameManager를 찾아서 할당
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        renderer.material.color = Color.black;

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
        Destroy(gameObject);
    }
}