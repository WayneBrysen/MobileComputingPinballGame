using Photon.Pun;
using UnityEngine;

public class BallController : MonoBehaviourPun, IPunObservable
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // ������Ǳ�����ҿ��Ƶ�С�򣬽��������������
        if (!photonView.IsMine)
        {
            rb.isKinematic = true;  // �����ͻ��˵�С��ֻ����ͬ����Ϣ����������������״̬
        }
    }

    // ʵ���ֶ�ͬ��С��״̬
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ���ؿͻ��˿��Ƶ�С�򣬽�λ�á��ٶȺ���ת��Ϣ���͸������ͻ���
            stream.SendNext(rb.position);
            stream.SendNext(rb.velocity);
            stream.SendNext(rb.rotation);
        }
        else
        {
            // ���������ͻ��˷��͵�С��״̬��������
            rb.position = (Vector3)stream.ReceiveNext();
            rb.velocity = (Vector3)stream.ReceiveNext();
            rb.rotation = (Quaternion)stream.ReceiveNext();
        }
    }
}