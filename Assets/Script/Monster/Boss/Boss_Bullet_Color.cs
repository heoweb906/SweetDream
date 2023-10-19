using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Boss_Bullet_Color : Boss_Bullet
{
    public int numColor;
    private Transform playerTransform;
    public GuidedMissile guideMissle;

    private void Awake()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    private void Start()
    {
        StartCoroutine(TurnIntoGuidedMissile());
    }

    private IEnumerator TurnIntoGuidedMissile()
    {
        yield return new WaitForSeconds(0.4f);

        if (guideMissle != null)
        {
            // ���� ��� ����
            if (playerTransform != null)
            {
                guideMissle.SetTarget(playerTransform);
                guideMissle.b_isGuided = true;
            }
        }
    }

    // #. �÷��̾��� �Ѿ˷� ���� �� �ݻ�Ǵ� �Լ�
    public void SetNewDirection(Vector3 direction)
    {
        guideMissle.b_isGuided = false; // ������ ����

        Rigidbody bulletRigidbody = gameObject.GetComponent<Rigidbody>();
        if (bulletRigidbody != null)
        {
            bulletRigidbody.velocity = Vector3.zero;
            Debug.Log("�Ѿ��� ƨ���� �����ϴ�");
            bulletRigidbody.velocity = direction * 80f;  // �Ѿ��� �ӵ�
        }
    }

}
