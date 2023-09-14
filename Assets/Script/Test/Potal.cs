using UnityEngine.SceneManagement;
using UnityEngine;

public class Potal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // "Player" 태그를 가진 오브젝트와 충돌했을 때
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            // 현재 씬 이름에 따라 다른 씬으로 이동
            if (currentSceneName == "Play1")
            {
                SceneManager.LoadScene("Play2"); // 씬 1에서 씬 2로 이동
            }
            else if (currentSceneName == "Play2")
            {
                SceneManager.LoadScene("Play1"); // 씬 2에서 씬 1로 이동
            }
        }
    }
}