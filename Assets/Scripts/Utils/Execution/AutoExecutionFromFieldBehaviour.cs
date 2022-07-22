using UnityEngine;

namespace Utils.Execution
{
    public abstract class AutoExecutionFromFieldBehaviour : AutoExecutionBehaviour
    {
        [SerializeField]
        ExecutionMode ExecutionMode = ExecutionMode.Awake;

        protected override ExecutionMode Mode => ExecutionMode;
    }
}