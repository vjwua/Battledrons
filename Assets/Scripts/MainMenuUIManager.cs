using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class MainMenuUIManager : MonoBehaviourPunCallbacks
{

    public GameObject lobbyCreationMenuUI;
    public GameObject lobbyUI; 
    public InputField playerNameInput; 
    public InputField roomNameInput;
    public Text playerListText; 
    public Text roomText;
    public GameObject resultGameUI;
    public Text resultText;
    

    public List<string> playerNamesList;

    private void Start()
    {

        
    }

    public void StartButton()
    {
        
        lobbyCreationMenuUI.SetActive(true);
    }

    public void CreateRoomButton()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogError("Error connected PhotonNetwork .");
            return;
        }
        PhotonNetwork.NickName = playerNameInput.text;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Create room - " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("connected to room - " + PhotonNetwork.CurrentRoom.Name);
        lobbyUI.SetActive(true);
        UpdateInfoRoom();  
    }


    public void OnJoinRoomButtonClicked()
    {
        PhotonNetwork.NickName = playerNameInput.text;
        PhotonNetwork.JoinRoom(roomNameInput.text);
    }


    public void UpdateInfoRoom()
    {
        playerListText.text = "";

        playerNamesList.Clear(); 
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerNamesList.Add(player.NickName); 
        }
        
        foreach (string playerName in playerNamesList)
        {
            playerListText.text += playerName + "\n";
        }

        roomText.text = "";

        roomText.text += "You are in the room - " + PhotonNetwork.CurrentRoom.Name;

    }

    public void LeaveButton()
    {
        PhotonNetwork.LeaveRoom();
        lobbyUI.SetActive(false);
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player entered room: " + newPlayer.NickName);
        UpdateInfoRoom();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Player left room: " + otherPlayer.NickName);
        UpdateInfoRoom();
    }



    [PunRPC]
    public void LoadGameScene()
    {
        resultGameUI.SetActive(true);
        PhotonNetwork.LoadLevel("SampleScene");
    }

    public void OnStartGameButtonClicked()
    {
        photonView.RPC("LoadGameScene", RpcTarget.All);
    }

    public void GoToRoom()
    {
        resultGameUI.SetActive(false);
    }
}
