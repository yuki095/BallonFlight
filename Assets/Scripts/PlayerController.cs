using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //キー入力用の文字列指定
    private string horizontal = "Horizontal";
    private string jump = "Jump";

    private Rigidbody2D rb;
    private Animator anim;
    private float scale;        //向きの設定

    //制限値
    private float limitPosX = 9.5f;
    private float limitPosY = 5.00f;

    private bool isGameOver = false;  //ゲームオーバーの判定（Trueならゲームオーバー）

    public bool isFirstGenerateBallon;   // 初めてバルーンを生成したかを判定

    public float moveSpeed;     //移動速度
    public float jumpPower;     //ジャンプ・浮遊力

    public bool isGrounded;

    public GameObject[] ballons;    //インスペクターからヒエラルキーにあるBallonを２つアサイン

    public int maxBallonCount;              // バルーンを生成する最大数
    public Transform[] ballonTrans;  　     // バルーンの生成位置の配列
    public GameObject ballonPrefab;         // バルーンのプレファブ

    public float generateTime;              // バルーンを生成する時間
    public bool isGenerating;               // バルーンを生成中かどうかを判定

    public float knockbackPower;            // 敵と接触したとき吹き飛ばされる力

    public int coinPoint;                   // コインを獲得すると増えるポイントの総数

    public UIManager uiManager;

    [SerializeField, Header("Linecast用　地面判定レイヤー")]
    private LayerMask groundLayer;

    [SerializeField]
    private StartChecker startChecker;

    [SerializeField]
    private AudioClip knockbackSE;  //敵と接触したSE用のオーディオファイルをアサイン

    [SerializeField]
    private GameObject knockbackEffectPrefab;  //敵と接触したエフェクト用のプレファブをアサイン

    [SerializeField]
    private AudioClip coinSE;  //コインSE

    [SerializeField]
    private GameObject coinEffectPrefab; // コインエフェクト

    [SerializeField]
    private Joystick joystick;  //FloatingJoystickゲームオブジェクトにアタッチされているFloatingJoystickスクリプトをアサイン

    [SerializeField]
    private Button btnJump;  //btnJumpゲームオブジェクトにアタッチされているButtonコンポーネント

    [SerializeField]
    private Button btnDetach;  //btnDetachOrGenerateゲームオブジェクトにアタッチされているButtonコンポーネント

    private int ballonCount;

    void Start()
    {
        //コンポーネントを取得して変数に代入
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        scale = transform.localScale.x;

        //配列の初期化（バルーンの最大生成数だけ配列の要素数を用意する）
        ballons = new GameObject[maxBallonCount];

        // 
        btnJump.onClick.AddListener(OnClickJump);
        btnDetach.onClick.AddListener(OnClickDetachOrGenerate);
    }

    void Update()
    {
        //地面接地
        isGrounded = Physics2D.Linecast(transform.position + transform.up * 0.4f, transform.position - transform.up * 0.9f, groundLayer);

        //SceneビューにPhysics2D.LinecastメソッドのLineを表示
        Debug.DrawLine(transform.position + transform.up * 0.4f, transform.position - transform.up * 0.9f, Color.red, 1.0f);

        //【修正前】Ballon配列変数の最大要素数が0以上なら
        //if (ballons.Length > 0)

        //Ballon変数の最初の要素の値が空ではないなら＝バルーンが１つ生成されたら
        if (ballons[0] != null)
        {
            //ジャンプ
            if (Input.GetButtonDown(jump))
            {
                Jump();
            }

            //落下中
            if (isGrounded == false && rb.velocity.y < 0.15f)
            {
                //落下アニメを繰り返す
                anim.SetTrigger("Fall");
            }

        }
        else
        {
            //Debug.Log("バルーンがない。ジャンプ不可");
        }

        //Velocity.yの値が5.0fを超えたら値に制限をかける（上空待機を防ぐため）
        if (rb.velocity.y > 5.0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, 5.0f);
        }

        // 地面に接地していて、バルーンが生成中ではない場合
        if (isGrounded == true && isGenerating == false)
        {
            // Qボタンを押したらバルーンを１つ生成する
            if (Input.GetKeyDown(KeyCode.Q))
            {
                StartCoroutine(GenerateBallon());
            }
        }
    }

    /// <summary>
    /// ジャンプと空中浮遊
    /// </summary>
    private void Jump()
    {
        //キャラの位置を上方向へ移動
        rb.AddForce(transform.up * jumpPower);
        //ジャンプアニメーションを再生（Up＋Mid）
        anim.SetTrigger("Jump");
    }

    private void FixedUpdate()
    {
        if (isGameOver == true) {
            return;                 //移動できなくする？
        }

        //移動
        Move();   //？
    }

    /// <summary>
    /// 移動
    /// </summary>
    private void Move()
    {
#if UNITY_EDITOR
        //水平方向への入力受付
        float x = Input.GetAxis(horizontal);
        //x = joystick.Horizontal;
#else
        float x = joystick.Horizontal;
#endif

        //xの値が0でない場合（キー入力がある場合）
        if (x != 0)
        {
            //velocity（速度）に新しい値を代入して移動
            rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);

            //temp変数に現在のLocalScale値を代入
            Vector3 temp = transform.localScale;
            //現在のキー入力値ｘをtemp.xに代入
            temp.x = x;

            //向きが変わるときに小数になるとキャラが縮むので整数にする
            if (temp.x > 0)
            {
                temp.x = scale;
            }
            else
            {
                temp.x = -scale;
            }
            //
            transform.localScale = temp;

            //待機アニメの再生をやめて走るアニメを再生する
            anim.SetBool("Idle", false);        //Idleアニメをfalseにして待機アニメを停止
            anim.SetFloat("Run", 0.5f);         //Runアニメに対して0.5fの値を渡す

        }
        else
        {
            //入力がなかったら横移動の速度を0に
            rb.velocity = new Vector2(0, rb.velocity.y);

            //走るアニメの再生をやめて待機アニメを再生する
            anim.SetFloat("Run", 0.0f);         //Runアニメに対して0.0fの値を渡す
            anim.SetBool("Idle", true);         //Idleアニメをtrueにして待機アニメを再生
        }

        //現在の位置情報を制限範囲内に収める
        float posX = Mathf.Clamp(transform.position.x, -limitPosX, limitPosX);
        float posY = Mathf.Clamp(transform.position.y, -limitPosY, limitPosY);

        //現在の位置を更新（制限範囲を超えた場合、移動範囲を制限する
        transform.position = new Vector2(posX, posY);
    }

    /// <summary>
    /// バルーン生成
    /// </summary>
    /// <returns></returns>
    private IEnumerator GenerateBallon()
    {
        // すべての配列の要素にバルーンが存在している場合には、バルーンを生成しない
        if (ballons[1] != null)
        {
            yield break;
        }

        // 生成中状態にする
        isGenerating = true;

        // isFirstGenerateBallon変数の値がfalse = まだバルーンを１回も生成していないなら
        if (isFirstGenerateBallon == false)
        {
            // 初回バルーン生成を行ったと判断し、trueに変更する = 次回以降はバルーンを生成してもこの処理には入らない
            isFirstGenerateBallon = true;

            Debug.Log("初回のバルーン生成");

            // startChecker変数に代入されているStartCheckerスクリプトにアクセスして、SetInitialSpeedメソッドを実行
            startChecker.SetInitialSpeed();
        }

        // １つめの配列の要素が空なら
        if (ballons[0] == null)
        {
            // 1つ目のバルーン生成を生成して、1番目の配列へ代入
            ballons[0] = Instantiate(ballonPrefab, ballonTrans[0]);
            // ふくらむ
            ballons[0].GetComponent<Ballon>().SetUpBallon(this);
        }
        else
        {
            // 2つ目のバルーン生成を生成して、2番目の配列へ代入
            ballons[1] = Instantiate(ballonPrefab, ballonTrans[1]);
            //　ふくらむ
            ballons[1].GetComponent<Ballon>().SetUpBallon(this);
        }

        // 生成時間分待機
        yield return new WaitForSeconds(generateTime);

        // 生成中状態終了
        isGenerating = false;
    }
    

    private void OnCollisionEnter2D(Collision2D col)
    {

        // 接触したコライダーを持つゲームオブジェクトのTagがEnemyなら 
        if (col.gameObject.tag == "Enemy")
        {
            // キャラと敵の位置から距離と方向を計算して、direction変数に代入
            Vector3 direction = (transform.position - col.transform.position).normalized;   // 正規化？

            // 敵の反対側にキャラを吹き飛ばす
            transform.position += direction * knockbackPower;

            // 敵との接触用のSEを再生
            AudioSource.PlayClipAtPoint(knockbackSE, transform.position);

            // 接触用のエフェクトを、敵の位置にクローンとして生成
            // 生成されたゲームオブジェクトを変数へ代入
            GameObject knockbackEffect = Instantiate(knockbackEffectPrefab, col.transform.position, Quaternion.identity);

            // エフェクトを0.5秒後に破棄
            Destroy(knockbackEffect, 0.5f);
        }
    }

    /// <summary>
    /// バルーン破壊
    /// </summary>
    public void DestroyBallon()
    {

        // TODO 後程、バルーンが破壊される際に「割れた」ように見えるアニメ演出を追加する

        if (ballons[1] != null)
        {
            Destroy(ballons[1]);
        }
        else if (ballons[0] != null)
        {
            Destroy(ballons[0]);
        }
    }

    // IsTriggerがオンのコライダーを持つゲームオブジェクトを通過した場合に呼び出されるメソッド
    private void OnTriggerEnter2D(Collider2D col)
    {

        if (col.gameObject.tag == "Coin")
        {
            // コインの持つCoinスクリプトを取得し、point変数の値をcoinPoint変数に加算
            coinPoint += col.gameObject.GetComponent<Coin>().point;

            // コインを破壊する
            Destroy(col.gameObject);

            uiManager.UpdateDisplayScore(coinPoint);

            // コイン接触用のSEを再生
            AudioSource.PlayClipAtPoint(coinSE, transform.position);

            // 接触用のエフェクトをコインの位置にクローンとして生成
            // 生成されたゲームオブジェクトを変数へ代入
            GameObject coinEffect = Instantiate(coinEffectPrefab, col.transform.position, Quaternion.identity);

            // エフェクトを0.5秒後に破棄
            Destroy(coinEffect, 0.5f);
        }
    }

    /// <summary>
    /// ゲームオーバー
    /// </summary>
    public void GameOver()
    {
        isGameOver = true;

        // ConsoleビューにisGameOver変数の値を表示する
        Debug.Log(isGameOver);

        // ゲームオーバー表示
        uiManager.DisplayGameOverInfo();
    }

    /// <summary>
    /// ジャンプボタンを押した際の処理
    /// </summary>
    private void OnClickJump()
    {
        // バルーンが１つ以上あるなら
        if (ballonCount > 0)
        {
            Jump();
        }
    }

    /// <summary>
    /// バルーン生成ボタンを押した際の処理
    /// </summary>
    private void OnClickDetachOrGenerate()
    {
        // 地面に接地していて、バルーンが２個以下の場合
        if (isGrounded == true && ballonCount < maxBallonCount && isGenerating == false)
        {
            // バルーンの生成中でなければ、バルーンを１つ作成する
            StartCoroutine(GenerateBallon());
        }
    }

}

