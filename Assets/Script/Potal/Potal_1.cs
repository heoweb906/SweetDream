using UnityEngine.SceneManagement;
using UnityEngine;

public class Potal_1 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // "Player" 태그를 가진 오브젝트와 충돌했을 때
        {
            SceneManager.LoadScene("Play2"); // Play 씬 2로 
        }
    }
}