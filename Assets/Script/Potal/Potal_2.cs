using UnityEngine.SceneManagement;
using UnityEngine;

public class Potal_2 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // "Player" �±׸� ���� ������Ʈ�� �浹���� ��
        {
            SceneManager.LoadScene("Play3"); // Play �� 2�� 
        }
    }
}