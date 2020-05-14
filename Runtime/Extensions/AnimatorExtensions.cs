using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolbox.Runtime.Extensions
{
    public static class AnimatorExtensions
    {
        public static bool HasParameterOfType(this Animator animator, string name, AnimatorControllerParameterType type)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            
            var parameters = animator.parameters;
            return parameters.Any(currParam => currParam.type == type && currParam.name == name);
        }

        public static void AddAnimatorParameterIfExists(Animator animator, string parameterName, out int parameter,
            AnimatorControllerParameterType type, List<int> parameterList)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                parameter = -1;
                return;
            }

            parameter = Animator.StringToHash(parameterName);

            if (animator.HasParameterOfType(parameterName, type))
            {
                parameterList.Add(parameter);
            }
        }
        
        public static void AddAnimatorParameterIfExists(Animator animator, string parameterName, AnimatorControllerParameterType type, List<string> parameterList)
        {
            if (animator.HasParameterOfType(parameterName, type))
            {
                parameterList.Add(parameterName);
            }
        }
        
        public static void UpdateAnimatorBool(Animator animator, string parameterName, bool value)
        {
            animator.SetBool(parameterName, value);
        }
        
        public static void UpdateAnimatorBool(Animator animator, int parameter, bool value, List<int> parameterList)
        {
            if (parameterList.Contains(parameter))
            {
                animator.SetBool(parameter, value);
            }
        }
        
        public static void UpdateAnimatorBool(Animator animator, string parameterName, bool value, List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetBool(parameterName, value);
            }
        }
        
        public static void UpdateAnimatorTrigger(Animator animator, int parameter, List<int> parameterList)
        {
            if (parameterList.Contains(parameter))
            {
                animator.SetTrigger(parameter);
            }
        }
        
        public static void UpdateAnimatorTrigger(Animator animator, string parameterName, List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetTrigger(parameterName);
            }
        }
        
        public static void SetAnimatorTrigger(Animator animator, int parameter, List<int> parameterList)
        {
            if (parameterList.Contains(parameter))
            {
                animator.SetTrigger(parameter);
            }
        }
        
        public static void SetAnimatorTrigger(Animator animator, string parameterName, List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetTrigger(parameterName);
            }
        }
        
        public static void UpdateAnimatorFloat(Animator animator, string parameterName, float value)
        {
            animator.SetFloat(parameterName, value);
        }
        
        public static void UpdateAnimatorFloat(Animator animator, int parameter, float value, List<int> parameterList)
        {
            if (parameterList.Contains(parameter))
            {
                animator.SetFloat(parameter, value);
            }
        }
        
        public static void UpdateAnimatorFloat(Animator animator, string parameterName, float value, List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetFloat(parameterName, value);
            }
        }
        
        public static void UpdateAnimatorInteger(Animator animator, string parameterName, int value)
        {
            animator.SetInteger(parameterName, value);
        }
        
        public static void UpdateAnimatorInteger(Animator animator, int parameter, int value, List<int> parameterList)
        {
            if (parameterList.Contains(parameter))
            {
                animator.SetInteger(parameter, value);
            }
        }
        
        public static void UpdateAnimatorInteger(Animator animator, string parameterName, int value, List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetInteger(parameterName, value);
            }
        }
        
        public static void UpdateAnimatorBoolIfExists(Animator animator, string parameterName, bool value)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Bool))
            {
                animator.SetBool(parameterName, value);
            }
        }
        
        public static void UpdateAnimatorTriggerIfExists(Animator animator, string parameterName)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger(parameterName);
            }
        }
        
        public static void SetAnimatorTriggerIfExists(Animator animator, string parameterName)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger(parameterName);
            }
        }
        
        public static void UpdateAnimatorFloatIfExists(Animator animator, string parameterName, float value)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Float))
            {
                animator.SetFloat(parameterName, value);
            }
        }
        
        public static void UpdateAnimatorIntegerIfExists(Animator animator, string parameterName, int value)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Int))
            {
                animator.SetInteger(parameterName, value);
            }
        }
    }
}