using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using TMPro;
using DG.Tweening.Core.Easing;

public class GameManager : MonoBehaviour 
{
    public Player player; // Fever 실행하는 함수에서 할당 받음
    private static GameManager instance;

    [Header("총의 상태")]
    public bool rhythmCorrect; // 박자가 맞은 상황인지, 이게 true인 동안에만 키입력을 인정함
    public bool isReload; // 지금 재장전 중인지
    public int bulletCount; // 남은 총알 개수
    public TMP_Text cruBulletCount;
    [Space(10f)]

    [Header("오브젝트")]
    public Canvas canvas;
    public PlayerInformation playerInformation;
    public Image[] Pins;
    public Image[] HpBars;
    [Space(10f)]


    [Header("리듬 라인")]
    public int bpmCount; // 몬스터 패턴에서 사용할 Cnt 변수
    public bool iconOn; // 판정선을 생성하는 상태인지 아닌지 - 이게 True여야 아이콘이 생성됨
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
    public float iconSpeed = 1.0f; 
    public float iconFadeDuration = 1f; // 페이드 인(서서히 나타나기) 시간


    private List<Image> rhythmImages = new List<Image>();
    [Space(10f)]


    [Header("콤보 시스템")]
    public bool isFever;
    public float initialHeight = 600; // 콤보바 초기 높이
    public Image comboBarImage;
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
                                                                         // 1.5초는 최초 딜레이
    }
    private void SetStartGame()
    {
        iconOn = true;
    }



    private void Update()
    {
        ShowBulletCount();
        //if (playerInformation.IsGame)
        //{
            
        //}
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

        if(!isFever)
        {
            ComboBarBounceDown();
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
        Tweener rhythmTween_sub = RhythmImage_sub.rectTransform.DOAnchorPosX(moveDistance, iconSpeed - 0.03f).SetEase(Ease.Linear);

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

        // #. 생성된 판정선을 리스트에 담음
        rhythmImages.Add(RhythmImage_1);
        rhythmImages.Add(RhythmImage_sub);
        rhythmImages.Add(RhythmImage_2);
    }

    private IEnumerator DestroyAfterDelay(GameObject obj, float delay, float startTime)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null)
        {
            float endTime = Time.time;
            float elapsedTime = endTime - startTime;

            // 이미지를 리스트에서 제거
            Image imageComponent = obj.GetComponent<Image>();
            if (imageComponent != null && rhythmImages.Contains(imageComponent))
            {
                rhythmImages.Remove(imageComponent);
            }
            // 이미지를 파괴
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
        for (int i = 0; i < Pins.Length; i++) // UI 상에서 어떤 무기를 쓰고 있는지 바늘로 찔러서 알려줌
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

    public void ActivateHpImage(int hp) // hp바 업데이트
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


    public void ComboBarBounceDown() // 지속적으로 콤보바의 수치를 낮춰줄 함수
    {
        float newHeight = (int)(comboBarImage.rectTransform.sizeDelta.y - 0.1f);
        comboBarImage.rectTransform.sizeDelta = new Vector2(comboBarImage.rectTransform.sizeDelta.x, newHeight);
    }

    public void ComboBarDown(int downNumber) // 판정에 실패할 시 게이지를 낮춤
    {
        float newHeight = comboBarImage.rectTransform.sizeDelta.y - downNumber;
        comboBarImage.rectTransform.sizeDelta = new Vector2(comboBarImage.rectTransform.sizeDelta.x, newHeight);
    }

    public void ComboBarBounceUp() // 옳은 판정 시에 콤보바 게이지의 높이를 올리는 함수
    {
        float currentHeight = comboBarImage.rectTransform.sizeDelta.y;
        float newHeight = currentHeight + 40; // 높이를 더함

        if (newHeight < 600)
        {
            comboBarImage.rectTransform.sizeDelta = new Vector2(comboBarImage.rectTransform.sizeDelta.x, newHeight);
        }
        else if (newHeight >= 600)
        {
            comboBarImage.rectTransform.sizeDelta = new Vector2(comboBarImage.rectTransform.sizeDelta.x, 600);
            StartCoroutine(ActivateFeverMode());

            player = FindObjectOfType<Player>();
            if (player != null)
            {
                player.FeverOn();
            }
        }
    }

    private IEnumerator ActivateFeverMode()
    {
        isFever = true;
        Pins[0].gameObject.SetActive(true);
        Pins[1].gameObject.SetActive(true);
        Pins[2].gameObject.SetActive(true);
        foreach (var rhythmImage in rhythmImages)
        {
            Destroy(rhythmImage.gameObject);
        }
        rhythmImages.Clear(); // 리스트 비우기
        iconOn = false;

        yield return new WaitForSeconds(5f);

        if (isFever)
        {
            SubFever();
        }
    }
    public void SubFever()
    {
        ComboBarDown(500);
        player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.FeverOff();
        }
        isFever = false;
        iconOn = true;
    }

    public void SetManagerSound()
    {
        soundManager.volume = playerInformation.VolumeBGM;
        soundTime.volume = playerInformation.VolumeBGM;
    }

    public void SetVolume()
    {
        soundManager.volume = playerInformation.VolumeBGM;
        soundTime.volume = playerInformation.VolumeBGM;
    }


    private void ShowBulletCount()
    {
        // #. 재장 전 중일 때 표시할 거
        if (isReload)
        {
            cruBulletCount.text = "X";
        }
        else
        {
            // #. 장전 중이 아닐 때는 최대 탄창의 수를 표시
            cruBulletCount.text = bulletCount.ToString();
        }

        if (isFever)
        {
            cruBulletCount.text = "무무한한대대";
        }
    }
}