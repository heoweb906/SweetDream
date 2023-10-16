using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Potal_1 : MonoBehaviour
{
    [Header("�������� �Ŵ���")]
    public StageManager stageManager;

    [Header("���� ������")]
    public Animator animDoor;
    public BoxCollider colliderBox;
    private bool b_isDoorOpen;

    private void Update()
    {
        if (stageManager.MonsterCount == 0 && !b_isDoorOpen)
        {
            MoveWall();
        }
    }

    public void MoveWall()
    {
        Debug.Log("���� ���������� ���� ���� ���Ƚ��ϴ�.");
        b_isDoorOpen = true;

        colliderBox.enabled = true;
        animDoor.SetTrigger("doDoor");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // "Player" �±׸� ���� ������Ʈ�� �浹���� ��
        {
            Debug.Log("���� ���������� �̵��մϴ�.");
            SceneManager.LoadScene("Play2"); // Play �� 2�� 
        }
    }
}