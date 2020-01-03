using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRuleCtrl : MonoBehaviour
{
    // 残り時間
    public float timeRemaining = 5.0f * 60.0f;
    // ゲームオーバーフラグ
    public bool gameOver = false;
    // ゲームクリア
    public bool gameClear = false;
    public float sceneChangeTime = 3.0f;

    public AudioClip clearSeClip;
    AudioSource clearSeAudio;

    private void Start()
    {
        this.clearSeAudio = this.gameObject.AddComponent<AudioSource>();
        this.clearSeAudio.clip = this.clearSeClip;
        this.clearSeAudio.loop = false;
    }

    void Update()
    {
        if (this.gameClear || this.gameOver)
        {
            this.sceneChangeTime -= Time.deltaTime;
            if (this.sceneChangeTime <= 0.0f)
            {
                SceneManager.LoadScene("TitleScene");
            }
            return;
        }

        this.timeRemaining -= Time.deltaTime;
        // 残り時間が無くなったらゲームオーバー
        if (this.timeRemaining <= 0.0f)
        {
            this.GameOver();
        }
    }

    public void GameOver()
    {
        this.gameOver = true;
        Debug.Log("GameOver");
    }
    public void GameClear()
    {
        this.gameClear = true;
        this.clearSeAudio.Play();
        Debug.Log("GameClear");
    }
}
