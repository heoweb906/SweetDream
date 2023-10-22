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
    public Player player; // Fever �����ϴ� �Լ����� �Ҵ� ����
    private static GameManager instance;

    [Header("���� ����")]
    public bool rhythmCorrect; // ���ڰ� ���� ��Ȳ����, �̰� true�� ���ȿ��� Ű�Է��� ������
    public bool isReload; // ���� ������ ������
    public int bulletCount; // ���� �Ѿ� ����
    public TMP_Text cruBulletCount;
    [Space(10f)]

    [Header("������Ʈ")]
    public Canvas canvas;
    public PlayerInformation playerInformation;
    public Image[] Pins;
    public Image[] HpBars;
    [Space(10f)]


    [Header("���� ����")]
    public int bpmCount; // ���� ���Ͽ��� ����� Cnt ����
    public bool iconOn; // �������� �����ϴ� �������� �ƴ��� - �̰� True���� �������� ������
    public AudioSource soundManager; // ���� �������
    public AudioSource soundTime; // Ÿ�̹� ���ߴ� ��Ʈ�γ�
    public Image aim_Around;
    public RectTransform rhythmPosition_1;
    public RectTransform rhythmPosition_2;
    public RectTransform rhythmPosition_sub;
    public Image rhythmSpriteRenderer_left;
    public Image rhythmSpriteRenderer_right;
    public float moveDistance = 250f; // �̵� �Ÿ�
    public float iconDestroydeay = 1.2f; // �ı� �ð�
    public float iconSpeed = 1.0f; 
    public float iconFadeDuration = 1f; // ���̵� ��(������ ��Ÿ����) �ð�


    private List<Image> rhythmImages = new List<Image>();
    [Space(10f)]


    [Header("�޺� �ý���")]
    public bool isFever;
    public float initialHeight = 600; // �޺��� �ʱ� ����
    public Image comboBarImage;
    [Space(10f)]


    [Header("�׽�Ʈ ��")]
    private float timeSinceLastCreation = 0.0f;
    //private float creationInterval = 1f / (120f / 60f); // 1��
    private float creationInterval = 0.5f; // 1��

    private void Awake()
    {
        playerInformation = FindObjectOfType<PlayerInformation>();

        if (instance == null)
        {
            bulletCount = 10;

            instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void GameStart()
    {
        Invoke("SetStartGame", playerInformation.Jugde * 0.01f + 1.5f);  // �̰ɷ� ������ ���� ����
                                                                         // 1.5�ʴ� ���� ������
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
                timeSinceLastCreation = 0.0f; // �ʱ�ȭ�ؼ� ���� ȣ����� ��ٸ��ϴ�.
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
        RhythmImage_1.transform.SetParent(rhythmPosition_1.transform); // ���� ��ġ�� �ڽ����� ����

        RhythmImage_1.transform.localScale = Vector3.one;
        
        // �̹��� ���� �� �ִϸ��̼� �� ���̵� �� ����
        RhythmImage_1.rectTransform.anchoredPosition = Vector2.zero;
        RhythmImage_1.color = new Color(1f, 1f, 1f, 0.2f); // �ʱ� ���İ� 0, ���������� ����
        Tweener rhythmTween = RhythmImage_1.rectTransform.DOAnchorPosX(moveDistance, iconSpeed).SetEase(Ease.Linear);
        RhythmImage_1.DOFade(1f, iconFadeDuration); // ���İ� ������ 1�� ����
        StartCoroutine(DestroyAfterDelay(RhythmImage_1.gameObject, iconDestroydeay, startTime)); // ���� �ð� �Ŀ� �̹��� �ı�



        Image RhythmImage_sub = Instantiate(rhythmSpriteRenderer_left, rhythmPosition_sub.position, Quaternion.identity);
        RhythmImage_sub.transform.SetParent(rhythmPosition_sub.transform);
        RhythmImage_sub.rectTransform.anchoredPosition = Vector2.zero;
        RhythmImage_sub.color = new Color(1f, 0f, 0f, 0f); // �ʱ� ���İ� 0, ���������� ����
        Tweener rhythmTween_sub = RhythmImage_sub.rectTransform.DOAnchorPosX(moveDistance, iconSpeed - 0.03f).SetEase(Ease.Linear);

        rhythmTween_sub.OnComplete(() =>
        {
            RhythmAnimationCompleted(aim_Around);
            bpmCount++;
        });

        Image RhythmImage_2 = Instantiate(rhythmSpriteRenderer_right, rhythmPosition_2.position, Quaternion.identity);
        RhythmImage_2.transform.SetParent(rhythmPosition_2.transform); // ���� ��ġ�� �ڽ����� ����

        RhythmImage_2.transform.localScale = Vector3.one;

        // �̹��� ���� �� �ִϸ��̼� �� ���̵� �� ����
        RhythmImage_2.rectTransform.anchoredPosition = Vector2.zero;
        RhythmImage_2.color = new Color(1f, 1f, 1f, 0.2f); // �ʱ� ���İ� 0, ���������� ����
        RhythmImage_2.rectTransform.DOAnchorPosX(-moveDistance, iconSpeed).SetEase(Ease.Linear);
        RhythmImage_2.DOFade(1f, iconFadeDuration); // ���İ� ������ 1�� ����

        StartCoroutine(DestroyAfterDelay(RhythmImage_2.gameObject, iconDestroydeay, startTime)); // ���� �ð� �Ŀ� �̹��� �ı�

        // #. ������ �������� ����Ʈ�� ����
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

            // �̹����� ����Ʈ���� ����
            Image imageComponent = obj.GetComponent<Image>();
            if (imageComponent != null && rhythmImages.Contains(imageComponent))
            {
                rhythmImages.Remove(imageComponent);
            }
            // �̹����� �ı�
            Destroy(obj);
        }
    }

    public void RhythmAnimationCompleted(Image rhythmImage)
    {
        StartCoroutine(SetRhythmCorrectWithDelay(0.17f));  // @@@@@@@@@@@@@ 0.2�� ���ȸ� ������ �Ű� ��
        //Debug.Log("RhythmImage_1 �ִϸ��̼� �Ϸ� �� �Լ� ȣ��");
        // rhythmImage�� �ִϸ��̼��� �Ϸ�� �̹����Դϴ�.
    }

    private IEnumerator SetRhythmCorrectWithDelay(float delay)
    {
        rhythmCorrect = true; // rhythmCorrect�� true�� ����
        yield return new WaitForSeconds(delay); // ������ �ð���ŭ ���
        rhythmCorrect = false; // ������ �ð� �Ŀ� rhythmCorrect�� �ٽ� false�� ����
    }


    public void ActivateImage(int imageIndex)
    {
        for (int i = 0; i < Pins.Length; i++) // UI �󿡼� � ���⸦ ���� �ִ��� �ٴ÷� �񷯼� �˷���
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

    public void ActivateHpImage(int hp) // hp�� ������Ʈ
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


    public void ComboBarBounceDown() // ���������� �޺����� ��ġ�� ������ �Լ�
    {
        float newHeight = (int)(comboBarImage.rectTransform.sizeDelta.y - 0.1f);
        comboBarImage.rectTransform.sizeDelta = new Vector2(comboBarImage.rectTransform.sizeDelta.x, newHeight);
    }

    public void ComboBarDown(int downNumber) // ������ ������ �� �������� ����
    {
        float newHeight = comboBarImage.rectTransform.sizeDelta.y - downNumber;
        comboBarImage.rectTransform.sizeDelta = new Vector2(comboBarImage.rectTransform.sizeDelta.x, newHeight);
    }

    public void ComboBarBounceUp() // ���� ���� �ÿ� �޺��� �������� ���̸� �ø��� �Լ�
    {
        float currentHeight = comboBarImage.rectTransform.sizeDelta.y;
        float newHeight = currentHeight + 40; // ���̸� ����

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
        rhythmImages.Clear(); // ����Ʈ ����
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
        // #. ���� �� ���� �� ǥ���� ��
        if (isReload)
        {
            cruBulletCount.text = "X";
        }
        else
        {
            // #. ���� ���� �ƴ� ���� �ִ� źâ�� ���� ǥ��
            cruBulletCount.text = bulletCount.ToString();
        }

        if (isFever)
        {
            cruBulletCount.text = "�������Ѵ��";
        }
    }
}