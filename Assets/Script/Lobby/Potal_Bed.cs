using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Potal_Bed : MonoBehaviour
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
            UnlockCursor(); // 커서 락 해제
            playerInformation.IsMenu = true;
            playerInformation.IsGame = false;
            gameManager.soundManager.Stop();
            gameManager.iconOn = false;

            SceneManager.LoadScene("Timing"); // "Timing" 씬으로 전환
            gameManager.soundTime.Play(); // gameManager에서 사운드를 재생 (GameManager 스크립트와 연결되어야 함)
        }
    }

    private void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}