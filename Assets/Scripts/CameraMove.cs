using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private GameObject mainCamera;              //���C���J�����i�[�p
    [SerializeField] private GameObject playerObject;            //��]�̒��S�ƂȂ�v���C���[�i�[�p
    public float rotateSpeed = 2.0f;            //��]�̑���
    public float moveSpeed = 2.0f;

    //�Ăяo�����Ɏ��s�����֐�
    void Start()
    {
        //���C���J�����ƃ��j�e�B���������ꂼ��擾
        mainCamera = Camera.main.gameObject;
        //playerObject = GameObject.Find("CameraMoveManager");
    }


    //�P�ʎ��Ԃ��ƂɎ��s�����֐�
    void Update()
    {
        //�}�E�X�z�C�[�����N���b�N����Ă���Ƃ�����
        if (Input.GetMouseButton(2))
        {
            //rotateCamera�̌Ăяo��
            rotateCamera();
        }

        // W�L�[�i�O���ړ��j
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += moveSpeed * transform.forward * Time.deltaTime;
        }

        // S�L�[�i����ړ��j
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= moveSpeed * transform.forward * Time.deltaTime;
        }

        // D�L�[�i�E�ړ��j
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += moveSpeed * transform.right * Time.deltaTime;
        }

        // A�L�[�i���ړ��j
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= moveSpeed * transform.right * Time.deltaTime;
        }
    }

    //�J��������]������֐�
    private void rotateCamera()
    {
            //Vector3��X,Y�����̉�]�̓x�������`
            Vector3 angle = new Vector3(Input.GetAxis("Mouse X") * rotateSpeed, Input.GetAxis("Mouse Y") * rotateSpeed, 0);

            //transform.RotateAround()�����悤���ă��C���J��������]������
            mainCamera.transform.RotateAround(playerObject.transform.position, Vector3.up, angle.x);
            mainCamera.transform.RotateAround(playerObject.transform.position, transform.right, angle.y);
    }
}
//Input.GetMouseButtonDown
