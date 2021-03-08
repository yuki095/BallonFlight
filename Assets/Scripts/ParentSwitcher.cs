using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentSwitcher : MonoBehaviour
{
    private string player = "Player";      // Tagの文字列

    // 他のゲームオブジェクトと接触している間ずっと接触判定を行うメソッド
    private void OnCollisionStay2D(Collision2D col)
    {

        // 接触判定が発生すると col 変数にコライダーの情報が代入される。
        // そのコライダーを持つゲームオブジェクトのTagが player 変数の値（"Player"）と同じ文字列なら
        if (col.gameObject.tag == player)
        {
            // 親子関係にする
            col.transform.SetParent(transform);
        }
    }

    // 他のゲームオブジェクトと離れた際に判定を行うメソッド
    private void OnCollisionExit2D(Collision2D col)
    {
        // 離れると col 変数にコライダーの情報が代入される。
        // そのコライダーを持つゲームオブジェクトのTagが player 変数の値（"Player"）と同じ文字列なら
        if (col.gameObject.tag == player)
        {
            // 親子関係を解消する
            col.transform.SetParent(null);
        }
    }
}