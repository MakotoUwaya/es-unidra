using UnityEngine;

public class InputManager : MonoBehaviour
{
    Vector2 slideStartPosition;
    Vector2 prevPosition;
    Vector2 delta = Vector2.zero;
    bool moved = false;
    bool isRotate = false;

    void Update()
    {
        // スライド開始地点
        if (Input.GetButtonDown("Fire1"))
        {
            this.slideStartPosition = this.GetCursorPosition();
        }

        // 画面の１割以上移動させたらスライド開始と判断する
        if (Input.GetButton("Fire1"))
        {
            if (Vector2.Distance(this.slideStartPosition, this.GetCursorPosition()) >= (Screen.width * 0.1f))
            {
                this.moved = true;
            }
        }

        this.isRotate = Input.GetButton("Fire2");

        // スライド操作が終了したか
        if (!Input.GetButtonUp("Fire1") && !Input.GetButton("Fire1"))
        {
            // スライド終了
            this.moved = false;
        }

        // 移動量を求める
        if (this.moved || this.isRotate)
        {
            this.delta = this.GetCursorPosition() - this.prevPosition;
        }
        else
        {
            this.delta = Vector2.zero;
        }

        // カーソル位置を更新
        this.prevPosition = this.GetCursorPosition();
    }

    // クリックされたか
    public bool Clicked()
    {
        return !this.moved && Input.GetButtonUp("Fire1");
    }

    // スライド時のカーソルの移動量
    public Vector2 GetDeltaPosition()
    {
        return this.delta;
    }

    // スライド中か
    public bool Moved()
    {
        return this.moved;
    }

    public bool IsRotate()
    {
        return this.isRotate;
    }

    public Vector2 GetCursorPosition()
    {
        return Input.mousePosition;
    }
}
