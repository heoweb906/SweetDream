using UnityEngine.SceneManagement;
using UnityEngine;

public class Potal_2 : MonoBehaviour
{
    public StageManager stageManager;

    private bool hasMoved = false; // 이미 이동했는지 여부를 나타내는 변수

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // "Player" 태그를 가진 오브젝트와 충돌했을 때
        {
            SceneManager.LoadScene("Play3"); // Play 씬 2로 
        }
    }

    public GameObject Wall;
    public Transform targetPosition; // 목표 위치
    public float moveDuration = 2f; // 이동에 걸리는 시간

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
                hasMoved = true; // 이동 완료 후 hasMoved를 true로 설정하여 더 이상 이동하지 않도록 함
            }
        }
    }

    // aaaaa가 true일 때 호출할 함수
    public void MoveWall()
    {
        if (!isMoving)
        {
            isMoving = true;
            moveStartTime = Time.time;
        }
    }
}