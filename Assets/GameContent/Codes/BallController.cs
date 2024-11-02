using Photon.Pun;
using UnityEngine;

public class BallController : MonoBehaviourPun, IPunObservable
{
    private Rigidbody rb;

    private Vector3 networkPosition;
    private Quaternion networkRotation;

    private float positionLerpRate = 10f;
    private float rotationLerpRate = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (photonView.IsMine)
        {
            rb.isKinematic = false;
        }
        else
        {
            rb.isKinematic = true;
            networkPosition = transform.position;
            networkRotation = transform.rotation;
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.fixedDeltaTime * positionLerpRate);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.fixedDeltaTime * rotationLerpRate);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(rb.velocity);
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            Vector3 networkVelocity = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();

            if (rb.isKinematic)
            {
                rb.velocity = networkVelocity;
            }
        }
    }
}