using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    // 体力
    public int HP = 100;
    public int MaxHP = 100;

    // 攻撃力
    public int Power = 10;

    // 最後に攻撃した対象
    public GameObject lastAttackTarget = null;

    // プレイヤー名.
    public string characterName = "Player";

    //状態
    public bool attacking = false;
    public bool died = false;

}
