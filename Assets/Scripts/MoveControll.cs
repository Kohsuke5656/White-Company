using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveControll : MonoBehaviour {

    //  フラグ名
     [SerializeField] private bool bWalkFlg, bJumpFlg, bInjectFlg, bRejectFlg, bRightPush, bLeftPush;

    [SerializeField]
    private int nJumpCnt = 0,
                nInjectCnt = 41,
                nRejectCnt = 41;
    //ジャンプ力
    [System.NonSerialized] public float JumpPower;

    //保持しているウイルス
    private uint HaveVirusType = 0;
    //プレイヤーの色
    private Color PlayerColor;

    //  乗っているブロック
    Block OnBlock = null;
    bool OnBlockFlg = false;
    //  押しているブロック
    GameObject PushBlock = null;
    bool PushBlockFlg = false;

    //ＵＩ
    public Text text;
    public Image image;

    // Use this for initialization
    void Start () {
        PlayerColor = Color.white;
        text.text = "Virus: None";
        image.color = Color.white;
    }

	// Update is called once per frame
	void Update () {
        #region 移動
        if (nInjectCnt > 40){
            if (Input.GetKey(KeyCode.RightArrow) && !bInjectFlg && !bRejectFlg)
            {
                bWalkFlg = true;
                transform.position +=
                    new Vector3(0.150f, 0.0f, 0.0f);
                transform.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
                if(bRightPush && PushBlock != null)
                {
                    PushBlock.transform.position += new Vector3(0.150f, 0.0f, 0.0f);
                }
            }

            else if (Input.GetKey(KeyCode.LeftArrow) && !bInjectFlg && !bRejectFlg)
            {
                bWalkFlg = true;
                transform.position +=
                    new Vector3(-0.150f, 0.0f, 0.0f);
                transform.eulerAngles = new Vector3(0.0f, -90.0f, 0.0f);
                if (bLeftPush && PushBlock != null)
                {
                    PushBlock.transform.position += new Vector3(-0.150f, 0.0f, 0.0f);
                }
            }

            else{
                bWalkFlg = false;
            }
        }

        #endregion

        #region ジャンプ
        if (Input.GetKey(KeyCode.Z) && !bJumpFlg && !bInjectFlg && !bRejectFlg){
            bJumpFlg = true;
            nJumpCnt = 0;
        }
        #endregion

        #region 採取と注入
        else if (Input.GetKey(KeyCode.X) && !bInjectFlg && !bRejectFlg){
            print("push X to ");
            if (OnBlock != null)
            {
                //  注入
                if (HaveVirusType != 0)
                {
                    print("Injection");
                    nInjectCnt = 0;
                    InjectVirus();
                }
                //  吸い取り
                else
                {
                    print("Rejection");
                    nRejectCnt = 0;
                    RejectVirus();
                }
            }
        }
        #endregion

        #region フラグのチェック
        //ジャンプしている
        if (bJumpFlg){
            if (nJumpCnt > 40)
                bJumpFlg = false;

            nJumpCnt++;
            Jump();
        }
        //注射している
        else if (bInjectFlg){
            nInjectCnt++;
            if (nInjectCnt > 40)
            {
                bInjectFlg = false;
                ChangeColorToWhite();
            }
        }
        //吸い取りしている
        else if (bRejectFlg){
            nRejectCnt++;
            if (nRejectCnt > 40)
            {
                bRejectFlg = false;
                ChangeColor();
            }
        }
        #endregion

        //ブロックに載っているかチェック
        CheckOnBlock();
        //ブロックを押しているか
        CheckPushBlock();

        //ＵＩ更新
        UpdateUI();
    }

    //  ジャンプ
    void Jump(){
        transform.position +=
            new Vector3(0.0f, JumpPower / (nJumpCnt + 1), 0.0f);
    }

    //  注射
    void InjectVirus(){
        bInjectFlg = true;
        //ブロックに注入
        OnBlock.Injected(HaveVirusType);
        HaveVirusType = 0;
    }

    //  吸い取り
    void RejectVirus()
    {
        bRejectFlg = true;
        //ブロックから吸い取り
        HaveVirusType =  OnBlock.Rejected();
    }

    //  色変え
    void ChangeColor(){
        //第１ビット：Walk
        if ((HaveVirusType & 0x01) == 1)
        {
            //イエロー
            PlayerColor.b = 0.0f;
        }
        //第２ビット：Bounce
        if ((HaveVirusType & 0x02) >> 1 == 1)
        {
            //マゼンダ（イエローならレッド）
            PlayerColor.g = 0.0f;
        }
        //第３ビット：未使用
        if ((HaveVirusType & 0x04) >> 2 == 1)
        {
            //シアン（イエローならグリーン、マゼンダならブルー、レッドならブラック）
            PlayerColor.r = 0.0f;
        }

        //更新
        GetComponent<Renderer>().material.color = PlayerColor;
    }

    //　白くする
    void ChangeColorToWhite()
    {
        PlayerColor = Color.white;
        //更新
        GetComponent<Renderer>().material.color = PlayerColor;
    }

    //  乗っているブロックがあれば取得
    void CheckOnBlock()
    {
        Ray DownRay = new Ray(transform.position, transform.up * -1);
        RaycastHit hit;
        //下にブロックがあるか
        if(Physics.Raycast(DownRay, out hit, 100))
        {
            //print("On ");
            if (hit.collider.gameObject.tag == "Block")
            {
                print("On Block");
                OnBlockFlg = true;
                OnBlock = hit.collider.gameObject.GetComponent<Block>();
                Debug.DrawRay(DownRay.origin, DownRay.direction, Color.red);
            }
            //else
            //{
            //    //print("OffBlock");
            //    Debug.DrawRay(DownRay.origin, DownRay.direction, Color.green);
            //    OnBlockFlg = false;
            //    OnBlock = null;
            //}
        }
        else
        {
            Debug.DrawRay(DownRay.origin, DownRay.direction, Color.green);
            OnBlockFlg = false;
            OnBlock = null;
        }
        
    }

    //  押しているブロックがあれば取得
    void CheckPushBlock()
    {
        //歩き続けている方向に短いRayを飛ばして、一定時間ぶつかった場合に判定を取る、とか？
        //仕様による
        if (!bWalkFlg)
        {
            bLeftPush = false;
            bRightPush = false;
            PushBlock = null;
            return;
        }
        Ray RightRay = new Ray(transform.position, transform.forward);
        Ray LeftRay = new Ray(transform.position, transform.forward * -1);
        RaycastHit RightHit, LeftHit;

        if(Physics.Raycast(RightRay, out RightHit, 10.0f, LayerMask.NameToLayer("Block")))
        {
            if (bRightPush)
            {
                PushBlock = RightHit.collider.gameObject;
            }
            else
            {
                PushBlock = null;
            }
            bRightPush = true;
        }
        if (Physics.Raycast(LeftRay, out LeftHit, 10.0f, LayerMask.NameToLayer("Block")))
        {
            if (bLeftPush)
            {
                PushBlock = LeftHit.collider.gameObject;
            }
            else
            {
                PushBlock = null;
            }
            bLeftPush = true;
        }

    }

    //UIの更新
    void UpdateUI()
    {
        string virusName = "";

        if ((HaveVirusType & 0x01) == 1)
            virusName = "Walk";
        if ((HaveVirusType & 0x02) >> 1 == 1)
            virusName += "Jump";
        if ((HaveVirusType & 0x04) >> 2 == 1)
            virusName += "Fly";
        if (virusName == "")
            virusName = "None";

        text.text = "Virus: " + virusName; 
        //色変え
        image.color = PlayerColor;
    }
}
