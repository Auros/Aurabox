using System;
using JetBrains.Annotations;

namespace Aurabox.VContainer
{
    [PublicAPI]
    public class ContextNotInitializedException : Exception
    {
        public ContextNotInitializedException() : base("The container is not initialized.")
        {
            
        }       
    }
}