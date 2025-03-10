using UnityEngine;

public class SceneViewCamera : MonoBehaviour
{
    public float moveSpeed = 10f;   // カメラの移動速度
    public float scrollSpeed = 10f; // ズーム速度
    public float rotateSpeed = 2f;  // 回転速度

    public bool CanMoveCam = true;

    private Vector3 lastMousePos;

    void Update()
    {
        if (!CanMoveCam)
        {
            return;
        }
        // 右クリックでカメラ回転（オービット）
        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            transform.Rotate(Vector3.up, delta.x * rotateSpeed, Space.World);
            transform.Rotate(Vector3.right, -delta.y * rotateSpeed, Space.Self);
        }

        // 中クリック（ホイールクリック）でパン
        if (Input.GetMouseButton(2))
        {
            Vector3 delta = (Input.mousePosition - lastMousePos) * 0.01f;
            transform.position -= transform.right * delta.x * moveSpeed;
            transform.position -= transform.up * delta.y * moveSpeed;
        }

        // マウスホイールでズーム
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.position += transform.forward * scroll * scrollSpeed;

        lastMousePos = Input.mousePosition;
    }
}
