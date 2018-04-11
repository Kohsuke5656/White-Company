using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalCollider : MonoBehaviour {
    //============================================================================
    #region メンバ
    //判定
    public bool OnLeft = false;
    public bool OnRight = false;
    private float sizeX;
    private float sizeY;
    #endregion
    //============================================================================


    //============================================================================
    #region MonoBegaviourメソッドのオーバーライド
    //<sumary>-------------------------------------------------------------------
    //  初期化
    //</sumary>------------------------------------------------------------------
    void Start()
    {
        sizeX = GetComponent<Block>().sizeX;
        sizeY = GetComponent<Block>().sizeY;
    }
    //<sumary>-------------------------------------------------------------------
    //  一定フレームで更新
    //</sumary>------------------------------------------------------------------
    void FixedUpdate () {
        Vector3 halfExtents = new Vector3(transform.localScale.x　/ sizeX / 6.0f, transform.localScale.y/ sizeY, transform.localScale.z / 2.0f);
        Vector3 Right = new Vector3(1, 0, 0);
        Vector3 Left = new Vector3(-1, 0, 0);
        //左チェック
        if (Physics.BoxCast(transform.localPosition, halfExtents, Left, transform.localRotation, 
            transform.localScale.x/ sizeX / 3.0f, LayerMask.GetMask("Floor") | LayerMask.GetMask("Block")))
        {
            OnLeft = true;
        }
       
        //右チェック
        if (Physics.BoxCast(transform.localPosition, halfExtents, Right, transform.localRotation, 
            transform.localScale.x / sizeX / 3.0f, LayerMask.GetMask("Floor") | LayerMask.GetMask("Block")))
        {
            OnRight = true;
        }
      

    }
    #endregion
    //============================================================================
}
