using UnityEngine;

namespace Utils.Execution
{
    public abstract class AutoExecutionBehaviour : MonoBehaviour
    {
        protected abstract ExecutionMode Mode { get; }


        #region Unity

        protected virtual void Awake()
        {
            if (Mode.HasFlag(ExecutionMode.Awake))
                Execute();
        }

        protected virtual void Start()
        {
            if (Mode.HasFlag(ExecutionMode.Start))
                Execute();
        }

        #endregion

        protected abstract void Execute();
    }
}