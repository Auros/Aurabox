using System;
using JetBrains.Annotations;

namespace Aurabox.VContainer
{
    [PublicAPI]
    public class ContextAlreadyInitializedException : Exception
    {
        public ContextAlreadyInitializedException() : base("This context already has a container.")
        {
            
        }
    }
}