using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class TestButton : MonoBehaviour
{
    public GameManager gameManager;
    public PlayerInformation playerInformation;

    public TMP_Text cruBulletCount;

    public void Awake()
    {
        playerInformation = FindObjectOfType<PlayerInformation>();
    }

    public void Update()
    {
        if(playerInformation.IsGame)
        {
            ShowBulletCount();
        }

       

        if (Input.GetKeyDown(KeyCode.T))
        {
            GoMenu();
        }
    }



    private void ShowBulletCount()
    {
        // #. 재장 전 중일 때 표시할 거
        if(gameManager.isReload)
        {
            cruBulletCount.text = "X";
        }
        else
        {
            // #. 장전 중이 아닐 때는 최대 탄창의 수를 표시
            cruBulletCount.text = gameManager.bulletCount.ToString();
        }
    }



    public void GoMenu()
    {
        if (!(playerInformation.IsMenu))  // 메뉴화면이 아닌 경우에만
        {
            UnlockCursor(); // 커서 락 해제

            SceneManager.LoadScene("Menu"); // "YourSceneName"은 이동하고자 하는 씬의 이름으로 바꿔주세요.
            gameManager.soundManager.Stop();
            playerInformation.IsMenu = true;
            playerInformation.IsGame = false;
            gameManager.iconOn = false;
        }
    }
    private void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}