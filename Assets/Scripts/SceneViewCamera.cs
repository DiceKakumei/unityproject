using UnityEngine;

public class SceneViewCamera : MonoBehaviour
{
    public float moveSpeed = 10f;   // �J�����̈ړ����x
    public float scrollSpeed = 10f; // �Y�[�����x
    public float rotateSpeed = 2f;  // ��]���x

    public bool CanMoveCam = true;

    private Vector3 lastMousePos;

    void Update()
    {
        if (!CanMoveCam)
        {
            return;
        }
        // �E�N���b�N�ŃJ������]�i�I�[�r�b�g�j
        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            transform.Rotate(Vector3.up, delta.x * rotateSpeed, Space.World);
            transform.Rotate(Vector3.right, -delta.y * rotateSpeed, Space.Self);
        }

        // ���N���b�N�i�z�C�[���N���b�N�j�Ńp��
        if (Input.GetMouseButton(2))
        {
            Vector3 delta = (Input.mousePosition - lastMousePos) * 0.01f;
            transform.position -= transform.right * delta.x * moveSpeed;
            transform.position -= transform.up * delta.y * moveSpeed;
        }

        // �}�E�X�z�C�[���ŃY�[��
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.position += transform.forward * scroll * scrollSpeed;

        lastMousePos = Input.mousePosition;
    }
}
