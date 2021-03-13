using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject aerialFloorPrefab;     // AerialFloor_Mid をインスペクターからアサインする

    [SerializeField]
    private Transform generateTran;           // プレファブのクローンを生成する位置の設定

    [Header("生成までの待機時間")]
    public float waitTime;                    // １回生成するまでの待機時間

    private float timer;                      // 待機時間の計測用

    [SerializeField]
    private GameDirector gameDirector;

    private bool isActive;   // Trueなら生成し、Falseなら生成しない

    void Update()
    {
        // 停止中は生成しない
        if (isActive == false)
        {
            return;
        }

        // 時間を計測する
        timer += Time.deltaTime;

        // 計測している時間がwaitTimeの値と同じか、超えたら
        if (timer >= waitTime)
        {
            // 次回の計測用にtimerを0にする
            timer = 0;

            // クローン生成用のメソッドを呼び出す
            GenerateFloor();
        }
    }

    /// <summary>
    /// プレファブを元にクローンのゲームオブジェクトを生成
    /// </summary>
    private void GenerateFloor()
    {

        // 空中床のプレファブを元にクローンのゲームオブジェクトを生成
        GameObject obj = Instantiate(aerialFloorPrefab, generateTran);

        // ランダムな値を取得
        float randomPosY = Random.Range(-4.0f, 4.0f);

        // Y軸にランダムな値を加算して、生成されるたびに高さの位置を変更する
        obj.transform.position = new Vector2(obj.transform.position.x, obj.transform.position.y + randomPosY);

        // 生成数をカウントアップ
        gameDirector.GenerateCount++;
    }

    /// <summary>
    /// FloorGeneratorの準備
    /// </summary>
    /// <param name="gameDirector"></param>
    public void SetUpGenerator(GameDirector gameDirector)
    {
        this.gameDirector = gameDirector;
        Debug.Log(gameDirector);

    }

    /// <summary>
    /// 生成状態のオン／オフ切り替え
    /// </summary>
    /// <param name="isSwitch"></param>
     public void SwitchActivation(bool isSwitch)
        {
            isActive = isSwitch;
        }
    
}