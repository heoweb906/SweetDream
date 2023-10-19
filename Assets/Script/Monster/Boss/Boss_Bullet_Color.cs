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
            Debug.LogError("Player 오브젝트를 찾을 수 없습니다.");
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
            // 추적 대상 설정
            if (playerTransform != null)
            {
                guideMissle.SetTarget(playerTransform);
                guideMissle.b_isGuided = true;
            }
        }
    }

    // #. 플레이어의 총알로 맞출 시 반사되는 함수
    public void SetNewDirection(Vector3 direction)
    {
        guideMissle.b_isGuided = false; // 추적을 중지

        Rigidbody bulletRigidbody = gameObject.GetComponent<Rigidbody>();
        if (bulletRigidbody != null)
        {
            bulletRigidbody.velocity = Vector3.zero;
            Debug.Log("총알이 튕겨져 나갑니다");
            bulletRigidbody.velocity = direction * 80f;  // 총알의 속도
        }
    }

}
