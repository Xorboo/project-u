using System;
using Core.Player;

namespace Core.Triggers
{
    public abstract class ImmediatePlayerTriggerBase : PlayerTriggerBase
    {
        public abstract void ImmediateProcessTrigger(Action onCompleted);
    }
}