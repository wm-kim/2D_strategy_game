using System.Collections.Generic;

namespace Minimax.GamePlay.CommandSystem
{
    /// <summary>
    /// A command that is composed of multiple commands
    /// </summary>
    public class CompositeCmd : Command
    {
        private List<Command> m_subCommands = new List<Command>();
        
        public CompositeCmd() { }
        
        public CompositeCmd(List<Command> subCommands)
        {
            m_subCommands = subCommands;
        }
        
        protected void AddSubCommand(Command command)
        {
            m_subCommands.Add(command);
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