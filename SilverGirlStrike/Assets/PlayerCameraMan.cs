using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤーを追従するカメラの目印となるキャラ
//左スティックを倒した方向
public class PlayerCameraMan : CharacterObject
{
    [SerializeField]
    private Fuchan.PlayerObject targetPlayer;

    [SerializeField]
    float power;

    [SerializeField]
    float lookDistance;

    private Vector3 lookVector;
    private Vector3 lookVectorGoal;


    private void Start()
    {
        this.lookVector = Vector3.zero;
    }


    public override void ApplyDamage()
    {
        //ダメージとか受けない
    }

    public override bool Damage(AttackData attackData)
    {
        //ダメージとか受けない
        return false;
    }

    public override void MoveCharacter()
    {


        //プレイヤーにべったり
        this.transform.position = targetPlayer.transform.position;

        if (this.targetPlayer.IsCurrentState((int)Fuchan.PlayerObject.State.START))
        {
            return;
        }


        //そこからずらす
        //スティックの向きを取得
        float stickX = Input.GetAxis("RStickX");
        float stickY = Input.GetAxis("RStickY") * -1;
        var vector = new Vector3(stickX, stickY);

        if (vector.magnitude > 0.6f)
        {
            this.lookVectorGoal = vector;
        }
        else
        {
            this.lookVectorGoal = Vector3.zero;
        }

        this.lookVector += (this.lookVectorGoal - this.lookVector) * power;

        this.transform.position += this.lookVector * this.lookDistance;
    }

    public override void UpdateCharacter()
    {
    }
}
