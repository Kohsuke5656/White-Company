using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//IDを色から取得するための列挙型
enum VirusID_Color
{
    WHITE,      //0b_0000
    YELLOW,     //0b_0001
    MAZENDA,    //0b_0010
    RED,        //0b_0011
    CYAN,       //0b_0100
    GREEN,      //0b_0101
    BLUE,       //0b_0110
    BLACK       //0b_0111
}


public class Block : MonoBehaviour {

    //============================================================================
    #region メンバ
    //<sumary>--------------------------------------------------------------------
    //保持しているウイルスのＩＤ(４ビット０～１５)
    //第１ビット：Walk        1 or 0
    //第２ビット：Bounce      1 or 0
    //第３ビット：未使用      0
    //第４ビット：未使用      0
    //</sumary>-------------------------------------------------------------------
    public uint VirusID;
    //潜伏中のウイルスＩＤ
    private uint NextVirusID;
    private uint ErosionCount = 0;
    private const uint ErosionTime = 17;

    //ブロック何個分
    public int sizeX = 1;
    public int sizeY = 1;

    //歩く速度の初期値
    public float speed;

    //ジャンプする間隔
    public int JumpCount = 0;
    public int JumpInterval;
    public float JumpPower;
    
    //ブロックの色
    private Color BlockColor;

    //リジッドボディ
    private Rigidbody rigidbody;

    //ウイルスの当たり判定
    GameObject VirusSphere;
    //ウイルスパーティクル
    GameObject VirusEffect;

    //注射・吸い取り
    public uint InjectionTime;
    public uint RejectionTime;
    private uint InjectionCount = 0;
    private uint RejectionCount = 0;
    private bool NowInjection = false;
    private bool NowRejection = false;

    #endregion
    //============================================================================


    //============================================================================
    #region MonoBegaviourメソッドのオーバーライド
    //<sumary>-------------------------------------------------------------------
    //  初期化
    //</sumary>------------------------------------------------------------------
    void Start()
    {
        //最初は潜伏ウイルスも同じ
        NextVirusID = VirusID;
        //ウイルスIDに応じて色変更
        ChangeBlockColor();
        //ブロックのサイズを変更
        transform.localScale = new Vector3(transform.localScale.x * sizeX, transform.localScale.y * sizeY, transform.localScale.z);
        //リジッドボディコンポーネント取得
        rigidbody = GetComponent<Rigidbody>();
        //ウイルスの当たり判定
        VirusSphere = transform.Find("VirusSphere").gameObject;
        //ウイルスのエフェクト
        VirusEffect = VirusSphere.transform.Find("VirusEffect").gameObject;
        //ウイルスを持っていれば、ウイルスの判定球とエフェクトを有効にする
        if (VirusID > 0)
        {
            VirusActivation();
        }
    }

    //<sumary>-------------------------------------------------------------------
    //  一定フレームで更新
    //</sumary>------------------------------------------------------------------
    void FixedUpdate () {
        //吸い取り中
        if (NowRejection)
        {
            //物理運動中止
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;

            //色変更
            ChangeBlockColorWhiteByTime(RejectionCount, RejectionTime);
            //吸い取り終了
            if (RejectionCount == RejectionTime)
            {
                NowRejection = false;
                RejectionCount = 0;
                VirusID = 0;
                //物理運動再開
                rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
                //ウイルをを非アクティブ化
                VirusDeactivation();
            }
            else
                RejectionCount++;
            return;
        }

        //新しいウイルスが潜伏中の処理/注射中
        if (NextVirusID != VirusID)
        {
            //浸食
            if(!NowInjection)
                HidingVirusErosion();

            //注射中
            if (NowInjection)
            {
                //色更新
                ChangeBlockColorByTime(InjectionCount, InjectionTime);
                //物理運動中止
                rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                //注射終了
                if (InjectionCount == InjectionTime)
                {
                    NowInjection = false;
                    InjectionCount = 0;
                    VirusID = NextVirusID;
                    //物理運動再開
                    rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
                    //ウイルスをアクティブにする
                    VirusActivation();
                }
                else
                {
                    InjectionCount++;
                }
                return;
            }
            
        }

        //歩く
        if ((VirusID & 0x01) == 1)
        {
           VirusWalk();
        }
        //跳ねる
        if((VirusID & 0x02) >> 1 == 1)
        {
            VirusJump();
        }
        //浮く
        if ((VirusID & 0x04) >> 2 == 1)
        {
            VirusFly();
        }
    }
    //<sumary>-------------------------------------------------------------------
    //  ウイルスと衝突
    //</sumary>------------------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Virus")
        {
            //衝突したウイルスの親のブロック取得
            var block = other.transform.GetComponentInParent<Block>();
            //IDの異なるビットを１
            uint xor = block.VirusID ^ VirusID;

            //互いに異なるウイルスを持っている場合
            if (xor > 0)
            {
                //さらに論理和を取る
                uint xor_or = xor | VirusID;
                //相手が自分が持っているウイルスを持ってないだけの場合
                if (xor_or == VirusID) return;

                //相手が新しいウイルスを持っている場合ID更新
                NextVirusID |= xor;
                //print("Infection to \"" + name + "\" from \"" + block.name + "\"");
            }
        }
    }

    //<sumary>-------------------------------------------------------------------
    //  衝突開始
    //</sumary>------------------------------------------------------------------
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Block")
    //    {
    //        //異なるビットを１
    //        uint xor = collision.gameObject.GetComponent<Block>().VirusID ^ VirusID;

    //        //互いに異なるウイルスを持っている場合
    //        if (xor > 0)
    //        {
    //            //さらに論理和を取る
    //            uint xor_or = xor | VirusID;
    //            //相手が自分が持っているウイルスを持ってないだけの場合
    //            if (xor_or == VirusID) return;

    //            //相手が新しいウイルスを持っている

    //            //ID更新
    //            VirusID |= xor;
    //            //色更新
    //            ChangeBlockColor();
    //            //ウイルスオブジェクトの有効化
    //            if (VirusID > 0)
    //            {
    //                VirusSphere.SetActive(true);
    //                VirusEffect.SetActive(true);
    //                var main = VirusEffect.GetComponent<ParticleSystem>().main;
    //                main.startColor = BlockColor;
    //            }
    //        }
    //    }
    //}
    //<sumary>-------------------------------------------------------------------
    //  衝突中
    //</sumary>------------------------------------------------------------------
   
    //private void OnCollisionStay(Collision collision)
    //{
    //    if(collision.gameObject.tag == "Block")
    //    {
    //        //異なるビットを１
    //        uint xor = collision.gameObject.GetComponent<Block>().VirusID ^ VirusID;

    //        //互いに異なるウイルスを持っている場合
    //        if (xor > 0)
    //        {
    //            //さらに論理和を取る
    //            uint xor_or = xor | VirusID;
    //            //相手が自分が持っているウイルスを持ってないだけの場合
    //            if (xor_or == VirusID) return;
                
    //            //相手が新しいウイルスを持っている
               
    //            //ID更新
    //            VirusID |= xor;
    //            //色更新
    //            ChangeBlockColor();
    //            //ウイルスオブジェクトの有効化
    //            if (VirusID > 0)
    //            {
    //                VirusSphere.SetActive(true);
    //                VirusEffect.SetActive(true);
    //                var main = VirusEffect.GetComponent<ParticleSystem>().main;
    //                main.startColor = BlockColor;
    //            }

    //        }
    //    }
    //}

    #endregion
    //============================================================================


    //============================================================================
    #region Block固有メソッド
    //<sumary>-------------------------------------------------------------------
    //ブロックの物理運動
    //</sumary>------------------------------------------------------------------
    private delegate void VirusAction();

    //<sumary>-------------------------------------------------------------------
    //歩く
    //</sumary>------------------------------------------------------------------
    private void VirusWalk()
    {
        //壁に当たった時向きを変える
        if(speed < 0 && GetComponent<HorizontalCollider>().OnLeft)
        {
            GetComponent<HorizontalCollider>().OnLeft = false;
            speed *= -1;   
        }
        else if (speed > 0 && GetComponent<HorizontalCollider>().OnRight)
        {
            GetComponent<HorizontalCollider>().OnRight = false;
            speed *= -1;
        }
            
        rigidbody.velocity = new Vector3(speed, rigidbody.velocity.y, 0);
    }

    //<sumary>-------------------------------------------------------------------
    //ジャンプ
    //</sumary>------------------------------------------------------------------
    private void VirusJump()
    {
        if (JumpCount == JumpInterval)
        {
            Vector3 Power = new Vector3(0, JumpPower, 0);
            rigidbody.AddForce(Power);
            JumpCount = 0;
        }
        else
            JumpCount++;
    }

    //<sumary>-------------------------------------------------------------------
    //  浮く
    //</sumary>------------------------------------------------------------------
    private void VirusFly()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y - transform.localScale.y / 2, transform.position.z);
        //高さを計るRay
        Ray HeightMeasure = new Ray(origin, new Vector3(0, -1, 0)) ;
        RaycastHit hit;
        //Color debugColor = Color.cyan;
        
        //重力に逆らう力を加える
        rigidbody.AddForce(new Vector3(0, 30, 0));

        if (Physics.Raycast(HeightMeasure, out hit, 100.0f, LayerMask.GetMask("Floor") | LayerMask.GetMask("Block"))){

            print(hit.collider.gameObject.name);
            //地面からの高さ
            float length = hit.distance;
            //地面から自分のサイズ一個分以上
            if(length > transform.localScale.y - 0.1f)
            {
                //デバッグ
                Debug.DrawRay(HeightMeasure.origin, HeightMeasure.direction, Color.red, 0.1f);
                //さらに上なら重力に逆らう力を切る
                if (length > transform.localScale.y)
                    rigidbody.AddForce(0, -30, 0);
            }
            //それ以下なら浮き上がる
            else
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
                //デバッグ
                //デバッグ
                Debug.DrawRay(HeightMeasure.origin, HeightMeasure.direction, Color.green, 0.1f);
            }
        }
        
    }

    //<sumary>-------------------------------------------------------------------
    //  潜伏中のウイルスが浸食する処理
    //</sumary>------------------------------------------------------------------
    private void HidingVirusErosion()
    {
        //色更新
        ChangeBlockColorByTime(ErosionCount, ErosionTime);
        //発症
        if (ErosionCount == ErosionTime)
        {
            //感染
            VirusID = NextVirusID;
            //ウイルスオブジェクトの有効化
            if (VirusID > 0)
            {
                VirusActivation();
            }
            //カウント０
            ErosionCount = 0;
        }
        //浸食
        else
        {
            //カウントアップ
            ErosionCount++;
        }
       
    }

    //<sumary>-------------------------------------------------------------------
    //  ウイルス注射される（プレイヤー側からアクセスする）
    //</sumary>------------------------------------------------------------------
    public void Injected(uint ID)
    {
        NowInjection = true;
        InjectionCount = 0;
        //カウント０
        ErosionCount = 0;
        NextVirusID |= ID;
    }

    //<sumary>-------------------------------------------------------------------
    //  ウイルス除去される（プレイヤー側からアクセスする）
    //</sumary>------------------------------------------------------------------
    public uint Rejected()
    {
        NowRejection = true;
        RejectionCount = 0;
        NextVirusID = 0;
        return VirusID;
    }

    //<sumary>-------------------------------------------------------------------
    //  色変更
    //</sumary>------------------------------------------------------------------
    private void ChangeBlockColor()
    {
       //全てを1.0にする
        BlockColor = Color.white;

        //第１ビット：Walk
        if ((VirusID & 0x01) == 1)
        {
            //イエロー
            BlockColor.b = 0.0f;
        }
        //第２ビット：Bounce
        if ((VirusID & 0x02) >> 1 == 1)
        {
            //マゼンダ（イエローならレッド）
            BlockColor.g = 0.0f;
        }
        //第３ビット：未使用
        if ((VirusID & 0x04) >> 2 == 1)
        {
            //シアン（イエローならグリーン、マゼンダならブルー、レッドならブラック）
            BlockColor.r = 0.0f;
        }

        //更新
        GetComponent<Renderer>().material.color = BlockColor;

    }
    //<sumary>-------------------------------------------------------------------
    //  だんだん色変更(浸食)
    //</sumary>------------------------------------------------------------------
    private void ChangeBlockColorByTime(uint count, uint time)
    {
        //全てを1.0にする
        BlockColor = Color.white;

        //第１ビット：Walk
        if ((NextVirusID & 0x01) == 1)
        {
            //イエロー
            BlockColor.b = (float)(time - count) / (float)time;
        }
        //第２ビット：Bounce
        if ((NextVirusID & 0x02) >> 1 == 1)
        {
            //マゼンダ（イエローならレッド）
            BlockColor.g = (float)(time - count) / (float)time;
        }
        //第３ビット：未使用
        if ((NextVirusID & 0x04) >> 2 == 1)
        {
            //シアン（イエローならグリーン、マゼンダならブルー、レッドならブラック）
            BlockColor.r = (float)(time - count) / (float)time;
        }

        //更新
        GetComponent<Renderer>().material.color = BlockColor;

    }
    //<sumary>-------------------------------------------------------------------
    //  だんだん白くする
    //</sumary>------------------------------------------------------------------
    private void ChangeBlockColorWhiteByTime(uint count, uint time)
    {
        Color color = BlockColor;

        color.r = BlockColor.r + (float)(1 - BlockColor.r) / time * count;
        color.g = BlockColor.g + (float)(1 - BlockColor.g) / time * count;
        color.b = BlockColor.b + (float)(1 - BlockColor.b) / time * count;

        //更新
        GetComponent<Renderer>().material.color = color;

        //最後のフレーム
        if(count == time)
        {
            GetComponent<Renderer>().material.color = BlockColor = Color.white;
        }

    }

    //<sumary>-------------------------------------------------------------------
    //  ウイルスをアクティブにする
    //</sumary>------------------------------------------------------------------
    void VirusActivation()
    {
        VirusSphere.SetActive(true);
        VirusEffect.SetActive(true);
        var main = VirusEffect.GetComponent<ParticleSystem>().main;
        main.startColor = BlockColor;
    }

    //<sumary>-------------------------------------------------------------------
    //  ウイルスを非アクティブにする
    //</sumary>------------------------------------------------------------------
    void VirusDeactivation()
    {
        var main = VirusEffect.GetComponent<ParticleSystem>().main;
        main.startColor = BlockColor;
        VirusEffect.SetActive(false);
        VirusSphere.SetActive(false);
    }
    #endregion
    //============================================================================

}
