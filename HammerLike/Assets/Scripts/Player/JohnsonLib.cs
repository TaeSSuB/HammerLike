using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Johnson
{
    public enum eGizmoDir
    {
        Foward,
        Back,
        Right,
        Left,
        Up,
        Down,
        End
    }


    public static class Funcs
	{

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
                case eGizmoDir.Foward:
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
                case eGizmoDir.Foward:
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
                case eGizmoDir.Foward:
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
                case eGizmoDir.Foward:
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
