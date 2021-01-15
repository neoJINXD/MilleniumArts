using System.Collections.Generic;
using UnityEngine;

// Author: Anik Patel
// Description: Invoker Parent class to run commands from
// Version: 1.0
// Changes: [N/A]
namespace Zone.Core.Utils
{
    public class Invoker<T> : MonoBehaviour where T: ICommand
    {
        private static Queue<T> buffer;

        void Awake()
        {
            buffer = new Queue<T>();
        }

        // Base update that just runs all the commands given by default
        protected virtual void Update()
        {
            while (buffer.Count != 0)
            {
                buffer.Dequeue().Execute();
            }
        }

        public static void AddCommand(T command)
        {
            buffer.Enqueue(command);
        }
    }
}
