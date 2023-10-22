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
    [Header("�ƹ�Ű�� ���� ���� ���� ������")]
    public TMP_Text anyToText;
    public bool bool_isBlinking = false;
    public bool bool_PressButton = false;
    
    private float blinkInterval = 0.4f; // �����̴� �ֱ� 1
    private float asdasdBlinkInterval = 0.2f; // �����̴� �ֱ� 2


    [Header("���̵� �ƿ� ���� �Լ� ��")]
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
        if (Input.GetKeyDown(KeyCode.Escape) && !isExiting) // isExiting ������ Ȯ���Ͽ� �ߺ����� ExitCutScene() ȣ�� ����
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
        if (CutSceneShowing && !isExiting) // isExiting ������ Ȯ���Ͽ� �ߺ����� ó������ �ʵ��� ��
        {
            isExiting = true; // �ⱸ ���·� ����
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
            // ���� �� ��ŵ
            if (cutIndex > 0)
                cutInfos[cutIndex - 1].DoEnter_Skip();

            if (CutShowing_C != null)
                StopCoroutine(CutShowing_C);

            // ���� �� �����ֱ�
            cutInfos[cutIndex].DoEnter();

            if (cutIndex == 30) // ������ �ƽ� ����
            {
                StartCoroutine(DelayedBlinkText());
                
            }


            // #. �ƾ� �ִϸ��̼� ����
            if(cutIndex == 11 || cutIndex == 27 || cutIndex == 28 || cutIndex == 29)  // ���Ʒ� �ִϸ��̼��� �� �ƾ� ��ȣ��
            {
                cutInfos[cutIndex].DoUpDownAnimation_Position();
            }
            if (cutIndex == 29 || cutIndex == 30)  // �¿�� �����̴� �Ƶ�
            {
                cutInfos[cutIndex].DoUpDownAnimation_Position();
            }
            if (cutIndex == 7 || cutIndex == 10 || cutIndex == 12 || cutIndex == 29)  // �¿�� ����̴� �Ƶ�
            {
                cutInfos[cutIndex].DoLeftRightAnimation_Angle();
            }


            CutShowing_C = CutShowingc();
            StartCoroutine(CutShowing_C);
        }
    }

    //Duration �ð� �Ѿ�� �ڿ������� ������ �Ѿ��
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
            // ���� �ֱ⿡ ���� ���ڰŸ�
            anyToText.enabled = !anyToText.enabled;

            // asdasd ���� ���� �ֱ� ����
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


    public void Fade() // ���̵� �ƿ� �Լ�
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
        SceneManager.LoadScene("Loading_Lobby");
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

    public void DoUpDownAnimation_Position() // ���Ʒ��� �����̴� �ִϸ��̼�
    {
        // �Ʒ��� �̵��ϴ� �ִϸ��̼�
        Vector2 downPosition = goalPos - Vector2.up * 30f; // �Ʒ��� �̵��� ��ġ (30 ���� �Ʒ���)

        // ���Ʒ��� �ݺ��Ǵ� �ִϸ��̼�
        Sequence upDownSequence = DOTween.Sequence()
            .Append(CutRtf.DOAnchorPosY(downPosition.y, 0.5f).SetEase(Ease.Linear)) // �Ʒ��� �̵� (0.5�� ����)
            .Append(CutRtf.DOAnchorPosY(goalPos.y, 0.5f).SetEase(Ease.Linear)); // �ٽ� ���� ��ġ�� �̵� (0.5�� ����)

        upDownSequence.SetLoops(-1); // ���� �ݺ�
    }

    public void DoLeftRightAnimation_Position()  // �¿�� �����̴� �ִϸ��̼�
    {
        // �������� ����̴� �ִϸ��̼�
        Vector2 leftPosition = goalPos - Vector2.right * 30f; // �������� ����� ��ġ (30 ���� ��������)

        // �¿�� �ݺ��Ǵ� �ִϸ��̼�
        Sequence leftRightSequence = DOTween.Sequence()
            .Append(CutRtf.DOAnchorPosX(leftPosition.x, 0.5f).SetEase(Ease.Linear)) // �������� ����� (0.5�� ����)
            .Append(CutRtf.DOAnchorPosX(goalPos.x, 0.5f).SetEase(Ease.Linear)); // �ٽ� ���� ��ġ�� ���� (0.5�� ����)

        leftRightSequence.SetLoops(-1); // ���� �ݺ�
    }

    public void DoLeftRightAnimation_Angle()
    {
        // �¿�� ����̴� �ִϸ��̼�
        Quaternion leftRotation = Quaternion.Euler(0f, 0f, 10f); // �������� ����� ���� (10��)

        // �¿�� �ݺ��Ǵ� �ִϸ��̼�
        Sequence leftRightSequence = DOTween.Sequence()
            .Append(CutRtf.DORotateQuaternion(leftRotation, 0.5f).SetEase(Ease.Linear)) // �������� ����� (0.5�� ����)
            .Append(CutRtf.DORotateQuaternion(Quaternion.identity, 0.5f).SetEase(Ease.Linear)); // �ٽ� ���� ������ ���� (0.5�� ����)

        leftRightSequence.SetLoops(-1); // ���� �ݺ�
    }
}