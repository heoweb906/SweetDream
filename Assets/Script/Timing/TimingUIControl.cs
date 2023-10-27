using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class TimingUIControl : MonoBehaviour
{
    [Header("������ ����")]
    public GameManager gameManager;
    public PlayerInformation playerInformation;

    public Canvas canvasTiming;
    public TMP_Text jugdeNumber;
    public bool iconOn;
    public Image centerIcon;
    public Image timingIcon;
    public Image timingIcon_Sand;
    public RectTransform position_timingIcon;
    public float moveDistance = 1000f; // �̵� �Ÿ�
    public float iconDestroydeay = 4f; // �ı� �ð�
    public float iconSpeed = 1f; 
    public float iconFadeDuration = 1f; // ���̵� ��(������ ��Ÿ����) �ð�


    private float timeSinceLastCreation = 0f;
    private float creationInterval = 1f; // 1��

    private bool hasExecuted = false;

    private float jugdeNum;
    [Space(30f)]


    [Header("���콺 ���� / ���� ����")]
    public float mouseFloat;
    public Slider mouseSensitivitySlider; 
    public float volumeBGM;
    public float volumeEffect;
    public Slider volumeBGMSlider;
    public Slider volumeEffectSlider;

    public void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerInformation = FindObjectOfType<PlayerInformation>();
        playerInformation.IsMenu = true;
    }
    public void Start()
    {
        // #. ���� ��ġ ������ �����̴��� ������
        mouseFloat = playerInformation.MouseSpeed; 
        mouseSensitivitySlider.value = mouseFloat; 
        volumeBGM = playerInformation.VolumeBGM;
        volumeBGMSlider.value = volumeBGM;
        volumeEffect = playerInformation.VolumeEffect;
        volumeEffectSlider.value = volumeEffect;

        // #. ��� ������ �̵�
        Vector3 centerIconPosition__ = centerIcon.rectTransform.anchoredPosition;
        centerIconPosition__.x = playerInformation.Jugde * 5f; // ���ϴ� �̵� �Ÿ�
        centerIcon.rectTransform.anchoredPosition = centerIconPosition__;

        TimingStart();
    }

    public void TimingStart()
    {
        Invoke("SetStartTimiig", playerInformation.Jugde * 0.01f + 1.5f);  // �̰ɷ� ������ ���� ����
                                                                         // 1.5�ʴ� ���� ������
    }
    private void SetStartTimiig()
    {
        iconOn = true;
    }


    private void Update()
    {
        ShowJugdeNumber();
    }
    private void FixedUpdate()
    {
        if (iconOn)
        {
            timeSinceLastCreation += Time.fixedUnscaledDeltaTime;

            if (timeSinceLastCreation >= creationInterval)
            {
                CreateTimingIcon();
                timeSinceLastCreation = 0.0f; // �ʱ�ȭ�ؼ� ���� ȣ����� ��ٸ��ϴ�.
            }
        }
    }

    private void CreateTimingIcon()
    {
        float startTime = Time.time;

        Image timingIcon_ = Instantiate(timingIcon, position_timingIcon.position, Quaternion.identity);
        timingIcon_.transform.SetParent(position_timingIcon.transform);
        timingIcon_.rectTransform.anchoredPosition = Vector2.zero;
        timingIcon_.color = new Color(0f, 0f, 0f, 1f); // �ʱ� ���İ� 0, ���������� ����
        Tweener timingIcon_main = timingIcon_.rectTransform.DOAnchorPosX(moveDistance, iconSpeed).SetEase(Ease.Linear);

        timingIcon_main.OnUpdate(() =>
        {
            // centerIcon�� timingIcon_�� ������ ����մϴ�.
            Vector3 centerPosition = centerIcon.rectTransform.position;
            Vector3 timingIconPosition = timingIcon_.rectTransform.position;

            // �� �������� ���� �Ÿ� �ȿ� ������ �� ������ �����ϵ��� ����
            float distanceThreshold = 10f; // ������ ���� �Ÿ� �Ӱ谪
            if (!hasExecuted && Vector3.Distance(centerPosition, timingIconPosition) <= distanceThreshold)
            {
                // centerIcon�� timingIcon_�� ������ �� ������ �ڵ带 ���⿡ �ۼ�
                RhythmAnimationCompleted(centerIcon);
                hasExecuted = true; // �� ���� ����ǵ��� �÷��׸� ����
            }
        });
        StartCoroutine(DestroyAfterDelay(timingIcon_.gameObject, iconDestroydeay, startTime));
    }

    public void RhythmAnimationCompleted(Image rhythmImage)
    {
        Image timingIconSand_ = Instantiate(timingIcon_Sand, rhythmImage.rectTransform.position, Quaternion.identity);
        timingIconSand_.transform.SetParent(canvasTiming.transform);

        timingIconSand_.DOFade(0f, 0.5f); // ���İ� ������ 1�� ����

        Debug.Log("�̹����� ���ƽ��ϴ�");
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
    public void jugdeMinus()
    {
        if(playerInformation.Jugde > -50)
        {
            playerInformation.Jugde -= 1;

            // #. ��� ������ �̵�
            Vector3 centerIconPosition = centerIcon.rectTransform.anchoredPosition;
            centerIconPosition.x -= 5f; // ���ϴ� �̵� �Ÿ�
            centerIcon.rectTransform.anchoredPosition = centerIconPosition;
        }
    }
    public void jugdePlus()
    {
        if (playerInformation.Jugde < 50)
        {
            playerInformation.Jugde += 1;

            // #. ��� ������ �̵�
            Vector3 centerIconPosition = centerIcon.rectTransform.anchoredPosition;
            centerIconPosition.x += 5f; // ���ϴ� �̵� �Ÿ�
            centerIcon.rectTransform.anchoredPosition = centerIconPosition;
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



    // @@@@@@@@@@@@@@@@@@@@@@@
    // #. ��Ÿ ���� ����
    // @@@@@@@@@@@@@@@@@@@@@@@

    public void OnMouseSensitivityChanged(float value) // ���콺 ���� ���� �����̴� �Լ�
    {
        // �Ҽ��� 2�ڸ����� �ݿø��Ͽ� mouseFloat�� �Ҵ�
        mouseFloat = Mathf.Round(value * 100) / 100;
        playerInformation.MouseSpeed = mouseFloat;
    }

    public void OnBGM_SoundSensitivityChanged(float value) // ������� ũ�� ���� �����̴� �Լ�
    {
        volumeBGM = Mathf.Round(value * 100) / 100;
        playerInformation.VolumeBGM = volumeBGM;
        gameManager.SetVolume();
    }

    public void OnEffect_SoundSensitivityChanged(float value) // ȿ���� ũ�� ���� �����̴� �Լ�
    {
        volumeEffect = Mathf.Round(value * 100) / 100;
        playerInformation.VolumeEffect = volumeEffect;
        gameManager.SetVolume();
    }



    public void SceneTurnMenu()
    {
        gameManager.soundTime.Stop();

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

        SceneManager.LoadScene("Lobby");
 
    }

}
