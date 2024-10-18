using Photon.Pun;
using UnityEngine;

public class BallController : MonoBehaviourPun, IPunObservable
{
    private Rigidbody rb;

    // ���ڲ�ֵ�Ǳ������λ�ú���ת
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (photonView.IsMine)
        {
            // ������ҵ������������������
            rb.isKinematic = false;
        }
        else
        {
            // ������ҵ��򣬽�������������ƣ�������ͬ��λ��
            rb.isKinematic = true;

            // ��ʼ������λ�ú���ת
            networkPosition = transform.position;
            networkRotation = transform.rotation;
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            // �ԷǱ�������в�ֵ���£�ƽ���ƶ�
            float lerpRate = 10f; // ��ֵ�ٶȣ����Ը�����Ҫ����
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.fixedDeltaTime * lerpRate);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.fixedDeltaTime * lerpRate);
        }
        // ������ҵ����������Ϊ�����������Զ����������ڴ���Ӵ���
    }

    // ʵ�� IPunObservable �ӿڣ������Զ���ͬ�����λ�ú���ת
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ������ҷ������ݸ������ͻ���
            stream.SendNext(rb.position);
            stream.SendNext(rb.velocity);
            stream.SendNext(rb.rotation);
        }
        else
        {
            // ����������ҵ�����
            networkPosition = (Vector3)stream.ReceiveNext();
            Vector3 networkVelocity = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}