using UnityEngine;
using UnityEngine.SceneManagement;
using MonobitEngine;

public class TitleScenenCtrl : MonobitEngine.MonoBehaviour
{
    // タイトル画面テクスチャ
    public Texture2D bgTexture;
    string playerName = "";

    void OnGUI()
    {
        // スタイルを準備.
        var buttonStyle = new GUIStyle(GUI.skin.button);

        // 解像度対応
        GUI.matrix = Matrix4x4.TRS(
            Vector3.zero,
            Quaternion.identity,
            new Vector3(Screen.width / 854.0f, Screen.height / 480.0f, 1.0f));
        // タイトル画面テクスチャ表示
        GUI.DrawTexture(new Rect(0.0f, 0.0f, 854.0f, 480.0f), this.bgTexture);

        // プレイヤー名の設定.
        GUI.Label(new Rect(327, 260, 80, 27), "Player Name: ");
        this.playerName = GUI.TextField(new Rect(407, 260, 120, 27), this.playerName, 32);
        if (GUI.Button(new Rect(327, 290, 200, 54), "Start", buttonStyle))
        {
            this.OnClickStartButton();
        }
        if (GUI.Button(new Rect(327, 360, 200, 54), "Exit", buttonStyle))
        {
            this.OnClickExitButton();
        }
    }

    void OnClickStartButton()
    {// 自動でデフォルトのロビーへ入室する
        MonobitNetwork.autoJoinLobby = true;
        var name = this.playerName.Trim();
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("User name is empty");
            return;
        }

        MonobitNetwork.playerName = name;
        MonobitNetwork.ConnectServer("ESUNIDRA_v_0_0_2");

        // TODO: ゲーム開始処理は別途追加する
        // SceneManager.LoadScene("GameScene");
    }

    void OnClickExitButton()
    {
        if (MonobitNetwork.inLobby)
        {
            MonobitNetwork.DisconnectServer();
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
        UnityEngine.Application.Quit();
#endif
    }

    // MUNサーバーとの接続に成功した際に呼ばれる接続コールバック
    public void OnConnectedToMonobit()
    {
        Debug.Log("接続しました");
    }

    // サーバから切断したときに呼ばれる接続コールバック
    public void OnDisconnectedFromServer()
    {
        Debug.Log("切断しました");
    }

    // MUNサーバーとの接続に失敗した際に呼ばれる接続コールバック
    public void OnConnectToServerFailed(MonobitEngine.DisconnectCause cause)
    {
        Debug.Log("接続に失敗しました:" + cause.ToString());
    }

    // MUNサーバーとの接続後に何らかの原因で切断されたときに呼ばれる接続コールバック
    public void OnConnectionFail(MonobitEngine.DisconnectCause cause)
    {
        Debug.Log("サーバーとの接続後に何らかの原因で切断されました:" + cause.ToString());
    }

    // サーバーへの接続数が上限だった際に呼ばれる接続コールバック
    public void OnMonobitMaxConnectionReached()
    {
        Debug.Log("サーバーに接続しているクライアント数が上限に達しています");
    }

    /// <summary>ロビーへ入室した際に呼ばれるコールバック</summary>
    private void OnJoinedLobby()
    {
        // ロビー入室に成功したのでロビー画面へ切り替える
        SceneManager.LoadScene("GameScene");
    }
}
