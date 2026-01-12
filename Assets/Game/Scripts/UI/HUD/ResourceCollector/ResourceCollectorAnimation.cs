using System;


namespace UI.HUD.ResourceCollector
{
    public class ResourceCollectorAnimation
    {
        public bool Completed { get; private set; } = false;

        public void Complete()
        {
            if (Completed)
                return;

            Completed = true;
        }
    }
}
