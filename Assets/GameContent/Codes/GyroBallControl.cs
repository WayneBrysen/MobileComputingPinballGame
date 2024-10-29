using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroBallControl : MonoBehaviour
{
    public float gravityMultiplier = 9.81f;
    private Vector3 originalGravity;

    void Start()
    {
        // 启用陀螺仪
        Input.gyro.enabled = true;

        // 记录原本的全局重力值
        originalGravity = Physics.gravity;
    }

    void FixedUpdate()
    {
        if (Input.gyro.enabled)
        {
            // 获取设备倾斜的方向
            Vector3 tilt = Input.gyro.gravity;

            // 更新全局重力
            Physics.gravity = new Vector3(tilt.x, -tilt.z, tilt.y) * gravityMultiplier;
        }
    }

    void OnDestroy()
    {
        // 恢复原本的全局重力
        Physics.gravity = originalGravity;
    }
}