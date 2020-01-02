using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    const float RayCastMaxDistance = 100.0f;
    InputManager inputManager;

    void Start()
    {
        this.inputManager = FindObjectOfType<InputManager>();

        // Assert
        if (this.inputManager == null)
        {
            Debug.LogError("Not found 'InputManager' object.");
        }
    }

    void Update()
    {
        this.Walking();
    }

    void Walking()
    {
        if (this.inputManager.Clicked())
        {
            var clickPos = this.inputManager.GetCursorPosition();
            // RayCastで対象物を調べる
            var ray = Camera.main.ScreenPointToRay(clickPos);
            if (Physics.Raycast(ray, out var hitInfo, RayCastMaxDistance, 1 << LayerMask.NameToLayer("Ground")))
            {
                this.SendMessage("SetDestination", hitInfo.point);
            }
        }
    }
}
