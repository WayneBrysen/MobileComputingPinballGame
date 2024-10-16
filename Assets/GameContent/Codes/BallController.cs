using Photon.Pun;
using UnityEngine;

public class BallController : MonoBehaviourPun, IPunObservable
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 如果不是本地玩家控制的小球，禁用物理引擎控制
        if (!photonView.IsMine)
        {
            rb.isKinematic = true;  // 其他客户端的小球只接收同步信息，不主动更新物理状态
        }
    }

    // 实现手动同步小球状态
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 本地客户端控制的小球，将位置、速度和旋转信息发送给其他客户端
            stream.SendNext(rb.position);
            stream.SendNext(rb.velocity);
            stream.SendNext(rb.rotation);
        }
        else
        {
            // 接收其他客户端发送的小球状态，并更新
            rb.position = (Vector3)stream.ReceiveNext();
            rb.velocity = (Vector3)stream.ReceiveNext();
            rb.rotation = (Quaternion)stream.ReceiveNext();
        }
    }
}