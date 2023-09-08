using System.Collections.Generic;
using Minimax.Utilities;

namespace Minimax.GamePlay.CommandSystem
{
    /// <summary>
    /// Server에서 실행되는 Logic에 따라 client쪽에서 시각적인 표현을 정확하고 순차적으로 반영하기 위해 Command Pattern을 활용
    /// </summary>
    public class Command
    {
        public static Queue<Command> CommandQueue = new Queue<Command>();
        public static bool IsExecuting = false;
        
        public void AddToQueue()
        {
            CommandQueue.Enqueue(this);
            if (!IsExecuting)
                PlayFirstCommandFromQueue();
        }
        
        public virtual void StartExecute()
        {
            DebugWrapper.Log($"Start Execute Command {this.GetType()}");
            // use tween sequence and call ExecutionComplete() in OnComplete()
        }
        
        /// <summary>
        /// Call when the command is finished executing.
        /// If there are more commands in the queue, play the next one.
        /// </summary>
        public static void ExecutionComplete()
        {
            if (CommandQueue.Count > 0)
                PlayFirstCommandFromQueue();
            else
            {
                IsExecuting = false;
            }
        }
        
        public static void PlayFirstCommandFromQueue()
        {
            IsExecuting = true;
            CommandQueue.Dequeue().StartExecute();
        }
    }
}