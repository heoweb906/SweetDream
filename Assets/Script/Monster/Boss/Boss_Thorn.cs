using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Thorn : MonoBehaviour
{
    public int damage;
    public float moveSpeedUp;
    public float moveSpeedDown;
    public float distance;
    public float stayTime; // �ö󰡰ų� ������ �� �ӹ��� �ð�
    public float destroyTime;

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool isMovingUp = true;

    private void Start()
    {
        initialPosition = transform.position;
        targetPosition = initialPosition + new Vector3(0, distance, 0);
        StartCoroutine(MoveUpAndDown());

        Invoke("DeleteThorn", destroyTime);
    }

    private IEnumerator MoveUpAndDown()
    {
        while (true)
        {
            Vector3 start;
            Vector3 end;

            if (isMovingUp)
            {
                start = initialPosition;
                end = targetPosition;
            }
            else
            {
                start = targetPosition;
                end = initialPosition;
            }

            float journeyLength = Vector3.Distance(start, end);
            float startTime = Time.time;
            float distanceCovered = 0;

            while (distanceCovered < journeyLength)
            {
                float moveSpeed = isMovingUp ? moveSpeedUp : moveSpeedDown;
                float distanceJourney = (Time.time - startTime) * moveSpeed;
                float fractionOfJourney = distanceJourney / journeyLength;

                transform.position = Vector3.Lerp(start, end, fractionOfJourney);

                distanceCovered = Vector3.Distance(start, transform.position);

                yield return null;
            }

            yield return new WaitForSeconds(stayTime);

            isMovingUp = !isMovingUp;
        }
    }

    private void DeleteThorn()
    {
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
                playerScript.OnDamage(damage);
            }
        }
    }
}
