using Photon.Pun;
using UnityEngine;


public class PlayerManager : MonoBehaviourPunCallbacks
{
    private GameObject southPlayerCamera;
    private GameObject northPlayerCamera;
    private GameObject southController;
    private GameObject northController;

    private Vector3 p1BallPosition;
    private Vector3 p2BallPosition;

    private Vector3 p1PlungerPosition;
    private Vector3 p2PlungerPosition;

    private GameObject playerBall;

    void Start()
    {
        // automatically find out camera
        southPlayerCamera = GameObject.FindWithTag("SouthCamera");
        northPlayerCamera = GameObject.FindWithTag("NorthCamera");

        if (southPlayerCamera == null)
        {
            Debug.LogWarning("South Camera not found!");
        }

        if (northPlayerCamera == null)
        {
            Debug.LogWarning("North Camera not found!");
        }

        // automatically find out controller
        southController = GameObject.Find("southController");
        northController = GameObject.Find("northController");

        GameObject p1PositionObj = GameObject.Find("P1BallPosition");
        GameObject p2PositionObj = GameObject.Find("P2BallPosition");

        GameObject p1PlungerPositionObj = GameObject.Find("p1PlungerPosition");
        GameObject p2PlungerPositionObj = GameObject.Find("p2PlungerPosition");


        if (p1PlungerPositionObj != null)
        {
            p1PlungerPosition = p1PlungerPositionObj.transform.position;
        }

        if (p2PlungerPositionObj != null)
        {
            p2PlungerPosition = p2PlungerPositionObj.transform.position;
        }



        if (p1PositionObj != null)
        {
            p1BallPosition = p1PositionObj.transform.position;
        }

        if (p2PositionObj != null)
        {
            p2BallPosition = p2PositionObj.transform.position;
        }

        Debug.Log("South Camera: " + (southPlayerCamera != null ? "Found" : "Not Found"));
        Debug.Log("North Camera: " + (northPlayerCamera != null ? "Found" : "Not Found"));
        Debug.Log("South Controller: " + (southController != null ? "Found" : "Not Found"));
        Debug.Log("North Controller: " + (northController != null ? "Found" : "Not Found"));

        if (photonView.IsMine)
        {
            Debug.Log("This playermanager is mine");
        }
        else
        {
            Debug.Log("This client is others");
        }

        if (PhotonNetwork.InRoom)
        {
            OnJoinedRoom();
            Debug.Log("already in the room, call OnJoinedRoom()");
        }
    }

    public override void OnJoinedRoom()
    {
        if (photonView.IsMine)
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                SetPlayerPosition(true);
            }
            else
            {
                SetPlayerPosition(false);
                TransferFlipperOwnership(northController);
            }
            SpawnPlayerBall();
            SpawnPlayerplunger();
        }
    }


    void SpawnPlayerplunger()
    {
        Vector3 spawnPosition = PhotonNetwork.LocalPlayer.IsMasterClient ? p1PlungerPosition : p2PlungerPosition;

        GameObject playerplunger = PhotonNetwork.Instantiate("plunger", spawnPosition, Quaternion.identity);

        PlungerLauncher plungerLauncher = playerplunger.GetComponent<PlungerLauncher>();

        plungerLauncher.playerBall = this.playerBall;


        PhotonView plungerView = playerplunger.GetComponent<PhotonView>();

        if (!plungerView.IsMine)
        {
            playerplunger.SetActive(false);
        }

        Debug.Log("generate plunger£º" + "plunger" + " at location: " + spawnPosition);
    }

    void SpawnPlayerBall()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("selectedBallPrefab", out object selectedBallPrefabNameObj))
        {
            string selectedBallPrefabName = selectedBallPrefabNameObj as string;

            if (string.IsNullOrEmpty(selectedBallPrefabName))
            {
                Debug.LogError("cannot find the ball from the player");
                return;
            }

            Vector3 spawnPosition = PhotonNetwork.LocalPlayer.IsMasterClient ? p1BallPosition : p2BallPosition;

            // generate ball from photonnetwork
            playerBall = PhotonNetwork.Instantiate(selectedBallPrefabName, spawnPosition, Quaternion.identity);

            Debug.Log("generate plunger£º" + "plunger" + " at location: " + spawnPosition);
        }
        else
        {
            Debug.LogError("cannot find the ball from the player");
        }
    }

    public GameObject GetPlayerBall()
    {
        return playerBall;
    }

    void TransferFlipperOwnership(GameObject controller)
    {
        Debug.Log("start transforming ownership");

        var photonViews = controller.GetComponentsInChildren<PhotonView>();
        foreach (var view in photonViews)
        {
            if (PhotonNetwork.LocalPlayer != view.Owner)
            {
                view.TransferOwnership(PhotonNetwork.LocalPlayer);
                Debug.Log("ownership: " + view.ViewID + " transfered to " + PhotonNetwork.LocalPlayer.NickName);
            }
        }
    }


    void SetPlayerPosition(bool isSouthSide)
    {
        if (isSouthSide)
        {
            Debug.Log("activate south camera, diactivate north");
            if (southPlayerCamera != null) southPlayerCamera.SetActive(true);
            if (northPlayerCamera != null) northPlayerCamera.SetActive(false);

            SetFlippersControl(southController, true);
            SetFlippersControl(northController, false);
        }
        else
        {
            Debug.Log("activate north camera, diactivate south");
            if (northPlayerCamera != null) northPlayerCamera.SetActive(true);
            if (southPlayerCamera != null) southPlayerCamera.SetActive(false);

            SetFlippersControl(northController, true);
            SetFlippersControl(southController, false);
        }
    }

    void SetFlippersControl(GameObject controller, bool isEnabled)
    {
        var leftFlipper = controller.transform.Find("LeftFlipper").GetComponent<LeftFlipperControl>();
        var rightFlipper = controller.transform.Find("RightFlipper").GetComponent<RightFlipperControl>();
        var gameManager = controller.transform.Find("GameManager").GetComponent<FlipperTouchController>();

        if (leftFlipper != null)
        {
            leftFlipper.enabled = isEnabled;
            Debug.Log("set leftFlipper");

        }

        if (rightFlipper != null)
        {
            rightFlipper.enabled = isEnabled;
            Debug.Log("set rightFlipper");

        }

        if (gameManager != null)
        {
            gameManager.enabled = isEnabled;
            Debug.Log("set GameManager");

        }

    }
}