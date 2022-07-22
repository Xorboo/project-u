using System;

namespace Utils.Execution
{
    [Flags]
    public enum ExecutionMode
    {
        None = 0,
        Awake = 1,
        Start = 2,

        // Note that AutoExecutionBehaviour doesn't check the flags below for performance reasons
        Update = 4,
        FixedUpdate = 8,
        LateUpdate = 16,
    }
}