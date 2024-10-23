using Photon.Pun;
using UnityEngine;

public class BallController : MonoBehaviourPun, IPunObservable
{
    private Rigidbody rb;

    // ���ڲ�ֵ�Ǳ������λ�ú���ת
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    // ��ֵ����
    private float positionLerpRate = 10f;
    private float rotationLerpRate = 10f;

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
            networkPosition = transform.position;
            networkRotation = transform.rotation;
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            // �ԷǱ�������в�ֵ���£�ƽ���ƶ�����ת
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.fixedDeltaTime * positionLerpRate);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.fixedDeltaTime * rotationLerpRate);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ������ҷ������ݸ������ͻ���
            stream.SendNext(transform.position);
            stream.SendNext(rb.velocity);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // ����������ҵ�����
            networkPosition = (Vector3)stream.ReceiveNext();
            Vector3 networkVelocity = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();

            // ���·Ǳ�������ٶ�
            if (rb.isKinematic)
            {
                rb.velocity = networkVelocity;
            }
        }
    }
}