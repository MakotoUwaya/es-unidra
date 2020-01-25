using UnityEngine;
using UnityEngine.SceneManagement;
using MonobitEngine;

public class GameRuleCtrl : MonobitEngine.MonoBehaviour
{
    GameObject player;
    public Transform startPoint;
    public FollowCamera followCamera;

    // 残り時間
    public float timeRemaining = 5.0f * 60.0f;
    // ゲームオーバーフラグ
    public bool gameOver = false;
    // ゲームクリア
    public bool gameClear = false;
    public float sceneChangeTime = 3.0f;

    public AudioClip clearSeClip;
    AudioSource clearSeAudio;

    MonobitView gameRuleMonobitView;

    private void Awake()
    {
        // すべての親オブジェクトに対して MonobitView コンポーネントを検索する
        if (this.GetComponentInParent<MonobitView>() != null)
        {
            this.gameRuleMonobitView = this.GetComponentInParent<MonobitView>();
        }
        // 親オブジェクトに存在しない場合、すべての子オブジェクトに対して MonobitView コンポーネントを検索する
        else if (this.GetComponentInChildren<MonobitView>() != null)
        {
            this.gameRuleMonobitView = this.GetComponentInChildren<MonobitView>();
        }
        // 親子オブジェクトに存在しない場合、自身のオブジェクトに対して MonobitView コンポーネントを検索して設定する
        else
        {
            this.gameRuleMonobitView = this.GetComponent<MonobitView>();
        }
    }

    private void Start()
    {
        this.clearSeAudio = this.gameObject.AddComponent<AudioSource>();
        this.clearSeAudio.clip = this.clearSeClip;
        this.clearSeAudio.loop = false;
    }

    void Update()
    {
        if (this.player == null && MonobitNetwork.inRoom)
        {
            var shiftVector = new Vector3(MonobitNetwork.room.playerCount * 1.5f, 0.0f);
            this.player = MonobitNetwork.Instantiate("Player", this.startPoint.position + shiftVector, this.startPoint.rotation, 0);
            this.followCamera.SetTarget(this.player.transform);
        }

        if (this.gameClear || this.gameOver)
        {
            this.sceneChangeTime -= Time.deltaTime;
            if (this.sceneChangeTime <= 0.0f)
            {
                this.sceneChangeTime = 3.0f;
                this.player = null;
                MonobitNetwork.LeaveRoom();
                MonobitNetwork.LeaveLobby();
                SceneManager.LoadScene("TitleScene");
            }
            return;
        }

        if (MonobitNetwork.inRoom)
        {
            this.timeRemaining -= Time.deltaTime;
            // 残り時間が無くなったらゲームオーバー
            if (this.timeRemaining <= 0.0f)
            {
                this.GameOver();
            }
        }
    }

    public void GameOver()
    {
        if (!this.gameOver && MonobitNetwork.isHost)
        {
            this.gameRuleMonobitView.RPC(nameof(this.GameOverOnNetwork), MonobitTargets.AllBuffered);
        }
    }

    [MunRPC]
    void GameOverOnNetwork()
    {
        this.gameOver = true;
        Debug.Log("GameOver");
    }

    public void GameClear()
    {
        if (!this.gameClear && MonobitNetwork.isHost)
        {
            this.gameRuleMonobitView.RPC(nameof(this.GameClearOnNetwork), MonobitTargets.AllBuffered);
        }
    }

    [MunRPC]
    void GameClearOnNetwork()
    {
        this.gameClear = true;
        this.clearSeAudio.Play();
        Debug.Log("GameClear");
    }

    /// <summary>他プレイヤーが入室してきた際に呼ばれるコールバック</summary>
    /// <param name="newPlayer">入室してきたプレイヤーの情報</param>
    private void OnOtherPlayerConnected(MonobitPlayer newPlayer)
    {
        Debug.Log($"Time Remaining: {newPlayer.name}");
        this.gameRuleMonobitView.RPC(nameof(this.SetRemainTime), newPlayer, this.timeRemaining);
    }

    [MunRPC]
    void SetRemainTime(float time)
    {
        this.timeRemaining = time;
        Debug.Log("Remain time");
    }
}
