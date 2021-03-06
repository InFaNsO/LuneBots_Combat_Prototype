﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class WeaponMove
{
    public string name;
    [SerializeField] public bool mIsAirMove = false;
    [SerializeField] protected int mDamage;
    [System.NonSerialized] public float mAnimationPlayTime;
    [System.NonSerialized] public float mAttackSpendTime;
    [System.NonSerialized] public int mMoveID;
    [SerializeField] protected float mAttackSpeedMutiplier = 1.0f; // wait how many second to next attack
    protected float mAttackSpeedCounter;
    [SerializeField] MoveContext mMoveContext;
    [SerializeField] int[] mToMoveId; // Store the move id that this move can go to
    [SerializeField] protected Bullet mBullet;
    [SerializeField] protected Transform mFirePosition;
    [SerializeField] protected AnimationClip mAnimationClip;
    [SerializeField] private AudioSource _Audio;
    private Weapon mWeapon;
    
    [System.NonSerialized] public Animator mWeaponAnimator;

    private ElementalAttributes mElement;

    public bool isMelee = true;

    // Getter & Setter -------------------------------------------------------------------------------------------------------
    public MoveContext GetMoveContext() { return mMoveContext; }
    public float AttackSpeed { get { return mAttackSpendTime; } set { mAttackSpendTime = value; } }

    public void Load(Weapon weapon, Animator animator, int index, ElementalAttributes element)
    {
        mWeapon = weapon;
        mMoveID = index;

        Assert.IsNotNull(mBullet, "[Weapon] Dont have bullet");                                                                  //|--- [SAFTY]: Check to see is there a bullet prefeb
        Assert.AreNotEqual(mAttackSpeedMutiplier, 0.0f, "[Weapon] mAttackSpeedMutiplier not initialized");                       //|--- [SAFTY]: Check to see if parry context got initialized
        Assert.IsNotNull(mAnimationClip, "[Weapon] mAnimationClip not initialized");                                             //|--- [SAFTY]: Check to see if parry context got initialized

        mWeaponAnimator = animator;

        mAnimationPlayTime = mAnimationClip.length;
        mAttackSpendTime = mAnimationPlayTime / mAttackSpeedMutiplier;

        mMoveContext.Load(mAttackSpendTime, mToMoveId.Length, mMoveID);

        mElement = element;                 // Take this line out if want each move has its own element
        Assert.IsNotNull(mElement);
    }

    public void Enter()
    {
        mWeapon.mIsAttacking = true;

        mWeaponAnimator.SetInteger("ToNextCombo", mMoveID);
        mWeaponAnimator.SetFloat("PlaySpeed", mAttackSpeedMutiplier);
        mWeaponAnimator.SetBool("IsReseting", false);
        Core.Debug.Log($"{mAttackSpeedMutiplier}, {mMoveID}");
        mMoveContext.Active = true;
        if (mWeapon.mComboBar)
        {
            mWeapon.mComboBar.Bind(GetMoveContext());
        }

        _Audio.Play();
    }

    public void Exit()
    {
        mWeapon.mIsAttacking = false;
        //mWeaponAnimator.SetInteger("ToNextCombo", mToMoveId[ mMoveContext.GetTransitionSliceCount()]);
        mWeaponAnimator.SetFloat("PlaySpeed", 1.0f);
        mMoveContext.Reset();
        if (mWeapon.mComboBar)
        {
            mWeapon.mComboBar.UnBind();
        }

    }

    public void Update(float deltaTime)
    {
        bool moveContextStatus = mMoveContext.Active;
        mMoveContext.Update(Time.deltaTime);
        if (mMoveContext.Active != moveContextStatus && !mMoveContext.Active)
        {
            if (mWeapon.mComboBar)
            {
                mWeapon.mComboBar.UnBind();
            }
        }


        if (mMoveContext.Active)
        {
            Vector3 attackMomentum = mWeapon.mOwner.mAttackMomentumPos.position - mWeapon.mOwner.transform.position;
            mWeapon.mOwner.transform.position += attackMomentum * deltaTime;

            if (mMoveContext.GetCurrentTimeSliceType() == MoveTimeSliceType.Type_Parryable)
            {
                //TODO :: make weapon become parrable
            }
            else if ((mMoveContext.GetCurrentTimeSliceType() == MoveTimeSliceType.Type_DealtDmg) && (mMoveContext.mHaveDealtDmg == false))
            {
                ShootBullet();
                mMoveContext.mHaveDealtDmg = true;
                //gameObject.GetComponent<MeshRenderer>().materials[0].color = Color.red;
            }
        }
    }

    public bool IsFinish()
    {
        return (!mMoveContext.Active);
    }

    public void ShootBullet()
    {
        Bullet newBullet = GameObject.Instantiate(mBullet, new Vector3(0, 0, 0), Quaternion.identity);
        Vector3 shootPos = mWeapon.transform.TransformPoint(mWeapon.transform.localPosition + mFirePosition.localPosition);
        if (mWeapon.mTargetPos.Equals(Vector3.negativeInfinity))
        {
            mWeapon.mTargetPos = mWeapon.transform.position + mWeapon.transform.right;
        }
        Vector3 dir = Vector3.Normalize(mWeapon.mTargetPos - mWeapon.transform.position);
        newBullet.Fire(mWeapon.tag, mDamage, shootPos, dir, isMelee ? WeaponType.Melee : WeaponType.Range);
        newBullet.Awake();
        newBullet.mElement = mWeapon.mOwnerElement + mElement;
        Core.Debug.Log(newBullet.mElement.ToString() + newBullet.mElement.fire);

        Debug.Log("attacked");
        Core.Debug.Log($"whichMove: {mMoveID} Typed: {GetMoveCurrentTimeSliceType()}");
    }

    public MoveTimeSliceType GetMoveCurrentTimeSliceType()
    {
        return mMoveContext.GetCurrentTimeSliceType();
    }

    public int GetNextTransitionMoveIndex()
    {
        return mToMoveId[mMoveContext.GetTransitionSliceCount()];
    }

    public bool IsActive()
    {
        return mMoveContext.Active;
    }

    public void Reset()
    {
        mMoveContext.Reset();
        
    }

    public void RefreshAnimator(Animator animator)
    {
        mWeaponAnimator = animator;
    }
}
