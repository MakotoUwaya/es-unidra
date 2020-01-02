using UnityEngine;

/// <summary>
/// キャラクターを移動させる
/// </summary>
public class CharacterMove : MonoBehaviour
{
    // 重力値
    const float GravityPower = 9.8f;
    //　目的地についたとみなす停止距離
    const float StoppingDistance = 0.6f;

    // 現在の移動速度
    Vector3 velocity = Vector3.zero;
    // キャラクターコントローラーのキャッシュ
    CharacterController characterController;
    // 到着済
    public bool arrived = false;

    // 向きを強制的に指示するか
    bool forceRotate = false;

    // 強制的に向かせたい方向
    Vector3 forceRotateDirection;

    // 目的地
    public Vector3 destination;

    // 移動速度
    public float walkSpeed = 6.0f;

    // 回転速度
    public float rotationSpeed = 360.0f;

    void Start()
    {
        this.characterController = this.GetComponent<CharacterController>();
        this.destination = this.transform.position;
    }

    void Update()
    {

        // 移動速度velocityを更新する
        if (this.characterController.isGrounded)
        {
            //　水平面での移動を考えるのでXZのみ扱う.
            var destinationXZ = this.destination;
            // 高さを目的地と現在地を同じにしておく
            destinationXZ.y = this.transform.position.y;

            // 目的地までの距離と方角を求める
            var direction = (destinationXZ - this.transform.position).normalized;
            var distance = Vector3.Distance(this.transform.position, destinationXZ);

            // 現在の速度をキャッシュ
            var currentVelocity = this.velocity;

            if (this.arrived || distance < StoppingDistance)
            {
                this.arrived = true;
            }

            // 移動速度を求める.
            if (this.arrived)
            {
                this.velocity = Vector3.zero;
            }
            else
            {
                this.velocity = direction * this.walkSpeed;
            }

            // スムーズに補間.
            this.velocity = Vector3.Lerp(currentVelocity, this.velocity, Mathf.Min(Time.deltaTime * 5.0f, 1.0f));
            this.velocity.y = 0;


            if (!this.forceRotate)
            {
                // 向きを行きたい方向に向ける.
                if (this.velocity.magnitude > 0.1f && !this.arrived)
                { // 移動してなかったら向きは更新しない.
                    var characterTargetRotation = Quaternion.LookRotation(direction);
                    this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, characterTargetRotation, this.rotationSpeed * Time.deltaTime);
                }
            }
            else
            {
                // 強制向き指定.
                var characterTargetRotation = Quaternion.LookRotation(this.forceRotateDirection);
                this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, characterTargetRotation, this.rotationSpeed * Time.deltaTime);
            }

        }

        // 重力.
        this.velocity += Vector3.down * GravityPower * Time.deltaTime;

        // 接地していたら思いっきり地面に押し付ける.
        // (UnityのCharactorControllerの特性のため）
        var snapGround = Vector3.zero;
        if (this.characterController.isGrounded)
        {
            snapGround = Vector3.down;
        }

        // CharacterControllerを使って動かす.
        this.characterController.Move(this.velocity * Time.deltaTime + snapGround);

        if (this.characterController.velocity.magnitude < 0.1f)
        { 
            this.arrived = true;
        }

        // 強制的に向きを変えないようにする
        if (this.forceRotate && Vector3.Dot(this.transform.forward, this.forceRotateDirection) > 0.99f)
        {
            this.forceRotate = false;
        }
    }

    public void SetDestination(Vector3 destination)
    {
        this.arrived = false;
        this.destination = destination;
    }

    public void SetDirection(Vector3 direction)
    {
        this.forceRotateDirection = direction;
        this.forceRotateDirection.y = 0;
        this.forceRotateDirection.Normalize();
        this.forceRotate = true;
    }

    public void StopMove()
    {
        // 現在地点を目的地にしてしまう
        this.destination = this.transform.position;
    }

    public bool Arrived()
    {
        return this.arrived;
    }


}
