﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : Character
{
    [SerializeField]
    private Weapon mWeapon1;
    [SerializeField]
    private Weapon mWeapon2;
    [SerializeField]
    private Weapon mCurrentWeapon;

    [SerializeField]
    private bool mIsIFrameOn;
    [SerializeField]
    private float mIFrameCD;
    [SerializeField]
    private float mIFrameDuration;
    [SerializeField]
    private float mIFrameDistance;
    [SerializeField]
    private int mLaserDamage;
    private bool isDouleJumpEnabled;

    public GameObject laserObj;
    private float laserDuration;

    public bool GetDoubleJumpEnable()
    {
        return isDouleJumpEnabled;
    }

    public float GetMoveSpeed()
    {
        return mMovementSpeed;
    }

    public float GetJumpStrength()
    {
        return mJumpStrength;
    }

    public float GetIFrameDistance()
    {
        return mIFrameDistance;
    }

    public void LaserAttack()
    {
        //if((mWeapon1 == null) && (mWeapon2 == null))
        //{
            laserObj.SetActive(true);
            laserDuration = 1.0f;
            mLaserDamage = 10;
            mCurrentHealth -= 10;
            Debug.Log("laser damage = 10, health - 10");
        //}
    }

    public void ObtainNewWeapon(Weapon newWeapon)
    {
        if(mWeapon1 == null)
        {
            mWeapon1 = newWeapon;
            mCurrentWeapon = mWeapon1;
        }
        else if(mWeapon2 == null)
        {
            mWeapon2 = newWeapon;
            mCurrentWeapon = mWeapon2;
        }
    }

    public void DropWeapon()
    {
        if ((mWeapon1 != null) || (mWeapon2 != null))
        {
            //                                 |  add a condition |                                                             //|--- [Mingzhuo Zhang] Edit: prevent trying drop NULL Weapon
            //                                 v                  v                                                             //|
            if ((mCurrentWeapon == mWeapon1) && (mWeapon1 != null))                                                             //|
            {
                Debug.Log("Weapon 1 dropped");
                mWeapon1.ThrowAway(new Vector3(gameObject.transform.right.x, gameObject.transform.up.y, 0.0f));
                mWeapon1 = null;
                mCurrentWeapon = null;
                if (mWeapon2 != null)                                                                                           //|--- [Mingzhuo Zhang] Edit: achieve automatic switch weapon
                    mCurrentWeapon = mWeapon2;                                                                                  //|
            }
            //                                 |  add a condition |                                                             //|--- [Mingzhuo Zhang] Edit: prevent trying drop NULL Weapon
            //                                 v                  v                                                             //|
            else if ((mCurrentWeapon == mWeapon2) && (mWeapon2 != null))                                                        //|
            {
                Debug.Log("Weapon 2 dropped");
                mWeapon2.ThrowAway(new Vector3(gameObject.transform.right.x, gameObject.transform.up.y, 0.0f));
                mWeapon2 = null;
                mCurrentWeapon = null;

                if (mWeapon1 != null)                                                                                           //|--- [Mingzhuo Zhang] Edit: achieve automatic switch weapon
                    mCurrentWeapon = mWeapon1;                                                                                  //|
            }
        }
        else
        {
            Debug.Log("There are no weapons to drop.");
        }
    }

    public void SwitchWeapon()
    {
        if(mCurrentWeapon == mWeapon1)
        {
            if(mWeapon2 != null)
            {
                mCurrentWeapon = mWeapon2;
                Debug.Log("Switch to Weapon 2");
            }
        }else if(mCurrentWeapon == mWeapon2)
        {
            if (mWeapon1 != null)
            {
                mCurrentWeapon = mWeapon1;
                Debug.Log("Switch to Weapon 1");
            }
        }
    }

    public float GetAttackSpeed()
    {
        return 1.0f;
            //mCurrentWeapon.AttackSpeed;
    }

    public bool Dodge()
    {
        if(mIFrameCD <= 0)
        {
            mIsIFrameOn = true;
            mIFrameDuration = 1;
            mIFrameCD = 4;
            return true;
        }
        return false;
    }

    override public void GetHit(float dmg)
    {
        Debug.Log("[Player] Player receives damage");
        if (mIsIFrameOn == false)
        {
            mCurrentHealth -= dmg;
        }

        if(mCurrentHealth <= 0)
        {
            Die();
        }
    }
    override public void Die()
    {
        Debug.Log("[Player] Player Dead, curr hp : " + mCurrentHealth + "max hp " + mMaxHealth);

        gameObject.SetActive(false);
    }

    public void Attack()
    {
        Debug.Log("[Player] Attack called");
        if ((mCurrentWeapon == null) && ((mWeapon1 != null) || (mWeapon2 != null)))
        {
            if(mWeapon1 != null)
            {
                mCurrentWeapon = mWeapon1;
                Debug.Log("Weapon 1 equipped");
                mCurrentWeapon.Attack();
            }
            else
            {
                mCurrentWeapon = mWeapon2;
                Debug.Log("Weapon 2 equipped");
                mCurrentWeapon.Attack();
            }
        }
        else if(mCurrentWeapon != null)
        {
            Debug.Log("current weapon attack");
            mCurrentWeapon.Attack();
        }
        else
        {
            Debug.Log("No Available weapon to attack.");
        }
    }

    new public void Awake()
    {
        base.Awake();
        mIsIFrameOn = true;
        mIFrameCD = 4.0f;
        mIFrameDuration = 1.0f;
        mMovementSpeed = 5.0f;
        mJumpStrength = 300.0f;
        mIFrameDistance = 40.0f;
        mLaserDamage = 0;
        isDouleJumpEnabled = true;
        laserObj.SetActive(false);

        //----------------------------------------------------------------------------------//|
        //- Mingzhuo Zhang Edit ------------------------------------------------------------//|
        //----------------------------------------------------------------------------------//|
        if (mWeapon1 != null)                                                               //|
        {                                                                                   //|
            mWeapon1.Picked(gameObject, gameObject.transform.position);                     //|    // second argument should be the [weapon position] as a individual variable in future
        }                                                                                   //|
        if (mWeapon2 != null)                                                               //|
        {                                                                                   //|
            mWeapon2.Picked(gameObject, gameObject.transform.position);                     //|    // second argument should be the [weapon position] as a individual variable in future
        }                                                                                   //|
                                                                                            //|
        //----------------------------------------------------------------------------------//|
        //- End Edit -----------------------------------------------------------------------//|
        //----------------------------------------------------------------------------------//|
    }

    new public void Update()
    {
        //[TOFIX] in player move controller.cs ,  GetKeyUp vec3 should be (0,0,0) rather than (-1, 0, 0 )
        base.Update();
        mIFrameCD -= Time.deltaTime;
        mIFrameDuration -= Time.deltaTime;
        laserDuration -= Time.deltaTime;
        if (mIFrameDuration <= 0)
        {
            mIsIFrameOn = false;
        }
        if(laserDuration <= 0)
        {
            laserObj.SetActive(false);
        }
    }

    public void LateUpdate()
    {
    }

    //----------------------------------------------------------------------------------//|
    //- Mingzhuo Zhang Edit ------------------------------------------------------------//|
    //----------------------------------------------------------------------------------//|
    public void PickWeaopn(Weapon newWeapon)                                            //|
    {                                                                                   //|
                                                                                        //|
        if (mWeapon1 == null)                                                           //|
        {                                                                               //|
            mWeapon1 = newWeapon;                                                       //|
            mWeapon1.Picked(gameObject, gameObject.transform.position);                 //|
            if (mCurrentWeapon == null)                                                 //|
                mCurrentWeapon = mWeapon1;                                              //|
        }                                                                               //|
        else if (mWeapon2 == null)                                                      //|--- [Mingzhuo Zhang] add a public function for pickUp weapon. This function will trigger by the collision of the weapon collider
        {                                                                               //|
            mWeapon2 = newWeapon;                                                       //|
            mWeapon2.Picked(gameObject, gameObject.transform.position);                 //|
            if (mCurrentWeapon == null)                                                 //|
                mCurrentWeapon = mWeapon2;                                              //|
        }                                                                               //|
                                                                                        //|
    }                                                                                   //|
    //----------------------------------------------------------------------------------//|
    //- End Edit -----------------------------------------------------------------------//|
    //----------------------------------------------------------------------------------//|
}