using System;

namespace Aurabox.VContainer
{
    public class InvalidSceneContextPlacement : Exception
    {
        public InvalidSceneContextPlacement() : base("The SceneContext is not placed in the root of the scene!")
        {
            
        }
    }
}