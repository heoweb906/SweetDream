using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
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
    public Color originalColor; // ���� ��Ƽ����

    // �´��� Ȯ�ο� �׽�Ʈ ����
    private Vector3 originalPosition;


    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>(); // GameManager�� ã�Ƽ� �Ҵ�
        stagemanager = FindObjectOfType<StageManager>();

        originalPosition = transform.position; // ������ ���� ��ġ ����
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        renderer.material.color = Color.black;

        // ���� ������ ���� �̵��ϰ� �س��� �׽�Ʈ���� @@@@@@@@@@@@@@@@@@@@@@@@@@@@
        // ���� �̰� ������ ���͵��� ������ �̻��� ������ �̵��� @@@@@@@@@@@@@@@@@@@@@@
        transform.position = originalPosition + Vector3.up * 0.1f; // ���� ��ġ���� �ణ ���� �̵�
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