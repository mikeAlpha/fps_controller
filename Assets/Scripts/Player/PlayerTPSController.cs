using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerTPSController : MonoBehaviour
{
    public Transform RightHandRef;
    public Transform RightElbowRef;
    public Transform LeftHandRef;
    public Transform LeftElbowRef;

    public Transform ShoulderIK;
    public Transform ShoulderBone;


    public Transform LookAtPosition;
    public float RighthandWeight = 1.0f;
    public float LefthandWeight = 1.0f;

    private GameObject rsp;

    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        InitIK();
    }

    private void Update()
    {
        HandleShoulder();
    }


    private void OnAnimatorIK(int layerIndex)
    {
        SolveIK();
    }

    void InitIK()
    {
        rsp = new GameObject("IKHandler");
        rsp.transform.parent = transform;
    }

    void HandleShoulder()
    {
        ShoulderBone.LookAt(LookAtPosition);
        rsp.transform.position = ShoulderBone.TransformPoint(Vector3.zero);
        ShoulderIK.transform.position = rsp.transform.position;

        ShoulderIK.LookAt(LookAtPosition);
    }

    void ResetWeights(bool val)
    {
        StartCoroutine(ResetWeightInternal(val));
    }

    IEnumerator ResetWeightInternal(bool val)
    {
        yield return new WaitForSeconds(0.3568f);
        if (!val)
        {
            RighthandWeight = LefthandWeight = 1.0f;
        }
        else
        {
            RighthandWeight = LefthandWeight = 0.0f;
        }
    }

    //void HandleIKAnimationState()
    //{
    //    if(hX == 0 && hZ == 0)
    //    {
    //        RightHandRef.position = Idle.RightHandRef;
    //        LeftHandRef.position = Idle.LeftHandRef;
    //        RightElbowRef.position = Idle.RightElbowRef;
    //        LeftElbowRef.position = Idle.LeftElbowRef;
    //    }
    //    else
    //    {
    //        RightHandRef.position = Run.RightHandRef;
    //        LeftHandRef.position = Run.LeftHandRef;
    //        RightElbowRef.position = Run.RightElbowRef;
    //        LeftElbowRef.position = Run.LeftElbowRef;
    //    }
    //}

    void SolveIK()
    {
        anim.SetIKPosition(AvatarIKGoal.RightHand, RightHandRef.position);
        anim.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandRef.position);

        anim.SetIKRotation(AvatarIKGoal.RightHand, RightHandRef.rotation);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandRef.rotation);

        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, RighthandWeight);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, LefthandWeight);

        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, RighthandWeight);
        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, LefthandWeight);

        anim.SetIKHintPosition(AvatarIKHint.RightElbow, RightElbowRef.position);
        anim.SetIKHintPosition(AvatarIKHint.LeftElbow, LeftElbowRef.position);
        anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, RighthandWeight);
        anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, LefthandWeight);

        anim.SetLookAtPosition(LookAtPosition.position);
        anim.SetLookAtWeight(1.0f, 1.0f, 1.0f, 1.0f);
    }

    //public void UpdateIKRef(PlayerSettings CurAnimSetting)
    //{
    //    RightHandRef.localPosition = CurAnimSetting.RightHandPosRef;
    //    LeftHandRef.localPosition = CurAnimSetting.LeftHandPosRef;
    //    RightElbowRef.localPosition = CurAnimSetting.RightElbowPosRef;
    //    LeftElbowRef.localPosition = CurAnimSetting.LeftElbowPosRef;

    //    RightHandRef.localRotation = Quaternion.Euler(CurAnimSetting.RightHandRotRef);
    //    LeftHandRef.localRotation = Quaternion.Euler(CurAnimSetting.LeftHandRotRef);

    //    RighthandWeight = CurAnimSetting.RightHandWeight;
    //    LefthandWeight = CurAnimSetting.LeftHandWeight;
    //}
}
