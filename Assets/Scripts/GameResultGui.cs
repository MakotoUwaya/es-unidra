using UnityEngine;

public class GameResultGui : MonoBehaviour
{
    GameRuleCtrl gameRuleCtrl;
    readonly float baseWidth = 854f;
    readonly float baseHeight = 480f;

    public Texture2D gameOverTexture;
    public Texture2D gameClearTexture;

    void Awake()
    {
        this.gameRuleCtrl = FindObjectOfType<GameRuleCtrl>();
    }

    void OnGUI()
    {
        Texture2D aTexture;
        if (this.gameRuleCtrl.gameClear)
        {
            aTexture = this.gameClearTexture;
        }
        else if (this.gameRuleCtrl.gameOver)
        {
            aTexture = this.gameOverTexture;
        }
        else
        {
            return;
        }

        // 解像度対応.
        GUI.matrix = Matrix4x4.TRS(
            Vector3.zero,
            Quaternion.identity,
            new Vector3(Screen.width / this.baseWidth, Screen.height / this.baseHeight, 1f));

        // リザルト.
        GUI.DrawTexture(new Rect(0.0f, 208.0f, 854.0f, 64.0f), aTexture);
    }
}
