using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class RotTest : MonoBehaviour
{
    void Start()
    {
        // 启用陀螺仪
        Input.gyro.enabled = true;
    }

    void Update()
    {
        // 获取陀螺仪的四元数
        Quaternion gyroAttitude = Input.gyro.attitude;

        // 将四元数从设备坐标系转换为Unity坐标系
        Quaternion deviceRotation = new Quaternion(gyroAttitude.x, gyroAttitude.y, -gyroAttitude.z, -gyroAttitude.w);

        // 应用旋转
        transform.rotation = Quaternion.Euler(90, 0, 0) * deviceRotation;
    }
}

