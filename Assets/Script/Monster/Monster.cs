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
    public int damage = 1;
    public bool doDie;


    [Header("���� ������")]

    // #. �ϴ� ������ �����Ű�� ���� �־����
    public new Renderer renderer; // ������ ������Ʈ
    public Color originalColor; // ���� ����
    public Color newColor; // �����Ϸ��� ����
    private Material monsterMaterial; // ������ ��Ƽ���� �߰�


    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>(); // GameManager�� ã�Ƽ� �Ҵ�
        stagemanager = FindObjectOfType<StageManager>();

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
            gameObject.layer = LayerMask.NameToLayer("MonsterDie");
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
        FixPosition(transform.position); // ���� ��ġ�� ����
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject playerObject = other.gameObject;
            Player playerScript = playerObject.GetComponent<Player>();

            // �÷��̾� ��ũ��Ʈ�� �����ϸ� �÷��̾��� ü���� ���ҽ�Ŵ
            if (playerScript != null)
            {
                if(!doDie)
                {
                    playerScript.OnDamage(damage);
                }
            }
        }
    }

    public void FixPosition(Vector3 desiredPosition)
    {
        transform.position = desiredPosition;
    }
}