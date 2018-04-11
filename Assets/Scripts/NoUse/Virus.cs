using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//全てのウイルスに継承させるクラス
public class Virus : MonoBehaviour {
    //ウイルスのＩＤ
    public uint VirusID;

    //ウイルスによるアニメーション
    public AnimationClip VirusAnimation;

    public void Awake()
    {
        VirusID = 0x000;
        VirusAnimation = null;
    }

    public Virus()
    {
        VirusID = 0x000;
        VirusAnimation = null;
    }
    public Virus(Virus rhs)
    {
        this.VirusID = rhs.VirusID;
        this.VirusAnimation = rhs.VirusAnimation;
    }
   

}
