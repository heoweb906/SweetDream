using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Camera : MonoBehaviour
{
    public PlayerInformation playerInformation;
    public UI_InGame inGame_UI;
    public Player player;

    public Transform target; // �÷��̾��� Transform�� ������ ����
    public Vector3 offset;   // ī�޶�� �÷��̾� ������ �Ÿ�
    public float turnSpeed; // ���콺 ȸ�� �ӵ�    
    private float xRotate = 0.0f; // ���� ����� X�� ȸ������ ���� ���� ( ī�޶� �� �Ʒ� ���� )


    private void Start()
    {
        inGame_UI = FindAnyObjectByType<UI_InGame>();
        player = FindAnyObjectByType<Player>();
        SetMouseSpeed();
    }

    public void SetMouseSpeed()
    {
        playerInformation = FindObjectOfType<PlayerInformation>();
        turnSpeed = playerInformation.MouseSpeed * 4f; // ���콺 ���� ���� 0~1�� �����ϰ� ����, ���� ����ÿ��� 4�� ������
    }


    void Update()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            transform.position = desiredPosition;
        }

        if(!(inGame_UI.isSettingPanel) && !(player.isDie))
        {
            // �¿�� ������ ���콺�� �̵��� * �ӵ��� ���� ī�޶� �¿�� ȸ���� �� ���
            float yRotateSize = Input.GetAxis("Mouse X") * turnSpeed;
            // ���� y�� ȸ������ ���� ���ο� ȸ������ ���
            float yRotate = transform.eulerAngles.y + yRotateSize;

            // ���Ʒ��� ������ ���콺�� �̵��� * �ӵ��� ���� ī�޶� ȸ���� �� ���(�ϴ�, �ٴ��� �ٶ󺸴� ����)
            float xRotateSize = -Input.GetAxis("Mouse Y") * turnSpeed;
            // ���Ʒ� ȸ������ ���������� -45�� ~ 80���� ���� (-45:�ϴù���, 80:�ٴڹ���)
            xRotate = Mathf.Clamp(xRotate + xRotateSize, -80, 80);

            // ī�޶� ȸ������ ī�޶� �ݿ�(X, Y�ุ ȸ��)
            transform.eulerAngles = new Vector3(xRotate, yRotate, 0);

            // �÷��̾� ������Ʈ�� ȸ���� ī�޶�� ���� ���� (ī�޶� ȸ������ �÷��̾ ����)
            target.rotation = Quaternion.Euler(0, yRotate, 0);
        }
    }
}
