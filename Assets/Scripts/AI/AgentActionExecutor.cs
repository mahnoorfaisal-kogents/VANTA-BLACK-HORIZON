using System;

namespace Vanta.AI
{
    public sealed class AgentActionExecutor
    {
        public AgentAction LastAction { get; private set; } = AgentAction.Idle;
        public event Action<AgentAction> ActionExecuted;

        public bool TryExecute(AgentAction action, bool agentAlive)
        {
            if (!agentAlive) return false;
            LastAction = action;
            ActionExecuted?.Invoke(action);
            return true;
        }
    }
}