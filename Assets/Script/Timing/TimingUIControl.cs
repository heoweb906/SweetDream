using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class TimingUIControl : MonoBehaviour
{
    public GameManager gameManager;
    public PlayerInformation playerInformation;

    public TMP_Text jugdeNumber;
    public Image centerIcon;
    public Image timingIcon;
    public RectTransform position_timingIcon;
    public float moveDistance = 1400f; // 이동 거리
    public float iconDestroydeay = 1.1f; // 파괴 시간
    public float iconSpeed = 1.1f; 
    public float iconFadeDuration = 1f; // 페이드 인(서서히 나타나기) 시간


    private float timeSinceLastCreation = 0f;
    private float creationInterval = 2f; // 1초

    private bool hasExecuted = false;


    private float jugdeNum;

    public void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerInformation = FindObjectOfType<PlayerInformation>();
        playerInformation.IsMenu = true;
    }
    private void Update()
    {
        ShowJugdeNumber();
    }
    private void FixedUpdate()
    {
        timeSinceLastCreation += Time.fixedUnscaledDeltaTime;

        if (timeSinceLastCreation >= creationInterval)
        {
            CreateTimingIcon();
            timeSinceLastCreation = 0.0f; // 초기화해서 다음 호출까지 기다립니다.
        }
    }

    private void CreateTimingIcon()
    {
        float startTime = Time.time;

        Image timingIcon_ = Instantiate(timingIcon, position_timingIcon.position, Quaternion.identity);
        timingIcon_.transform.SetParent(position_timingIcon.transform);
        timingIcon_.rectTransform.anchoredPosition = Vector2.zero;
        timingIcon_.color = new Color(0f, 0f, 0f, 1f); // 초기 알파값 0, 빨간색으로 설정
        Tweener timingIcon_main = timingIcon_.rectTransform.DOAnchorPosX(moveDistance, iconSpeed).SetEase(Ease.Linear);

        timingIcon_main.OnUpdate(() =>
        {
            // centerIcon과 timingIcon_의 중점을 계산합니다.
            Vector3 centerPosition = centerIcon.rectTransform.position;
            Vector3 timingIconPosition = timingIcon_.rectTransform.position;

            // 두 아이콘이 일정 거리 안에 들어왔을 때 동작을 실행하도록 설정
            float distanceThreshold = 10f; // 아이콘 간의 거리 임계값
            if (!hasExecuted && Vector3.Distance(centerPosition, timingIconPosition) <= distanceThreshold)
            {
                // centerIcon과 timingIcon_이 만났을 때 실행할 코드를 여기에 작성
                RhythmAnimationCompleted(centerIcon);
                hasExecuted = true; // 한 번만 실행되도록 플래그를 설정
            }
        });

        StartCoroutine(DestroyAfterDelay(timingIcon_.gameObject, iconDestroydeay, startTime));

    }

    public void RhythmAnimationCompleted(Image rhythmImage)
    {
        // 이미지가 겹쳤을 때 일어날 일
        Debug.Log("이미지가 겹쳤습니다");
    }

    private IEnumerator DestroyAfterDelay(GameObject obj, float delay, float startTime)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null)
        {
            hasExecuted = false;

            Destroy(obj);
        }
    }








    public void SceneTurnMenu()
    {
        SceneManager.LoadScene("Menu");
        gameManager.soundTime.Stop();
    }

    public void jugdeMinus()
    {
        if(playerInformation.Jugde > -50)
        {
            playerInformation.Jugde -= 1;
        }
    }

    public void jugdePlus()
    {
        if (playerInformation.Jugde < 50)
        {
            playerInformation.Jugde += 1;
        }
    }

    private void ShowJugdeNumber()
    {
        if(playerInformation.Jugde < 0)
        {
            jugdeNum = playerInformation.Jugde * (0.1f);
            jugdeNumber.text = jugdeNum.ToString();
        }
        else if(playerInformation.Jugde == 0)
        {
            jugdeNumber.text = "0";
        }
        else if(playerInformation.Jugde > 0)
        {
            jugdeNum = playerInformation.Jugde * (0.1f);
            jugdeNumber.text = "+" + jugdeNum.ToString();
        }
    }

}
