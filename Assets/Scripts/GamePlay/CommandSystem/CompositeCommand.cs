using System.Collections.Generic;

namespace Minimax.GamePlay.CommandSystem
{
    /// <summary>
    /// A command that is composed of multiple commands
    /// </summary>
    public class CompositeCommand : Command
    {
        private List<Command> m_subCommands = new List<Command>();
        
        public CompositeCommand(List<Command> subCommands)
        {
            m_subCommands = subCommands;
        }
        
        public override void StartExecute()
        {
            base.StartExecute();
            foreach (var subCommand in m_subCommands)
            {
                subCommand.AddToQueue();
            }
        }
    }
}