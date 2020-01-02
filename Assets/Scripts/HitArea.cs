using UnityEngine;

public class HitArea : MonoBehaviour
{

    void Damage(AttackArea.AttackInfo attackInfo)
    {
        this.transform.root.SendMessage("Damage", attackInfo);
    }
}
