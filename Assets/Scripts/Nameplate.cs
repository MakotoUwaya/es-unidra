using UnityEngine;

public class Nameplate : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 2.0f, 0);
    public CharacterStatus status;
    TextMesh textMesh;

    void Start()
    {
        this.textMesh = this.GetComponent<TextMesh>();
    }

    void Update()
    {
        // 名前の更新
        if (this.textMesh.text != this.status.characterName)
        {
            this.textMesh.text = this.status.characterName;
        }

        // 頭上に移動
        this.transform.position = this.status.transform.position + this.offset;
        //　常にカメラと同じ向きにする
        this.transform.rotation = Camera.main.transform.rotation;
        // 大きさ調整
        var scale = Camera.main.transform.InverseTransformPoint(this.transform.position).z / 30.0f;
        this.transform.localScale = Vector3.one * scale;
    }
}