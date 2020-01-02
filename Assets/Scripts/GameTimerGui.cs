using UnityEngine;

public class GameTimerGui : MonoBehaviour
{
    GameRuleCtrl gameRuleCtrl;
    readonly float baseWidth = 854f;
    readonly float baseHeight = 480f;

    public Texture timerIcon;
    public GUIStyle timerLabelStyle;

    void Awake()
    {
        this.gameRuleCtrl = FindObjectOfType<GameRuleCtrl>();
    }

    void OnGUI()
    {
        // 解像度対応.
        GUI.matrix = Matrix4x4.TRS(
            Vector3.zero,
            Quaternion.identity,
            new Vector3(Screen.width / this.baseWidth, Screen.height / this.baseHeight, 1f));

        // タイマー.
        GUI.Label(
            new Rect(8f, 8f, 128f, 48f),
            new GUIContent(this.gameRuleCtrl.timeRemaining.ToString("0"), this.timerIcon),
            this.timerLabelStyle);
    }
}
