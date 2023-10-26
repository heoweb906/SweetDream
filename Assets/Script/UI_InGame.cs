using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Net.NetworkInformation;
using Unity.VisualScripting;

public class UI_InGame : MonoBehaviour
{
    //@@@@@@@@@@@@@@@@@@@@@@@@@@
    // �ش� ���������� ���Ͱ� �ִ� ���ȿ��� ������ �� ������ �����ؾ� ��
    // or ����â�� ���� �������� ��� ���쵵�� �����ؾ��� (�ǹ�Ÿ��ó��)
    //@@@@@@@@@@@@@@@@@@@@@@@@@@


    public GameManager gameManager;
    public PlayerInformation playerInformation;
    public new Camera camera;
    public Player player;
    public StageManager stageManager;

    [Header("���� ���� �г�")]
    public GameObject gameoverPanel;

    [Header("����â �г�")]
    public bool isSettingPanel; // ���� �г��� ���� �ִ��� �ƴ���
    public GameObject settingPanel;

    public float mouseFloat;
    public Slider mouseSensitivitySlider; // �����̴� ���� �߰�

    public float volumeBGM;
    public float volumeEffect;
    public Slider volumeBGMSlider;
    public Slider volumeEffectSlider;

    public GameObject textPanel; // "���� �ʵ忡 ���Ͱ� �ֽ��ϴ�."

    public void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerInformation = FindObjectOfType<PlayerInformation>();

        mouseFloat = playerInformation.MouseSpeed; 
        mouseSensitivitySlider.value = mouseFloat; 

        volumeBGM = playerInformation.VolumeBGM;
        volumeBGMSlider.value = volumeBGM;

        volumeEffect = playerInformation.VolumeEffect;
        volumeEffectSlider.value = volumeEffect;
    }

    public void OnOffSettingPanel()
    {
        if (stageManager.MonsterCount > 0)
        {
            textPanel.SetActive(true);
            CamLock();
            Invoke("TextPanelOff",0.5f);
        }
        else if(stageManager.MonsterCount <= 0)
        {
            if (!(player.isDie))
            {
                if (!(settingPanel.activeSelf))
                {
                    player.CamUnlock();

                    isSettingPanel = true;
                    settingPanel.gameObject.SetActive(true);
                }
                else if (settingPanel.activeSelf)
                {
                    camera.SetMouseSpeed();

                    isSettingPanel = false;
                    settingPanel.gameObject.SetActive(false);

                    player.CamLock();
                }
            }
        }
    }
    public void TextPanelOff()
    {
        textPanel.SetActive(false);
    }

    public void OnOffSettingPanel_PlayerDie()
    {
        if (!(settingPanel.activeSelf))
        {
            player.CamUnlock();

            isSettingPanel = true;
            settingPanel.gameObject.SetActive(true);
        }
        else if (settingPanel.activeSelf)
        {
            camera.SetMouseSpeed();

            isSettingPanel = false;
            settingPanel.gameObject.SetActive(false);

            player.CamLock();
        }
    }

    public void OnOffGameoverPanel()
    {
        player.CamUnlock();
        gameoverPanel.gameObject.SetActive(true);
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
        player.SetPlayerSound();
    }



    public void GoMenu()
    {
        UnlockCursor(); // Ŀ�� �� ����

        PlayerPrefs.SetInt("PlayerHp", 4);

        SceneManager.LoadScene("Loading_Lobby"); // "YourSceneName"�� �̵��ϰ��� �ϴ� ���� �̸����� �ٲ��ּ���.
        gameManager.soundManager.Stop();
        playerInformation.IsMenu = true;
        playerInformation.IsGame = false;
        gameManager.iconOn = false;
    }
    private void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void CamLock() // ���콺 Ŀ���� ����� �Լ�
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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
