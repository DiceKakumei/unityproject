using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private GameObject mainCamera;              //メインカメラ格納用
    [SerializeField] private GameObject playerObject;            //回転の中心となるプレイヤー格納用
    public float rotateSpeed = 2.0f;            //回転の速さ
    public float moveSpeed = 2.0f;

    //呼び出し時に実行される関数
    void Start()
    {
        //メインカメラとユニティちゃんをそれぞれ取得
        mainCamera = Camera.main.gameObject;
        //playerObject = GameObject.Find("CameraMoveManager");
    }


    //単位時間ごとに実行される関数
    void Update()
    {
        //マウスホイールがクリックされているときだけ
        if (Input.GetMouseButton(2))
        {
            //rotateCameraの呼び出し
            rotateCamera();
        }

        // Wキー（前方移動）
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += moveSpeed * transform.forward * Time.deltaTime;
        }

        // Sキー（後方移動）
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= moveSpeed * transform.forward * Time.deltaTime;
        }

        // Dキー（右移動）
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += moveSpeed * transform.right * Time.deltaTime;
        }

        // Aキー（左移動）
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= moveSpeed * transform.right * Time.deltaTime;
        }
    }

    //カメラを回転させる関数
    private void rotateCamera()
    {
            //Vector3でX,Y方向の回転の度合いを定義
            Vector3 angle = new Vector3(Input.GetAxis("Mouse X") * rotateSpeed, Input.GetAxis("Mouse Y") * rotateSpeed, 0);

            //transform.RotateAround()をしようしてメインカメラを回転させる
            mainCamera.transform.RotateAround(playerObject.transform.position, Vector3.up, angle.x);
            mainCamera.transform.RotateAround(playerObject.transform.position, transform.right, angle.y);
    }
}
//Input.GetMouseButtonDown
