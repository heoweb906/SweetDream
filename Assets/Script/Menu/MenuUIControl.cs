using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuUIControl : MonoBehaviour
{
    public GameManager gameManager;
    public PlayerInformation playerInformation;

    public void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerInformation = FindObjectOfType<PlayerInformation>();
        playerInformation.IsMenu = true;
        playerInformation.IsGame = false;
    }

    public void Play() // 게임 시작 시의 상태도 만들 수 있음
    {
        gameManager.bulletCount = 10; // 게임 첫 시작시의 총알

        SceneManager.LoadScene("Play1"); // "YourSceneName"은 이동하고자 하는 씬의 이름으로 바꿔주세요.
        gameManager.soundManager.Play();
        gameManager.GameStart();
        playerInformation.IsMenu = false;
        playerInformation.IsGame = true;

        gameManager.isReload = false;
        gameManager.ActivateImage(playerInformation.WeponColor);
        gameManager.ActivateHpImage(3);
    }

    public void SceneTurnTiming()
    {
        SceneManager.LoadScene("Timing");
        gameManager.soundTime.Play();
    }




    public void QuitGame() // 게임 종료 버튼 클릭(게임 종료)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
