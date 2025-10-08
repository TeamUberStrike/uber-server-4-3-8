using Cmune.Util;
using log4net;
using System.Diagnostics;

namespace Cmune.Realtime.Photon.Server.Diagnostics
{
    public class PhotonDebug : ICmuneDebug
    {
        private readonly ILog _myLog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //private readonly ILog _myLog = log4net.LogManager.GetLogger("Uberstrike");//System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Log(int level, string s)
        {
            //StackTrace trace = new StackTrace(3, true);
            //StackFrame f = trace.GetFrame(0);
            //s = string.Format("[{0}:{1}]\n{2}", f.GetFileName(), f.GetFileLineNumber(), s);

            switch (level)
            {
                case 0: if (_myLog.IsInfoEnabled) _myLog.Info(s); break;
                case 1: if (_myLog.IsWarnEnabled) _myLog.Warn(s); break;
                case 2: if (_myLog.IsErrorEnabled) _myLog.Error(s); break;
                default: _myLog.Info(s); break;
            }
        }
    }
}
