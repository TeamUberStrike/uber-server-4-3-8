using System;
using ExitGames.Concurrency.Fibers;

namespace Uberstrike.Realtime.Server.Base
{
    public abstract class FiberBase : IDisposable
    {
        public bool IsDisposed { get; private set; }

        protected PoolFiber fiber;

        public FiberBase()
        {
            fiber = new PoolFiber();
            fiber.Start();
        }

        #region IDispoable

        ~FiberBase()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
            this.IsDisposed = true;

            if (dispose)
            {
                fiber.Dispose();
            }
        }

        #endregion
    }
}