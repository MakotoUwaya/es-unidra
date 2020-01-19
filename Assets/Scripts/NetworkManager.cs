using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using MonobitEngine;

public class NetworkManager : MonobitEngine.MonoBehaviour
{
    RoomSettings commonRoomSettings;


    string roomName = "YourRoom";
    bool isGameStarted = false;

    private void Start()
    {
        if (!MonobitNetwork.inLobby)
        {
            Debug.LogError("Not in lobby.");
            return;
        }

        this.commonRoomSettings = new RoomSettings
        {
            // 公開か非公開かを設定できる
            isVisible = true,
            // 入室を許可するかを設定できる
            isOpen = true,
            // 作成するルームの最大人数を4人に設定する
            maxPlayers = 4,
            // パスワードとして利用するためのカスタムパラメータを作成
            roomParameters = new Hashtable() { { "password", "empty" }, },
            // 上記だけだと、ルーム外からカスタムパラメータを扱うことが出来ないので、扱えるように設定する。
            lobbyParameters = new string[] { "password" }
        };
    }

    void OnGUI()
    {
        if (this.isGameStarted)
        {
            return;
        }

        // 高さ480の(0,0)中心を基準にする.
        var scale = Screen.height / 480.0f;
        GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0), Quaternion.identity, new Vector3(scale, scale, 1.0f));

        GUI.Window(0, new Rect(-200, -200, 400, 400), this.NetworkSettingWindow, "Network Setting");
    }

    Vector2 scrollPosition;

    void NetworkSettingWindow(int windowID)
    {
        // ゲームサーバー名の設定.
        GUILayout.BeginHorizontal();
        GUILayout.Label("Create Room Name: ");
        this.roomName = GUILayout.TextField(this.roomName, 32);
        GUILayout.EndHorizontal();

        // ゲームサーバーを起動する.
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (MonobitNetwork.inRoom)
        {
            if (MonobitNetwork.isHost)
            {
                if (GUILayout.Button("Start game"))
                {
                    Debug.Log("Start game");
                    var playerCustomParams = new Hashtable
                    {
                        ["isGameStarted"] = true
                    };
                    MonobitNetwork.SetPlayerCustomParameters(playerCustomParams);
                    this.isGameStarted = true;
                }
            }
            else
            {
                if (GUILayout.Button("Leave room"))
                {
                    Debug.Log("Leave room");
                    MonobitNetwork.LeaveRoom();
                }
            }
        }
        else
        {
            if (GUILayout.Button("Create room"))
            {
                this.CreateRoom();
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        // ゲームサーバーリスト.
        GUILayout.BeginHorizontal();
        GUILayout.Label("Room List (Click To Join):");
        GUILayout.EndHorizontal();

        this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, GUILayout.Width(380), GUILayout.Height(200));
        var rooms = MonobitNetwork.GetRoomData();
        if (rooms.Length > 0)
        {
            foreach (var room in rooms)
            {
                if (GUILayout.Button($"{room.name}({room.playerCount}/{room.maxPlayers})", GUI.skin.box, GUILayout.Width(360)))
                {
                    if (!room.customParameters["password"].Equals("empty"))
                    {
                        Debug.Log($"Room:[{room.name}] is private room.");
                    }
                    MonobitNetwork.JoinRoom(room.name);
                }
            }
        }
        else
        {
            GUILayout.Label("No rooms");
        }
        GUILayout.EndScrollView();

        if (GUILayout.Button("Back title"))
        {
            this.OnClickBack();
        }

        // ステータスの表示.
        var isHostDisplay = MonobitNetwork.isHost ? "(Host)" : "";
        GUILayout.Label(
            MonobitNetwork.inRoom
            ? $"Room: {MonobitNetwork.room.name} {MonobitNetwork.room.playerCount}/{MonobitNetwork.room.maxPlayers} players {isHostDisplay}"
            : $"Rooms count: {MonobitNetwork.roomCountInLobby} InLobby Players count: {MonobitNetwork.clientCountInLobby}"
        );
    }

    /// <summary>MUNサーバーとの接続を切った際に呼ばれるコールバック</summary>
    void OnDisconnectedFromServer()
    {
        this.isGameStarted = false;

        // サーバーから切断された後にタイトル画面に戻るようにする
        SceneManager.LoadScene("TitleScene");
    }

    /// <summary>タイトル画面へ戻るボタンを押した際に呼ばれる</summary>
    void OnClickBack()
    {
        // サーバーから切断する
        MonobitNetwork.DisconnectServer();
    }

    /// <summary>ルームを作る</summary>
    void CreateRoom()
    {
        MonobitNetwork.CreateRoom(this.roomName, this.commonRoomSettings, MonobitNetwork.lobby);
    }

    /// <summary>ルーム作成成功時に呼ばれる</summary>
    void OnCreatedRoom()
    {
        Debug.Log("Created game room");
    }

    /// <summary>ルーム入室成功時に呼ばれる</summary>
    private void OnJoinedRoom()
    {
        Debug.Log("Joined room");
        this.isGameStarted = false;
    }

    /// <summary>ルームの作成に失敗した際に呼ばれるコールバック</summary>
    /// <param name="codeAndMsg">エラーコード</param>
    void OnCreateRoomFailed(object[] codeAndMsg)
    {
        Debug.Log($"OnCreateRoomFailed : errorCode = {codeAndMsg[0]}, message = {codeAndMsg[1]}");
    }

    /// <summary>他プレイヤーが入室してきた際に呼ばれるコールバック</summary>
    /// <param name="newPlayer">入室してきたプレイヤーの情報</param>
    private void OnOtherPlayerConnected(MonobitPlayer newPlayer)
    {
        Debug.Log($"Connected new player: {newPlayer.name}");
    }

    /// <summary>他プレイヤーが退室した際に呼ばれるコールバック</summary>
    /// <param name="otherPlayer">退室したプレイヤーの情報</param>
    private void OnOtherPlayerDisconnected(MonobitPlayer otherPlayer)
    {
        Debug.Log($"Disconected player: {otherPlayer.name}");
    }

    /// <summary>MonobitNetwork.SetPlayerCustomParametersによりプレイヤーカスタムパラメータが変更された際に呼ばれるコールバック</summary>
    /// <param name="playerAndUpdatedParameters">プレイヤーカスタムパラメータに変更があったMonobitPlayer及びHashtable</param>
    public void OnMonobitPlayerParametersChanged(object[] playerAndUpdatedParameters)
    {
        var monobitPlayer = (MonobitPlayer)playerAndUpdatedParameters[0];
        var playerParams = (Hashtable)playerAndUpdatedParameters[1];
        Debug.Log($"Start game by: {monobitPlayer.name}");
        if ((bool)playerParams["isGameStarted"] == false)
        {
            return;
        }
        this.isGameStarted = true;

    }
}
