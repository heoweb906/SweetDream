using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [Header("�̱����")]
    public GameManager gameManager;
    public PlayerInformation playerInformation;

    public void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerInformation = FindObjectOfType<PlayerInformation>();

        PlayerPrefs.SetInt("PlayerHp", 4);
    }
}
