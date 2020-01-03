using UnityEngine;

public class TargetCursor : MonoBehaviour
{
    // 半径
    public float radius = 1.0f;
    // 回転速度
    public float angularVelocity = 480.0f;
    // 目的地
    public Vector3 destination = new Vector3(0.0f, 0.5f, 0.0f);
    // 位置
    Vector3 position = new Vector3(0.0f, 0.5f, 0.0f);
    // 角度
    float angle = 0.0f;

    // 位置を設定する
    public void SetPosition(Vector3 iPosition)
    {
        this.destination = iPosition;
        // 高さは固定
        this.destination.y = 0.5f;
    }

    void Start()
    {
        // 初期位置を目的地に設定
        this.SetPosition(this.transform.position);
        this.position = this.destination;
    }

    void Update()
    {
        this.position += (this.destination - this.position) / 10.0f;
        // 回転角度
        this.angle += this.angularVelocity * Time.deltaTime;
        // オフセット位置
        var offset = Quaternion.Euler(0.0f, this.angle, 0.0f) * new Vector3(0.0f, 0.0f, this.radius);
        // エフェクトの位置
        this.transform.localPosition = this.position + offset;
    }
}