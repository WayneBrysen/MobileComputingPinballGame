using Photon.Pun;
using UnityEngine;

public class GyroCameraControl : MonoBehaviourPunCallbacks
{
    private Camera playerCamera;

    void Start()
    {
        if (photonView.Owner != PhotonNetwork.LocalPlayer)
        {
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            Debug.Log("transfer camera to the localPlayer");
        }

        playerCamera = GetComponent<Camera>();

        if (photonView.IsMine)
        {
            Input.gyro.enabled = true;
        }
    }

    void Update()
    {
        if (photonView.IsMine && Input.gyro.enabled)
        {
            Quaternion gyroRotation = Input.gyro.attitude;
            gyroRotation = new Quaternion(-gyroRotation.x, -gyroRotation.y, gyroRotation.z, gyroRotation.w);

            playerCamera.transform.localRotation = gyroRotation;
        }
    }
}