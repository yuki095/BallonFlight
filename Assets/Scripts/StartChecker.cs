using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartChecker : MonoBehaviour
{
    private MoveObject moveObject;   // MoveObjectスクリプトを取得した際に代入するための準備

    void Start()
    {
        // ゲームオブジェクトの持つMoveObjectスクリプトを探して取得し、moveObject変数に代入
        moveObject = GetComponent<MoveObject>(); 
    }

    /// <summary>
    /// 空中床に移動速度を与える
    /// </summary>
    public void SetInitialSpeed()
    {
        //ゲームオブジェクトの持つMoveObjectスクリプトのmoveSpeed変数にアクセスして、右辺の値を代入する
        moveObject.moveSpeed = 0.005f;
    }
}