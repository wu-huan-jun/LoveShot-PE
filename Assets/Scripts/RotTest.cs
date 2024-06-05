using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class RotTest : MonoBehaviour
{
    void Start()
    {
        // ����������
        Input.gyro.enabled = true;
    }

    void Update()
    {
        // ��ȡ�����ǵ���Ԫ��
        Quaternion gyroAttitude = Input.gyro.attitude;

        // ����Ԫ�����豸����ϵת��ΪUnity����ϵ
        Quaternion deviceRotation = new Quaternion(gyroAttitude.x, gyroAttitude.y, -gyroAttitude.z, -gyroAttitude.w);

        // Ӧ����ת
        transform.rotation = Quaternion.Euler(90, 0, 0) * deviceRotation;
    }
}

