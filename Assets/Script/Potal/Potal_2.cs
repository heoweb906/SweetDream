using UnityEngine.SceneManagement;
using UnityEngine;

public class Potal_2 : MonoBehaviour
{
    public StageManager stageManager;

    private bool hasMoved = false; // �̹� �̵��ߴ��� ���θ� ��Ÿ���� ����

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // "Player" �±׸� ���� ������Ʈ�� �浹���� ��
        {
            SceneManager.LoadScene("Play3"); // Play �� 2�� 
        }
    }

    public GameObject Wall;
    public Transform targetPosition; // ��ǥ ��ġ
    public float moveDuration = 2f; // �̵��� �ɸ��� �ð�

    private Vector3 initialPosition;
    private float moveStartTime;
    private bool isMoving = false;

    private void Start()
    {
        initialPosition = Wall.transform.position;
    }

    private void Update()
    {
        if (stageManager.MonsterCount == 0 && !hasMoved)
        {
            MoveWall();
        }

        if (isMoving)
        {
            float elapsedTime = Time.time - moveStartTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);

            Wall.transform.position = Vector3.Lerp(initialPosition, targetPosition.position, t);

            if (t >= 1f)
            {
                isMoving = false;
                hasMoved = true; // �̵� �Ϸ� �� hasMoved�� true�� �����Ͽ� �� �̻� �̵����� �ʵ��� ��
            }
        }
    }

    // aaaaa�� true�� �� ȣ���� �Լ�
    public void MoveWall()
    {
        if (!isMoving)
        {
            isMoving = true;
            moveStartTime = Time.time;
        }
    }
}