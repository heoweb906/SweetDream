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
    // 해당 스테이지에 몬스터가 있는 동안에는 설정할 수 없도록 수정해야 함
    // or 설정창을 열면 판정선을 모두 지우도록 수정해야함 (피버타임처럼)
    //@@@@@@@@@@@@@@@@@@@@@@@@@@


    public GameManager gameManager;
    public PlayerInformation playerInformation;
    public new Camera camera;
    public Player player;

    [Header("게임 오버 패널")]
    public GameObject gameoverPanel;

    [Header("설정창 패널")]
    public bool isSettingPanel; // 세팅 패널이 켜져 있는지 아닌지
    public GameObject settingPanel;

    public float mouseFloat;
    public Slider mouseSensitivitySlider; // 슬라이더 변수 추가

    public float volumeBGM;
    public float volumeEffect;
    public Slider volumeBGMSlider;
    public Slider volumeEffectSlider;

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

    public void OnMouseSensitivityChanged(float value) // 마우스 감도 조절 슬라이더 함수
    {
        // 소수점 2자리까지 반올림하여 mouseFloat에 할당
        mouseFloat = Mathf.Round(value * 100) / 100;
        playerInformation.MouseSpeed = mouseFloat;
    }

    public void OnBGM_SoundSensitivityChanged(float value) // 배경음악 크기 조절 슬라이더 함수
    {
        volumeBGM = Mathf.Round(value * 100) / 100;
        playerInformation.VolumeBGM = volumeBGM;
        gameManager.SetVolume();
    }

    public void OnEffect_SoundSensitivityChanged(float value) // 효과음 크기 조절 슬라이더 함수
    {
        volumeEffect = Mathf.Round(value * 100) / 100;
        playerInformation.VolumeEffect = volumeEffect;
        gameManager.SetVolume();
        player.SetPlayerSound();
    }



    public void GoMenu()
    {
        UnlockCursor(); // 커서 락 해제

        SceneManager.LoadScene("Menu"); // "YourSceneName"은 이동하고자 하는 씬의 이름으로 바꿔주세요.
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
}
