using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Potal_Stage_1 : MonoBehaviour
{
    public GameManager gameManager;
    public PlayerInformation playerInformation;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerInformation = FindObjectOfType<PlayerInformation>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UnlockCursor(); // Ŀ�� �� ����
            playerInformation.IsMenu = true;
            playerInformation.IsGame = false;
            gameManager.soundManager.Stop();
            gameManager.iconOn = false;

            SceneManager.LoadScene("Loading_Stage_1"); // "Timing" ������ ��ȯ
        }
    }

    private void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}