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
            // #. 생성 시 적용되는 velocity를 0으로 해줘서 정상적으로 플레이어를 따라올 수 있도록
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





