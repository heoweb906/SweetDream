using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public GameManager gameManager;
    public StageManager stagemanager;

    [Header("���� ����")]
    public int currentHealth;
    public int monsterColor;
    public bool doDie; 

    // �ϴ� ������ �����Ű�� ���� �־����
    public new Renderer renderer; // ������ ������Ʈ
    public Color originalColor; // ���� ����
    public Color newColor; // �����Ϸ��� ����
    private Material monsterMaterial; // ������ ��Ƽ���� �߰�


    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>(); // GameManager�� ã�Ƽ� �Ҵ�
        stagemanager = FindObjectOfType<StageManager>();

        renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            monsterMaterial = renderer.material;
            originalColor = monsterMaterial.GetColor("_BaseColor"); // ���� ���� ����
        }
        else
        {
            Debug.LogError("Renderer ������Ʈ�� ã�� �� �����ϴ�.");
        }
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
            Invoke("Die", 1.5f);
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
        Destroy(gameObject);
    }
}