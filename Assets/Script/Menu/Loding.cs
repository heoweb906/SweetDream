using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Loding : MonoBehaviour
{
    /*
        operation.isDone;      // isDone은 작업의 완료 유무를 bool 형태로 반환한다.
        operation.progress;    // 진행정도를 float형 0, 1로 반환한다 (0 - 진행중, 1 - 진행완료)
        operation.allowSceneActivation;     // true가 되면 바로 씬을 넘긴다. 
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
                text_Loading.text = "로딩 완료";
            }


            if(Input.GetKeyDown(KeyCode.Space) && progressbar.value >= 1f && operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }
        }
    }
}
