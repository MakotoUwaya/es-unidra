using UnityEngine;

public class CharacterStatusGui : MonoBehaviour
{
    private readonly float baseWidth = 854f;
    private readonly float baseHeight = 480f;

    // ステータス.
    private CharacterStatus playerStatus;
    private Vector2 playerStatusOffset = new Vector2(8f, 80f);

    // 名前.
    private Rect nameRect = new Rect(0f, 0f, 120f, 24f);
    public GUIStyle nameLabelStyle;

    // ライフバー.
    public Texture backLifeBarTexture;
    public Texture frontLifeBarTexture;
    private readonly float frontLifeBarOffsetX = 2f;
    private readonly float lifeBarTextureWidth = 128f;
    private Rect playerLifeBarRect = new Rect(0f, 0f, 128f, 16f);
    private Color playerFrontLifeBarColor = Color.green;
    private Rect enemyLifeBarRect = new Rect(0f, 0f, 128f, 24f);
    private Color enemyFrontLifeBarColor = Color.red;

    // プレイヤーステータスの描画.
    private void DrawPlayerStatus()
    {
        var x = this.baseWidth - this.playerLifeBarRect.width - this.playerStatusOffset.x;
        var y = this.playerStatusOffset.y;
        this.DrawCharacterStatus(
            x, y,
            this.playerStatus,
            this.playerLifeBarRect,
            this.playerFrontLifeBarColor);
    }

    // 敵ステータスの描画.
    private void DrawEnemyStatus()
    {
        if (this.playerStatus.lastAttackTarget != null)
        {
            var target_status = this.playerStatus.lastAttackTarget.GetComponent<CharacterStatus>();
            this.DrawCharacterStatus(
                (this.baseWidth - this.enemyLifeBarRect.width) / 2.0f, 0f,
                target_status,
                this.enemyLifeBarRect,
                this.enemyFrontLifeBarColor);
        }
    }

    // キャラクターステータスの描画.
    private void DrawCharacterStatus(float x, float y, CharacterStatus status, Rect bar_rect, Color front_color)
    {
        // 名前.
        GUI.Label(
            new Rect(x, y, this.nameRect.width, this.nameRect.height),
            status.characterName,
            this.nameLabelStyle);

        var life_value = (float)status.HP / status.MaxHP;
        if (this.backLifeBarTexture != null)
        {
            // 背面ライフバー.
            y += this.nameRect.height;
            GUI.DrawTexture(new Rect(x, y, bar_rect.width, bar_rect.height), this.backLifeBarTexture);
        }

        // 前面ライフバー.
        if (this.frontLifeBarTexture != null)
        {
            var resize_front_bar_offset_x = this.frontLifeBarOffsetX * bar_rect.width / this.lifeBarTextureWidth;
            var front_bar_width = bar_rect.width - resize_front_bar_offset_x * 2;
            var gui_color = GUI.color;
            GUI.color = front_color;
            GUI.DrawTexture(new Rect(x + resize_front_bar_offset_x, y, front_bar_width * life_value, bar_rect.height), this.frontLifeBarTexture);
            GUI.color = gui_color;
        }
    }

    private void Awake()
    {
        var playerCtrl = FindObjectOfType<PlayerCtrl>();
        this.playerStatus = playerCtrl.GetComponent<CharacterStatus>();
    }

    private void OnGUI()
    {
        // 解像度対応.
        GUI.matrix = Matrix4x4.TRS(
            Vector3.zero,
            Quaternion.identity,
            new Vector3(Screen.width / this.baseWidth, Screen.height / this.baseHeight, 1f));

        // ステータス.
        this.DrawPlayerStatus();
        this.DrawEnemyStatus();
    }
}