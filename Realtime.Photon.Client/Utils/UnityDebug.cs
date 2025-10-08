using System;
using Cmune.Realtime.Common;
using UnityEngine;
using Cmune.Util;

namespace Cmune.Realtime.Photon.Client.Utils
{
    public class UnityDebug : ICmuneDebug
    {
        public void Log(int level, string message)
        {
            string debugMessage = string.Format("[{0} {1}] {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), message);

            switch (level)
            {
                case 0: Debug.Log(debugMessage); break;
                case 1: Debug.LogWarning(debugMessage); break;
                case 2: Debug.LogError(debugMessage); break;
                default: Debug.Log(debugMessage); break;
            }
        }
    }

}
