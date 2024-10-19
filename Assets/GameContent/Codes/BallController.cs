using Photon.Pun;
using UnityEngine;

public class BallController : MonoBehaviourPun, IPunObservable
{
    private Rigidbody rb;

    // 用于插值非本地球的位置和旋转
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    // 插值速率
    private float positionLerpRate = 10f;
    private float rotationLerpRate = 10f;

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
            networkPosition = transform.position;
            networkRotation = transform.rotation;
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            // 对非本地球进行插值更新，平滑移动和旋转
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.fixedDeltaTime * positionLerpRate);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.fixedDeltaTime * rotationLerpRate);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 本地玩家发送数据给其他客户端
            stream.SendNext(transform.position);
            stream.SendNext(rb.velocity);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // 接收其他玩家的数据
            networkPosition = (Vector3)stream.ReceiveNext();
            Vector3 networkVelocity = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();

            // 更新非本地球的速度
            if (rb.isKinematic)
            {
                rb.velocity = networkVelocity;
            }
        }
    }
}