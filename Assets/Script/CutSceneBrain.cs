using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class CutSceneBrain : MonoBehaviour
{
    [Header("아무키나 눌러 시작 관련 변수들")]
    public TMP_Text anyToText;
    public bool bool_isBlinking = false;
    public bool bool_PressButton = false;
    
    private float blinkInterval = 0.4f; // 깜박이는 주기 1
    private float asdasdBlinkInterval = 0.2f; // 깜박이는 주기 2


    [Header("페이드 아웃 관련 함수 들")]
    public Image image_Fade;
    float time = 0f;
    float F_time = 1.5f;


    [SerializeField] UnityEvent enterEvent;
    [SerializeField] UnityEvent exitEvent;

    [Space(10f)]
    [SerializeField] bool CutSceneShowing;
    [SerializeField] int cutIndex = 0;
    [Space(10f)]
    [SerializeField] SingleCutInfo[] cutInfos;
    [SerializeField] float nextCutDelay;
    WaitForSeconds nextCutDelayWFS;

    private bool isExiting = false;

    void Awake()
    {
        nextCutDelayWFS = new WaitForSeconds(nextCutDelay);
        for (int i = 0; i < cutInfos.Length; i++)
        {
            cutInfos[i].Init();
        }

        EnterCutScene();
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            TryNextCut();
            if(bool_isBlinking)
            {
                bool_PressButton = true;
                ExitCutScene();
            }
        }  
        if (Input.GetKeyDown(KeyCode.Escape) && !isExiting) // isExiting 변수를 확인하여 중복으로 ExitCutScene() 호출 방지
            ExitCutScene();
    }

    [ContextMenu("EnterCutScene")]
    public void EnterCutScene()
    {
        if (!CutSceneShowing)
        {
            CutSceneShowing = true;
            enterEvent.Invoke();

            cutIndex = -1;
            ShowNextCut();
        }
    }



    [ContextMenu("ExitCutScene")]
    public void ExitCutScene()
    {
        if (CutSceneShowing && !isExiting) // isExiting 변수를 확인하여 중복으로 처리되지 않도록 함
        {
            isExiting = true; // 출구 상태로 설정
            Fade();
            
        }
    }


    [ContextMenu("TryNextCut")]
    public void TryNextCut()
    {
        if (CutSceneShowing)
        {
            if (cutIndex >= cutInfos.Length)
                ExitCutScene();

            else
                ShowNextCut();
        }
    }

    void ShowNextCut()
    {
        cutIndex++;

        if (cutIndex < cutInfos.Length)
        {
            // 이전 컷 스킵
            if (cutIndex > 0)
                cutInfos[cutIndex - 1].DoEnter_Skip();

            if (CutShowing_C != null)
                StopCoroutine(CutShowing_C);

            // 현재 컷 보여주기
            cutInfos[cutIndex].DoEnter();

            if (cutIndex == 29) // 마지막 컷신 순서
            {
                StartCoroutine(DelayedBlinkText());
                
            }


            // #. 컷씬 애니메이션 관련
            if(cutIndex == 10 || cutIndex == 26 || cutIndex == 27 || cutIndex == 28)  // 위아래 애니메이션을 줄 컷씬 번호들
            {
                cutInfos[cutIndex].DoUpDownAnimation_Position();
            }
            if (cutIndex == 27 || cutIndex == 29)  // 좌우로 움직이는 컷들
            {
                cutInfos[cutIndex].DoUpDownAnimation_Position();
            }
            if (cutIndex == 6 || cutIndex == 9 || cutIndex == 11|| cutIndex == 27)  // 좌우로 기울이는 컷들
            {
                cutInfos[cutIndex].DoLeftRightAnimation_Angle();
            }


            CutShowing_C = CutShowingc();
            StartCoroutine(CutShowing_C);
        }
    }

    //Duration 시간 넘어가면 자연스럽게 다음컷 넘어가기
    IEnumerator CutShowing_C;
    IEnumerator CutShowingc()
    {
        float waitT = cutInfos[cutIndex].Duration;
        while (waitT > 0)
        {
            waitT -= Time.deltaTime;
            yield return null;
        }

        yield return nextCutDelayWFS;
        ShowNextCut();
    }

    private IEnumerator DelayedBlinkText()
    {
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(BlinkText());
    }

    private IEnumerator BlinkText()
    {
        bool_isBlinking = true;

        while (true)
        {
            // 현재 주기에 따라 깜박거림
            anyToText.enabled = !anyToText.enabled;

            // asdasd 값에 따라 주기 변경
            if (bool_PressButton)
            {
                yield return new WaitForSeconds(asdasdBlinkInterval);
            }
            else
            {
                yield return new WaitForSeconds(blinkInterval);
            }
        }
    }


    public void Fade() // 페이드 아웃 함수
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        image_Fade.gameObject.SetActive(true);
        Color alpha = image_Fade.color;
        while (alpha.a < 1f)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(0, 1, time);
            image_Fade.color = alpha;
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("Loading_Menu");
    }

}

[System.Serializable]
public class SingleCutInfo
{
    Image CutImg;
    public RectTransform CutRtf;
    public float Duration;
    public Vector2 StartOffset;
    Vector2 goalPos;

    Color whiteClear = new Color(1, 1, 1, 0);

    public void Init()
    {
        CutRtf.TryGetComponent(out CutImg);
        CutImg.color = Color.clear;
    }

    public void DoEnter()
    {
        goalPos = CutRtf.anchoredPosition;

        CutRtf.anchoredPosition = goalPos + StartOffset;
        CutRtf.DOAnchorPos(goalPos, Duration);

        CutImg.color = whiteClear;
        CutImg.DOColor(Color.white, Duration);
    }

    public void DoEnter_Skip()
    {
        CutRtf.DOKill();
        CutImg.DOKill();

        CutRtf.anchoredPosition = goalPos;
        CutImg.color = Color.white;
    }

    public void DoExit()
    {
        CutRtf.DOKill();
        CutImg.DOKill();

        CutImg.DOColor(whiteClear, 0.5f);
    }

    public void DoUpDownAnimation_Position() // 위아래로 움직이는 애니메이션
    {
        // 아래로 이동하는 애니메이션
        Vector2 downPosition = goalPos - Vector2.up * 30f; // 아래로 이동할 위치 (30 유닛 아래로)

        // 위아래로 반복되는 애니메이션
        Sequence upDownSequence = DOTween.Sequence()
            .Append(CutRtf.DOAnchorPosY(downPosition.y, 0.5f).SetEase(Ease.Linear)) // 아래로 이동 (0.5초 동안)
            .Append(CutRtf.DOAnchorPosY(goalPos.y, 0.5f).SetEase(Ease.Linear)); // 다시 원래 위치로 이동 (0.5초 동안)

        upDownSequence.SetLoops(-1); // 무한 반복
    }

    public void DoLeftRightAnimation_Position()  // 좌우로 움직이는 애니메이션
    {
        // 왼쪽으로 기울이는 애니메이션
        Vector2 leftPosition = goalPos - Vector2.right * 30f; // 왼쪽으로 기울일 위치 (30 유닛 왼쪽으로)

        // 좌우로 반복되는 애니메이션
        Sequence leftRightSequence = DOTween.Sequence()
            .Append(CutRtf.DOAnchorPosX(leftPosition.x, 0.5f).SetEase(Ease.Linear)) // 왼쪽으로 기울임 (0.5초 동안)
            .Append(CutRtf.DOAnchorPosX(goalPos.x, 0.5f).SetEase(Ease.Linear)); // 다시 원래 위치로 복귀 (0.5초 동안)

        leftRightSequence.SetLoops(-1); // 무한 반복
    }

    public void DoLeftRightAnimation_Angle()
    {
        // 좌우로 기울이는 애니메이션
        Quaternion leftRotation = Quaternion.Euler(0f, 0f, 10f); // 왼쪽으로 기울일 각도 (10도)

        // 좌우로 반복되는 애니메이션
        Sequence leftRightSequence = DOTween.Sequence()
            .Append(CutRtf.DORotateQuaternion(leftRotation, 0.5f).SetEase(Ease.Linear)) // 왼쪽으로 기울임 (0.5초 동안)
            .Append(CutRtf.DORotateQuaternion(Quaternion.identity, 0.5f).SetEase(Ease.Linear)); // 다시 원래 각도로 복귀 (0.5초 동안)

        leftRightSequence.SetLoops(-1); // 무한 반복
    }
}