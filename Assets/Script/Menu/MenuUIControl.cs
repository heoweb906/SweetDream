using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuUIControl : MonoBehaviour
{
    [Header("����â ����")]
    public bool isSettingPanel; // ���� �г��� ���� �ִ��� �ƴ���
    public GameObject settingPanel;


    [Header("���콺 ���� ����")]
    public float mouseFloat;
    public Slider mouseSensitivitySlider;
    public float volumeBGM;
    public float volumeEffect;
    public Slider volumeBGMSlider;
    public Slider volumeEffectSlider;


    [Header("�̱����")]
    public GameManager gameManager;
    public PlayerInformation playerInformation;

    
    public void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerInformation = FindObjectOfType<PlayerInformation>();

        playerInformation.IsMenu = true;
        playerInformation.IsGame = false;

        mouseFloat = playerInformation.MouseSpeed; // ���� �÷��̾� ���� ���� ������
        mouseSensitivitySlider.value = mouseFloat; // �����̴����� ����

        volumeBGM = playerInformation.VolumeBGM;
        volumeBGMSlider.value = volumeBGM;

        volumeEffect = playerInformation.VolumeEffect;
        volumeEffectSlider.value = volumeEffect;
    }

    public void Play() // ���� ���� ���� ���µ� ���� �� ����
    {
        gameManager.bulletCount = 10; // ���� ù ���۽��� �Ѿ�

        gameManager.soundManager.Play();
        gameManager.GameStart();
        playerInformation.IsMenu = false;
        playerInformation.IsGame = true;

        if(gameManager.isFever) // @@@@@@@@@@@@@@@ �ӽ÷� �ǹ��� ������ �س��� ����
        {
            gameManager.SubFever();
        }


        gameManager.ActivateImage(playerInformation.WeponColor); // ���� ������ �°� �ٴ� UI ������Ʈ
        gameManager.ActivateHpImage(3); // ü�¹� Ȱ��ȭ
        gameManager.SetVolume(); // �����̴� �۾����� �ϴ� ������ Ȥ�� �𸣴ϱ�... 

        SceneManager.LoadScene("Play1");
    } 

    public void SceneTurnTiming()
    {
        SceneManager.LoadScene("Timing");
        gameManager.soundTime.Play();
    }

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

    public void OnOffSettingPanel()
    {
        if(!(settingPanel.activeSelf))
        {
            isSettingPanel = true;
            settingPanel.gameObject.SetActive(true);
        }
        else if(settingPanel.activeSelf)
        {
            isSettingPanel = false ;
            settingPanel.gameObject.SetActive(false);
        }
    }




    public void QuitGame() // ���� ���� ��ư Ŭ��(���� ����)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
