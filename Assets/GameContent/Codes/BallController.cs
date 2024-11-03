using Photon.Pun;
using UnityEngine;

public class BallController : MonoBehaviourPun, IPunObservable
{
    private Rigidbody rb;

    private Vector3 networkPosition;
    private Quaternion networkRotation;

    private float positionLerpRate = 10f;
    private float rotationLerpRate = 10f;

    private Vector3 initialPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (photonView.IsMine)
        {
            rb.isKinematic = false;
            initialPosition = transform.position;
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

    void OnTriggerEnter(Collider other)
    {
        HandleBoundaryCollision(other);
    }

    void OnCollisionEnter(Collision collision)
    {
        HandleBoundaryCollision(collision.collider);
    }

    void HandleBoundaryCollision(Collider other)
    {
        if (other.CompareTag("Boundary"))
        {
            Debug.Log("Ball collided with boundary.");

            if (photonView.IsMine)
            {
                photonView.RPC("ResetBallPosition", RpcTarget.AllBuffered, initialPosition);
            }
        }
    }

    [PunRPC]
    void ResetBallPosition(Vector3 resetPosition)
    {
        transform.position = resetPosition;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Debug.Log("Ball position has been reset to: " + resetPosition);
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

    public void SetInitialPosition(Vector3 position)
    {
        if (photonView.IsMine)
        {
            initialPosition = position;
        }
    }
}