﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    [SerializeField]
    private GoalChecker goalHousePrefab;         // ゴール地点のプレファブをアサイン

    [SerializeField]
    private PlayerController playerController;   // ヒエラルキーにあるYuko_Playerゲームオブジェクトをアサイン

    [SerializeField]
    private FloorGenerator[] floorGenerators;    // floorGeneratorスクリプトのアタッチされているゲームオブジェクトをアサイン

    [SerializeField]
    private RandomObjectGenerator[] randomObjectGenerators; 

    [SerializeField]
    private RandomObjectGenerator[] ramdomObjectGenerator;

    [SerializeField]
    private AudioManager audioManager;
    // AudioManagerスクリプトのアタッチされているゲームオブジェクトをアサイン

    private bool isSetUp;                        // trueになるとゲーム開始
    private bool isGameUp;                       // trueになるとゲーム終了

    private int generateCount;                   // 空中床の生成回数

    // generateCount 変数のプロパティ
    public int GenerateCount
    {
        set
        {
            generateCount = value;

            Debug.Log("生成数 / クリア目標数 : " + generateCount + " / " + clearCount);

            if (generateCount >= clearCount)
            {
                // ゴール地点を生成
                GenerateGoal();

                // ゲーム終了
                GameUp();
            }
        }
        get
        {
            return generateCount;
        }
    }

    public int clearCount;　　// ゴール地点を生成するまでに必要な空中床の生成回数

    void Start()
    {
        //タイトル曲再生
        StartCoroutine(audioManager.PlayBGM(0));

        // ゲーム開始状態にセット
        isGameUp = false;
        isSetUp = false;

        // FloorGeneratorの準備
        SetUpFloorGenerators();

        // 各ジェネレータを停止
        StopGenerators();
    }

    /// <summary>
    /// FloorGeneratorの準備
    /// </summary>
    private void SetUpFloorGenerators()
    {
        for (int i = 0; i < floorGenerators.Length; i++)
        {
            // FloorGeneratorの準備・初期設定を行う
            floorGenerators[i].SetUpGenerator(this);
        }
    }

    void Update()
    {
        // プレイヤーがはじめてバルーンを生成したら
        if (playerController.isFirstGenerateBallon && isSetUp == false)
        {
            // 準備完了
            isSetUp = true;

            // 各ジェネレータの生成をスタート
            ActivateGenerators();

            //タイトル曲終了、メイン曲再生
            StartCoroutine(audioManager.PlayBGM(1));
        }
    }

    /// <summary>
    /// ゴール地点の生成
    /// </summary>
    private void GenerateGoal()
    {
        // ゴール地点を生成
        GoalChecker goalHouse = Instantiate(goalHousePrefab);

        // ゴール地点の初期設定
        goalHouse.SetUpGoalHouse(this);
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void GameUp()
    {
        // ゲーム終了
        isGameUp = true;

        // 各ジェネレータの生成を停止
        StopGenerators();
    }

    /// <summary>
    /// 各ジェネレータを停止する
    /// </summary>
    private void StopGenerators()
    {
        for (int i = 0; i < randomObjectGenerators.Length; i++)
        {
            randomObjectGenerators[i].SwitchActivation(false);
        }

        for (int i = 0; i < floorGenerators.Length; i++)
        {
            floorGenerators[i].SwitchActivation(false);
        }
    }

    /// <summary>
    /// 各ジェネレータを動かし始める
    /// </summary>
    private void ActivateGenerators()
    {
        for (int i = 0; i < randomObjectGenerators.Length; i++)
        {
            randomObjectGenerators[i].SwitchActivation(true);
        }

        for (int i = 0; i < floorGenerators.Length; i++)
        {
            floorGenerators[i].SwitchActivation(true);
        }
    }

    public void GoalClear()
    {
        //クリア曲再生
        StartCoroutine(audioManager.PlayBGM(2));
    }
}