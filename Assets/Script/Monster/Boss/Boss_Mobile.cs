using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Mobile : MonoBehaviour
{
    public int damage;
    public float initialVelocity; // �ʱ� �ӵ�
    private Rigidbody rb;

    // #. ���� ���� ���� ����
    public Transform[] spawnPosition;
    public GameObject[] randomMonster;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.down * initialVelocity; // �Ʒ� �������� �ʱ� �ӵ� ����
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

        if (other.CompareTag("Floor"))
        {
            SpawnRandomMonsters();
            Destroy(gameObject);
        }
    }

    // ���� ���� ���� �Լ�
    private void SpawnRandomMonsters()
    {
        for (int i = 0; i < spawnPosition.Length; i++)
        {
            // ������ ���� ������ ����
            int randomMonsterIndex = Random.Range(0, randomMonster.Length);
            GameObject selectedMonster = randomMonster[randomMonsterIndex];

            // ���͸� ���� ��ġ�� ����
            Instantiate(selectedMonster, spawnPosition[i].position, Quaternion.identity);
        }
    }
}
