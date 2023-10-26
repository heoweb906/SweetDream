using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening.Core.Easing;

public class Loding_Stage_1 : MonoBehaviour
{
    [Header("싱글톤들")]
    public GameManager gameManager;
    public PlayerInformation playerInformation;

    /*
        operation.isDone;      // isDone은 작업의 완료 유무를 bool 형태로 반환한다.
        operation.progress;    // 진행정도를 float형 0, 1로 반환한다 (0 - 진행중, 1 - 진행완료)
        operation.allowSceneActivation;     // true가 되면 바로 씬을 넘긴다. 
    */

    public Slider progressbar;
    public TMP_Text text_Loading;


    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerInformation = FindObjectOfType<PlayerInformation>();

        playerInformation.IsMenu = true;
        playerInformation.IsGame = false;

        StartCoroutine(LoadScene());
    }


    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation operation = SceneManager.LoadSceneAsync("Play1");
        operation.allowSceneActivation = false;

        while (!(operation.isDone))
        {
            yield return null;

            if (progressbar.value < 0.9f)
            {
                progressbar.value = Mathf.MoveTowards(progressbar.value, 0.9f, Time.deltaTime);
            }
            else if (progressbar.value >= 0.9f)
            {
                progressbar.value = Mathf.MoveTowards(progressbar.value, 1f, Time.deltaTime);
            }


            if (progressbar.value >= 1f)
            {
                text_Loading.text = "로딩 완료 - 아무 키나 눌러서 시작";
            }


            if (Input.anyKeyDown && progressbar.value >= 1f && operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
                Play();
            }
        }
    }


    public void Play() // 게임 시작 시의 상태도 만들 수 있음
    {
        gameManager.bulletCount = 10; // 게임 첫 시작시의 총알

        gameManager.soundManager.Play();
        gameManager.GameStart();
        playerInformation.IsMenu = false;
        playerInformation.IsGame = true;

        gameManager.isReload = false;

        if (gameManager.isFever) // @@@@@@@@@@@@@@@ 임시로 피버를 끝나게 해놓은 거임
        {
            gameManager.SubFever();
        }

        gameManager.ActivateImage(playerInformation.WeponColor); // 무기 정보에 맞게 바늘 UI 업데이트
        gameManager.ActivateHpImage(4); // 체력바 활성화
        gameManager.SetVolume(); // 슬라이더 작업에서 하는 거지만 혹시 모르니까... 
    }
}
