using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Johnson
{
    public enum eGizmoDir
    {
        Forward,
        Back,
        Right,
        Left,
        Up,
        Down,
        End
    }


    public static class Defines
    {
        public const float minWidth = 1280f;
        public const float minHeight = 720f;

        //public const float minWidth = 800;
        //public const float minHeight = 600f;


        public static Vector2[] ViewportPos =
{
        new Vector2(0,1),	//LT
		new Vector2(1,1),	//RT
		new Vector2(1,0),	//RB
		new Vector2(0,0),//LB
		new Vector2(0.5f, 0.5f) //Center
	};

    }


    public static class Funcs
	{

        public static string GetEnumName<T>(int index) where T : struct, IConvertible
        {//where 조건 struct, IConvertible => Enum으로 제한
            return Enum.GetName(typeof(T), index);
        }



        public static bool IsAnimationAlmostFinish(Animator animCtrl, string animationName, float ratio = 0.95f)
        {
            if (animCtrl.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            {//여기서 IsName은 애니메이션클립 이름이 아니라 애니메이터 안에 있는 노드이름임
                if (animCtrl.GetCurrentAnimatorStateInfo(0).normalizedTime >= ratio)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsAnimationCompletelyFinish(Animator animCtrl, string animationName, float ratio = 1.0f)
        {
            if (animCtrl.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            {//여기서 IsName은 애니메이션클립 이름이 아니라 애니메이터 안에 있는 노드이름임
                if (animCtrl.GetCurrentAnimatorStateInfo(0).normalizedTime >= ratio)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsAnimationPlay(Animator animCtrl, string animationName, int animationLayer)
        {
            if (animCtrl.GetCurrentAnimatorStateInfo(animationLayer).IsName(animationName))
            {//여기서 IsName은 애니메이션클립 이름이 아니라 애니메이터 안에 있는 노드이름임
                return true;
            }
            return false;
        }


        #region SpeciticBone Move
        public static void LookAtSpecificBone(Animator animCtrl, HumanBodyBones boneName, Transform targetTr, Vector3 offsetEulerRotate)
        {
            Transform boneTr = animCtrl.GetBoneTransform(boneName);
            boneTr.LookAt(targetTr);
            boneTr.rotation = boneTr.rotation * Quaternion.Euler(offsetEulerRotate);
        }

        //본 마다 로컬 방향이 조금씩 달라서 보정해주기 위해서 Dir값 넣어주는거.
        //하나씩 넣어서 테스트 해보는 수밖에 없는 듯
        public static void LookAtSpecificBone(Transform boneTr, Transform targetTr, eGizmoDir boneDir)
        {
            Vector3 lookDir = (targetTr.position - boneTr.position).normalized;

            switch (boneDir)
            {
                case eGizmoDir.Forward:
                    {
                        boneTr.forward = lookDir;
                    }
                    break;
                case eGizmoDir.Back:
                    {
                        boneTr.forward = -lookDir;
                    }
                    break;
                case eGizmoDir.Right:
                    {
                        boneTr.right = lookDir;
                    }
                    break;
                case eGizmoDir.Left:
                    {
                        boneTr.right = -lookDir;
                    }
                    break;
                case eGizmoDir.Up:
                    {
                        boneTr.up = lookDir;
                    }
                    break;
                case eGizmoDir.Down:
                    {
                        boneTr.up = -lookDir;
                    }
                    break;

                default:
                    {
                        Debug.Log("Enemy bone LookAt Error");
                    }
                    break;
            }
        }

        public static void LookAtSpecificBone(Transform boneTr, Transform targetTr, eGizmoDir boneDir, Vector3 offsetEulerRotate)
        {
            //Vector3 lookDir = (boneTr.position - targetTr.position).normalized;

            Vector3 lookDir = (targetTr.position - boneTr.position).normalized;

            switch (boneDir)
            {
                case eGizmoDir.Forward:
                    {
                        boneTr.forward = lookDir;
                    }
                    break;
                case eGizmoDir.Back:
                    {
                        boneTr.forward = -lookDir;
                    }
                    break;
                case eGizmoDir.Right:
                    {
                        boneTr.right = lookDir;
                    }
                    break;
                case eGizmoDir.Left:
                    {
                        boneTr.right = -lookDir;
                    }
                    break;
                case eGizmoDir.Up:
                    {
                        boneTr.up = lookDir;
                    }
                    break;
                case eGizmoDir.Down:
                    {
                        boneTr.up = -lookDir;
                    }
                    break;

                default:
                    {
                        Debug.Log("Enemy bone LookAt Error");
                    }
                    break;
            }

            boneTr.rotation = boneTr.rotation * Quaternion.Euler(offsetEulerRotate);
        }

        public static void LookAtSpecificBone(Transform boneTr, Vector3 targetPos, eGizmoDir boneDir, Vector3 offsetEulerRotate)
        {

            Vector3 lookDir = (targetPos - boneTr.position).normalized;

            switch (boneDir)
            {
                case eGizmoDir.Forward:
                    {
                        boneTr.forward = lookDir;
                    }
                    break;
                case eGizmoDir.Back:
                    {
                        boneTr.forward = -lookDir;
                    }
                    break;
                case eGizmoDir.Right:
                    {
                        boneTr.right = lookDir;
                    }
                    break;
                case eGizmoDir.Left:
                    {
                        boneTr.right = -lookDir;
                    }
                    break;
                case eGizmoDir.Up:
                    {
                        boneTr.up = lookDir;
                    }
                    break;
                case eGizmoDir.Down:
                    {
                        boneTr.up = -lookDir;
                    }
                    break;

                default:
                    {
                        Debug.Log("Enemy bone LookAt Error");
                    }
                    break;
            }

            boneTr.rotation = boneTr.rotation * Quaternion.Euler(offsetEulerRotate);
        }

        public static void LookAtSpecificBone(Transform boneTr, eGizmoDir boneDir, Vector3 lookDir, Vector3 offsetEulerRotate)
        {

            //Vector3 lookDir = (targetPos - boneTr.position).normalized;

            switch (boneDir)
            {
                case eGizmoDir.Forward:
                    {
                        boneTr.forward = lookDir;
                    }
                    break;
                case eGizmoDir.Back:
                    {
                        boneTr.forward = -lookDir;
                    }
                    break;
                case eGizmoDir.Right:
                    {
                        boneTr.right = lookDir;
                    }
                    break;
                case eGizmoDir.Left:
                    {
                        boneTr.right = -lookDir;
                    }
                    break;
                case eGizmoDir.Up:
                    {
                        boneTr.up = lookDir;
                    }
                    break;
                case eGizmoDir.Down:
                    {
                        boneTr.up = -lookDir;
                    }
                    break;

                default:
                    {
                        Debug.Log("Enemy bone LookAt Error");
                    }
                    break;
            }

            boneTr.rotation = boneTr.rotation * Quaternion.Euler(offsetEulerRotate);
        }
        #endregion
    }



}
