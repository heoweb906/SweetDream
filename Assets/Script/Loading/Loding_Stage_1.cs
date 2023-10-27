using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening.Core.Easing;

public class Loding_Stage_1 : MonoBehaviour
{
    [Header("�̱����")]
    public GameManager gameManager;
    public PlayerInformation playerInformation;

    /*
        operation.isDone;      // isDone�� �۾��� �Ϸ� ������ bool ���·� ��ȯ�Ѵ�.
        operation.progress;    // ���������� float�� 0, 1�� ��ȯ�Ѵ� (0 - ������, 1 - ����Ϸ�)
        operation.allowSceneActivation;     // true�� �Ǹ� �ٷ� ���� �ѱ��. 
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
                text_Loading.text = "�ε� �Ϸ� - �ƹ� Ű�� ������ ����";
            }


            if (Input.anyKeyDown && progressbar.value >= 1f && operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
                Play();
            }
        }
    }


    public void Play() // ���� ���� ���� ���µ� ���� �� ����
    {
        gameManager.bulletCount = 10; // ���� ù ���۽��� �Ѿ�

        gameManager.soundManager.Play();
        gameManager.GameStart();
        playerInformation.IsMenu = false;
        playerInformation.IsGame = true;


        if (gameManager.isFever) // @@@@@@@@@@@@@@@ �ӽ÷� �ǹ��� ������ �س��� ����
        {
            gameManager.SubFever();
        }

        gameManager.ActivateImage(playerInformation.WeponColor); // ���� ������ �°� �ٴ� UI ������Ʈ
        gameManager.ActivateHpImage(4); // ü�¹� Ȱ��ȭ
        gameManager.SetVolume(); // �����̴� �۾����� �ϴ� ������ Ȥ�� �𸣴ϱ�... 
    }
}
