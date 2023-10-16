using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Long : Monster
{
    [Header("���� ������Ʈ / ����")]
    public bool isAttack;
    public GameObject bulletPrefab; // �Ѿ� �������� �����ؾ� �մϴ�.

    public Animator anim;
    private Transform player;
    private Rigidbody rigid;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (currentHealth <= 0 && !doDie)
        {
            anim.SetTrigger("doDie");
            //anim.SetBool("boolDie",true);
            doDie = true;
        }


        if (!isAttack && !doDie && gameManager.bpmCount % 5 == 0 && gameManager.bpmCount != 0)  // 5��° bpm ���� �ѹ��� ����
        {
            isAttack = true;
            StartCoroutine(AttackAfterDelay(0.35f));  // 0.35�� �ڿ� ���� �ڷ�ƾ ���� - �ִϸ��̼� �ӵ��� �����ؼ� ���ݰ� BPM�� ��ġ��Ű�� ���ؼ�
        }

        if (!doDie)
        {
            LookAtPlayer(); // �÷��̾� �������� ȸ����Ű�� �Լ�
        }
    }

    private IEnumerator AttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 0.4�� ���
        yield return StartCoroutine(Attack()); // Attack �ڷ�ƾ ����
    }
    IEnumerator Attack()
    {
        //anim.SetTrigger("doAttack");

        yield return new WaitForSeconds(1.5f);
        isAttack = false;
    }


    public void LookAtPlayer()
    {
        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0f; // y�� ȸ���� ������� ����
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation;
        }
    }


    public void ShootAtPlayer() // �ִϸ��̼� �̺�Ʈ�� �����س���
    {
        if (bulletPrefab != null)
        {
            // �÷��̾� ��ġ�� ���� ȸ��
            Vector3 playerPosition = player.position;  
            Vector3 offset = new Vector3(0f, -1.2f, 0f); // �Ʒ��� 1 ������ŭ ������ ������ // �Ѿ��� ��ǥ�� �ϴ� ��ġ�� ����
            Vector3 direction = (playerPosition + offset) - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);

            // �Ѿ��� �����ϰ� �߻�
            Vector3 offset_2 = new Vector3(0f, 2.1f, 1f);
            GameObject bullet = Instantiate(bulletPrefab, transform.position + offset_2, rotation);

            // Ŀ���� ȿ���� �߻縦 ���� �ڷ�ƾ�� ����
            StartCoroutine(EnlargeAndShoot(bullet, 1f, direction));
        }
    }

    private IEnumerator EnlargeAndShoot(GameObject bullet, float enlargeTime, Vector3 direction)
    {
        // �Ѿ��� Ŀ���� ȿ��
        float initialScale = 0.5f; // �ʱ� ũ��
        float finalScale = 3f;    // ���� ũ��
        float elapsedTime = 0f;

        while (elapsedTime < enlargeTime)
        {
            float t = elapsedTime / enlargeTime;
            float scale = Mathf.Lerp(initialScale, finalScale, t);

            bullet.transform.localScale = new Vector3(scale, scale, scale);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        // �Ѿ˿� ���� ���� �߻� (���ϴ� ���� �������� �����ؾ� ��)
        float bulletSpeed = 10f; // �Ѿ� �߻� �ӵ�
        bulletRb.velocity = direction.normalized * bulletSpeed;

        // �Ѿ��� �߻��� �� �� �� �Ŀ� �ڵ����� ���� (���ϴ� �ð����� ���� ����)
        float bulletDestroyDelay = 5f; // �� �� �Ŀ� �������� ����
        Destroy(bullet, bulletDestroyDelay);
    }

}