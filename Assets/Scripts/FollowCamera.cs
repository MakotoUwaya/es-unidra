using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public float distance = 5.0f;
    public float horizontalAngle = 0.0f;
    /// <summary>
    /// 画面の横幅分カーソルを移動させたとき何度回転するか
    /// </summary>
    public float rotAngle = 180.0f;
    public float verticalAngle = 10.0f;
    public Transform lookTarget;
    public Vector3 offset = Vector3.zero;

    InputManager inputManager;
    void Start()
    {
        this.inputManager = FindObjectOfType<InputManager>();
    }

    void LateUpdate()
    {
        // ドラッグ入力でカメラのアングルを更新する
        if (this.inputManager.IsRotate())
        {
            var anglePerPixel = this.rotAngle / (float)Screen.width;
            var delta = this.inputManager.GetDeltaPosition();
            this.horizontalAngle += delta.x * anglePerPixel;
            this.horizontalAngle = Mathf.Repeat(this.horizontalAngle, 360.0f);
            this.verticalAngle -= delta.y * anglePerPixel;
            this.verticalAngle = Mathf.Clamp(this.verticalAngle, -60.0f, 60.0f);
        }

        // カメラを位置と回転を更新する
        if (this.lookTarget != null)
        {
            var lookPosition = this.lookTarget.position + this.offset;
            // 注視対象からの相対位置を求める
            var relativePos = Quaternion.Euler(this.verticalAngle, this.horizontalAngle, 0) * new Vector3(0, 0, -this.distance);

            // 注視対象の位置にオフセット加算した位置に移動させる
            this.transform.position = lookPosition + relativePos;

            // 注視対象を注視させる
            this.transform.LookAt(lookPosition);

            // 障害物を避ける
            if (Physics.Linecast(lookPosition, this.transform.position, out var hitInfo, 1 << LayerMask.NameToLayer("Ground")))
            {
                this.transform.position = hitInfo.point;
            }
        }
    }
}
