using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuUIControl : MonoBehaviour
{
    [Header("설정창 관련")]
    public bool isSettingPanel; // 세팅 패널이 켜져 있는지 아닌지
    public GameObject settingPanel;


    [Header("마우스 감도 관련")]
    public float mouseFloat;
    public Slider mouseSensitivitySlider;
    public float volumeBGM;
    public float volumeEffect;
    public Slider volumeBGMSlider;
    public Slider volumeEffectSlider;


    [Header("싱글톤들")]
    public GameManager gameManager;
    public PlayerInformation playerInformation;

    
    public void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerInformation = FindObjectOfType<PlayerInformation>();
        playerInformation.IsMenu = true;
        playerInformation.IsGame = false;

        mouseFloat = playerInformation.MouseSpeed; // 현재 플레이어 감도 값을 가져옴
        mouseSensitivitySlider.value = mouseFloat; // 슬라이더에도 적용

        volumeBGM = playerInformation.VolumeBGM;
        volumeBGMSlider.value = volumeBGM;

        volumeEffect = playerInformation.VolumeEffect;
        volumeEffectSlider.value = volumeEffect;
    }

    public void Play() // 게임 시작 시의 상태도 만들 수 있음
    {
        gameManager.bulletCount = 10; // 게임 첫 시작시의 총알

        gameManager.soundManager.Play();
        gameManager.GameStart();
        playerInformation.IsMenu = false;
        playerInformation.IsGame = true;

        gameManager.isReload = false;

        if(gameManager.isFever) // @@@@@@@@@@@@@@@ 임시로 피버를 끝나게 해놓은 거임
        {
            gameManager.SubFever();
        }


        gameManager.ActivateImage(playerInformation.WeponColor); // 무기 정보에 맞게 바늘 UI 업데이트
        gameManager.ActivateHpImage(3); // 체력바 활성화
        gameManager.SetVolume(); // 슬라이더 작업에서 하는 거지만 혹시 모르니까... 

        SceneManager.LoadScene("Play1");
    } 

    public void SceneTurnTiming()
    {
        SceneManager.LoadScene("Timing");
        gameManager.soundTime.Play();
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




    public void QuitGame() // 게임 종료 버튼 클릭(게임 종료)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
