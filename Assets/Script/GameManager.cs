using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Net.NetworkInformation;

public class GameManager : MonoBehaviour 
{
    private static GameManager instance;

    [Header("총의 상태")]
    public bool rhythmCorrect; // 박자가 맞은 상황인지, 이게 true인 동안에만 키입력을 인정함
    public bool isReload; // 지금 재장전 중인지
    public int bulletCount; // 남은 총알 개수
    [Space(10f)]

    [Header("기타 변수들")]

    [Header("오브젝트")]
    public Canvas canvas;
    public PlayerInformation playerInformation;
    public Image[] Pins;
    public Image[] HpBars;
    [Space(10f)]


    [Header("리듬 라인")]
    public int bpmCount; // 몬스터 패턴에서 사용할 Cnt 변수

    public bool iconOn;
    public AudioSource soundManager; // 게임 배경음악
    public AudioSource soundTime; // 타이밍 맞추는 매트로놈
    public Image aim_Around;
    public RectTransform rhythmPosition_1;
    public RectTransform rhythmPosition_2;
    public RectTransform rhythmPosition_sub;
    public Image rhythmSpriteRenderer_left;
    public Image rhythmSpriteRenderer_right;
    public float moveDistance = 250f; // 이동 거리
    public float iconDestroydeay = 1.2f; // 파괴 시간
    public float iconSpeed = 1.1f; 
    public float iconFadeDuration = 1f; // 페이드 인(서서히 나타나기) 시간
    [Space(10f)]

    [Header("테스트 중")]
    private float timeSinceLastCreation = 0.0f;
    //private float creationInterval = 1f / (120f / 60f); // 1초
    private float creationInterval = 0.5f; // 1초

    private void Awake()
    {
        playerInformation = FindObjectOfType<PlayerInformation>();

        if (instance == null)
        {
            bulletCount = 10;

            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GameStart()
    {
        Invoke("SetStartGame", playerInformation.Jugde * 0.01f + 1.5f);  // 이걸로 판정을 맞출 거임
    }

    private void SetStartGame()
    {
        iconOn = true;
    }


    private void FixedUpdate()
    {   
        if(iconOn)
        {
            timeSinceLastCreation += Time.fixedUnscaledDeltaTime;

            if (timeSinceLastCreation >= creationInterval)
            {
                CreateRhythmIcon();
                timeSinceLastCreation = 0.0f; // 초기화해서 다음 호출까지 기다립니다.
            }
        }
    }


    private void CreateRhythmIcon()
    {
        float startTime = Time.time;

        Image RhythmImage_1 = Instantiate(rhythmSpriteRenderer_left, rhythmPosition_1.position, Quaternion.identity);
        RhythmImage_1.transform.SetParent(rhythmPosition_1.transform); // 리듬 위치의 자식으로 설정

        RhythmImage_1.transform.localScale = Vector3.one;
        
        // 이미지 생성 후 애니메이션 및 페이드 인 설정
        RhythmImage_1.rectTransform.anchoredPosition = Vector2.zero;
        RhythmImage_1.color = new Color(1f, 1f, 1f, 0.2f); // 초기 알파값 0, 빨간색으로 설정
        Tweener rhythmTween = RhythmImage_1.rectTransform.DOAnchorPosX(moveDistance, iconSpeed).SetEase(Ease.Linear);
        RhythmImage_1.DOFade(1f, iconFadeDuration); // 알파값 서서히 1로 변경
        StartCoroutine(DestroyAfterDelay(RhythmImage_1.gameObject, iconDestroydeay, startTime)); // 일정 시간 후에 이미지 파괴



        Image RhythmImage_sub = Instantiate(rhythmSpriteRenderer_left, rhythmPosition_sub.position, Quaternion.identity);
        RhythmImage_sub.transform.SetParent(rhythmPosition_sub.transform);
        RhythmImage_sub.rectTransform.anchoredPosition = Vector2.zero;
        RhythmImage_sub.color = new Color(1f, 0f, 0f, 0f); // 초기 알파값 0, 빨간색으로 설정
        Tweener rhythmTween_sub = RhythmImage_sub.rectTransform.DOAnchorPosX(moveDistance, iconSpeed - 1f).SetEase(Ease.Linear);

        rhythmTween_sub.OnComplete(() =>
        {
            RhythmAnimationCompleted(aim_Around);
            bpmCount++;
        });

        Image RhythmImage_2 = Instantiate(rhythmSpriteRenderer_right, rhythmPosition_2.position, Quaternion.identity);
        RhythmImage_2.transform.SetParent(rhythmPosition_2.transform); // 리듬 위치의 자식으로 설정

        RhythmImage_2.transform.localScale = Vector3.one;

        // 이미지 생성 후 애니메이션 및 페이드 인 설정
        RhythmImage_2.rectTransform.anchoredPosition = Vector2.zero;
        RhythmImage_2.color = new Color(1f, 1f, 1f, 0.2f); // 초기 알파값 0, 빨간색으로 설정
        RhythmImage_2.rectTransform.DOAnchorPosX(-moveDistance, iconSpeed).SetEase(Ease.Linear);
        RhythmImage_2.DOFade(1f, iconFadeDuration); // 알파값 서서히 1로 변경

        StartCoroutine(DestroyAfterDelay(RhythmImage_2.gameObject, iconDestroydeay, startTime)); // 일정 시간 후에 이미지 파괴
    }

    private IEnumerator DestroyAfterDelay(GameObject obj, float delay, float startTime)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null)
        {
            float endTime = Time.time;
            float elapsedTime = endTime - startTime;

            //Debug.Log("이미지가 생성된 후 " + elapsedTime + "초 뒤에 사라짐");

            Destroy(obj);
        }
    }

    public void RhythmAnimationCompleted(Image rhythmImage)
    {
        StartCoroutine(SetRhythmCorrectWithDelay(0.17f));  // @@@@@@@@@@@@@ 0.2초 동안만 판정을 옮게 함
        //Debug.Log("RhythmImage_1 애니메이션 완료 및 함수 호출");
        // rhythmImage는 애니메이션이 완료된 이미지입니다.
    }

    private IEnumerator SetRhythmCorrectWithDelay(float delay)
    {
        rhythmCorrect = true; // rhythmCorrect를 true로 설정
        yield return new WaitForSeconds(delay); // 지정된 시간만큼 대기
        rhythmCorrect = false; // 지정된 시간 후에 rhythmCorrect를 다시 false로 설정
    }


    public void ActivateImage(int imageIndex)
    {
        for (int i = 0; i < Pins.Length; i++)
        {
            Pins[i].gameObject.SetActive(false);
        }

        if(imageIndex == 1 && !isReload)
        {
            Pins[0].gameObject.SetActive(true);
        }
        if (imageIndex == 2 && !isReload)
        {
            Pins[1].gameObject.SetActive(true);
        }
        if (imageIndex == 3 && !isReload)
        {
            Pins[2].gameObject.SetActive(true);
        }
    }

    public void ActivateHpImage(int hp)
    {

        for (int i = 0; i < HpBars.Length; i++)
        {
            HpBars[i].gameObject.SetActive(false);
        }
        if(hp >= 0)
        {
            HpBars[hp].gameObject.SetActive(true);
        }
        
    }
}