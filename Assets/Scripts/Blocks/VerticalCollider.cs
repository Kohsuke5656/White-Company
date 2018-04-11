using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalCollider : MonoBehaviour {
    //============================================================================
    #region メンバ
    //判定
    public bool OnUp = false;
    public bool OnBottom = false;
    //ブロックサイズ
    private float sizeX;
    private float sizeY;
    //プレイヤーが乗っているか
    public bool PlayerOn;
    #endregion
    //============================================================================
    // Use this for initialization
    void Start () {
        sizeX = GetComponent<Block>().sizeX;
        sizeY = GetComponent<Block>().sizeY;
        PlayerOn = false;
    }
	
	// Update is called once per frame
	void fixedUpdate () {
        RaycastHit hit;
        Vector3 halfExtents = new Vector3(transform.localScale.x / sizeX / 6.0f, transform.localScale.y / sizeY / 2.0f, transform.localScale.z / 2.0f);
        //上チェック
        if (Physics.BoxCast(transform.position, halfExtents, transform.up, out hit, transform.rotation,
            transform.localScale.x / sizeX / 3.0f, LayerMask.GetMask("Floor") | LayerMask.GetMask("Block") | LayerMask.GetMask("Player")))
        {
            OnUp = true;
            if (hit.collider.gameObject.tag == "Player")
            {
                PlayerOn = true;
            }
            else
                PlayerOn = false;
        }
        else
            OnUp = false;
        //右チェック
        if (Physics.BoxCast(transform.position, halfExtents, transform.up * -1, transform.rotation, 
            transform.localScale.x / sizeX / 3.0f, LayerMask.GetMask("Floor") | LayerMask.GetMask("Block") | LayerMask.GetMask("Player")))
        {
            OnBottom = true;
        }
        else
            OnBottom = false;

    }
}
