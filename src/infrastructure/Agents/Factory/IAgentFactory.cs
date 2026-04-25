using Microsoft.Agents.AI;
using System;
using System.Collections.Generic;
using System.Text;

namespace infrastructure.Agents.Factory
{
    internal interface IAgentFactory
    {
        AIAgent Create();
    }
}
