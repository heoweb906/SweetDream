using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Loding : MonoBehaviour
{
    /*
        operation.isDone;      // isDone�� �۾��� �Ϸ� ������ bool ���·� ��ȯ�Ѵ�.
        operation.progress;    // ���������� float�� 0, 1�� ��ȯ�Ѵ� (0 - ������, 1 - ����Ϸ�)
        operation.allowSceneActivation;     // true�� �Ǹ� �ٷ� ���� �ѱ��. 
    */

    public Slider progressbar;
    public TMP_Text text_Loading;


    private void Start()
    {
        StartCoroutine(LoadScene());
    }


    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation operation = SceneManager.LoadSceneAsync("Menu");
        operation.allowSceneActivation = false;

        while(!(operation.isDone))
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
                text_Loading.text = "�ε� �Ϸ�";
            }


            if(Input.GetKeyDown(KeyCode.Space) && progressbar.value >= 1f && operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }
        }
    }
}
