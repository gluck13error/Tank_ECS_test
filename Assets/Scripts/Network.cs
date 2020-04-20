using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Network : MonoBehaviourPunCallbacks
{
    public TextMeshPro text;

    public GameObject[] DestroyForStartGame;
    private Tween textTween;

    #region UNITY

    public void Awake()
    {
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.ConnectUsingSettings();
        textTween = text.transform.DOScale(1.1f, 0.4f).SetLoops(-1, LoopType.Yoyo);


    }

    #endregion

    public void Connect()
    {

        // #Critical, we must first and foremost connect to Photon Online Server.
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.ConnectUsingSettings();

    }

    #region PUN CALLBACKS

    public override void OnConnectedToMaster()
    {
        Debug.Log($"Network OnConnectedToMaster");
        PhotonNetwork.JoinRandomRoom();

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Network OnJoinRandomFailed");
        string roomName = "Room " + Random.Range(1000, 10000);

        RoomOptions options = new RoomOptions { MaxPlayers = 2 };

        PhotonNetwork.CreateRoom(roomName, options, null);
        text.text = "Expect the enemy";
    
    }

    public override void OnJoinedRoom()
    {
      
        if (PhotonNetwork.PlayerList.Length ==2)
        {
            StartGame();
        }
       

    }

    public override void OnLeftRoom()
    {
        Debug.LogWarning("OnLeftRoom");

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.PlayerList.Length == 2)
        {
            StartGame();
        }

    }

    void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        textTween.Kill();
        //Startgame
        foreach (var item in DestroyForStartGame)
        {
            Destroy(item);
        }
        SceneManager.LoadScene("Level", LoadSceneMode.Additive);

    }
   
    IEnumerator SendMessage(int seconds)
    {
      
        yield return new WaitForSeconds(seconds);
        byte evCode = 1; // Custom Event 1: Used as "MoveUnitsToTargetPosition" event
        object[] content = new object[] { new Vector3(10.0f, 2.0f, 5.0f), 1, 2, 5, 10 }; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
        yield return null;


    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
     
        Debug.LogWarning("OnPlayerLeftRoom");
       
    }


    #endregion

    #region UI CALLBACKS


    #endregion

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == 1)
        {
            object[] data = (object[])photonEvent.CustomData;

            Vector3 targetPosition = (Vector3)data[0];
           // Log.text += $"\n OnEvent: {targetPosition} id:";
            for (int index = 1; index < data.Length; ++index)
            {
                int unitId = (int)data[index];
             //   Log.text += $"{unitId} ";
            }
        }
    }
}
