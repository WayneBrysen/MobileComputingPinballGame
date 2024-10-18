using Photon.Pun;
using UnityEngine;

public class BallController : MonoBehaviourPun, IPunObservable
{
    private Rigidbody rb;

    // 用于插值非本地球的位置和旋转
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (photonView.IsMine)
        {
            // 本地玩家的球，启用物理引擎控制
            rb.isKinematic = false;
        }
        else
        {
            // 其他玩家的球，禁用物理引擎控制，由网络同步位置
            rb.isKinematic = true;

            // 初始化网络位置和旋转
            networkPosition = transform.position;
            networkRotation = transform.rotation;
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            // 对非本地球进行插值更新，平滑移动
            float lerpRate = 10f; // 插值速度，可以根据需要调整
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.fixedDeltaTime * lerpRate);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.fixedDeltaTime * lerpRate);
        }
        // 本地玩家的球的物理行为由物理引擎自动处理，无需在此添加代码
    }

    // 实现 IPunObservable 接口，用于自定义同步球的位置和旋转
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 本地玩家发送数据给其他客户端
            stream.SendNext(rb.position);
            stream.SendNext(rb.velocity);
            stream.SendNext(rb.rotation);
        }
        else
        {
            // 接收其他玩家的数据
            networkPosition = (Vector3)stream.ReceiveNext();
            Vector3 networkVelocity = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}