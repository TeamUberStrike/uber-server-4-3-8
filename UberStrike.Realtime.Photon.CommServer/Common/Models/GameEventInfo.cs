//using System.Collections.Generic;
//using Cmune.Realtime.Common;

//namespace UberStrike.Realtime.Common
//{
//    public class GameEventInfo : CmuneDeltaSync
//    {
//        public GameEventInfo()
//        { }

//        public GameEventInfo(byte[] bytes, ref int idx)
//        {
//            idx = FromBytes(bytes, idx);
//        }

//        #region FIELDS
//        [CMUNESYNC(FieldTag.Pickups)]
//        private List<int> _pickups;
//        [CMUNESYNC(FieldTag.SplatCount)]
//        private int _splatCount;
//        #endregion

//        public new class FieldTag : CmuneDeltaSync.FieldTag
//        {
//            public const int Pickups = 1;
//            public const int SplatCount = 2;
//        }
//    }
//}
