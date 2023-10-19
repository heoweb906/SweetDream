using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedMissile : MonoBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 5f;
    public bool b_isGuided;
    private Transform target;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void Update()
    {
        if(b_isGuided)
        {
            // #. ���� �� ����Ǵ� velocity�� 0���� ���༭ ���������� �÷��̾ ����� �� �ֵ���
            Rigidbody bulletRigidbody = gameObject.GetComponent<Rigidbody>();
            bulletRigidbody.velocity = Vector3.zero; 

            Vector3 dir = target.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }
}





