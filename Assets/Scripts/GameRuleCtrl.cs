using UnityEngine;

public class GameRuleCtrl : MonoBehaviour
{
    // 残り時間
    public float timeRemaining = 5.0f * 60.0f;
    // ゲームオーバーフラグ
    public bool gameOver = false;
    // ゲームクリア
    public bool gameClear = false;

    void Update()
    {
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
        Debug.Log("GameClear");
    }
}
