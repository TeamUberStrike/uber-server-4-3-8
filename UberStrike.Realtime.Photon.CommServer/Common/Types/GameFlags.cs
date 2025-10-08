using Cmune.Realtime.Common;
using UberStrike.Core.Types;

namespace UberStrike.Realtime.Common
{
    public class GameFlags
    {
        [System.Flags]
        public enum GAME_FLAGS : int
        {
            None = BIT_FLAGS.BIT_NONE,
            LowGravity = BIT_FLAGS.BIT_01,
            Instakill = BIT_FLAGS.BIT_02,
            NinjaArena = BIT_FLAGS.BIT_03,
            SniperArena = BIT_FLAGS.BIT_04,
            CannonArena = BIT_FLAGS.BIT_05
        }

        private GAME_FLAGS gameFlags = GAME_FLAGS.None;

        public bool LowGravity { get { return IsFlagSet(GAME_FLAGS.LowGravity); } set { SetFlag(GAME_FLAGS.LowGravity, value); } }
        public bool Instakill { get { return IsFlagSet(GAME_FLAGS.Instakill); } set { SetFlag(GAME_FLAGS.Instakill, value); } }
        public bool NinjaArena { get { return IsFlagSet(GAME_FLAGS.NinjaArena); } set { SetFlag(GAME_FLAGS.NinjaArena, value); } }
        public bool SniperArena { get { return IsFlagSet(GAME_FLAGS.SniperArena); } set { SetFlag(GAME_FLAGS.SniperArena, value); } }
        public bool CannonArena { get { return IsFlagSet(GAME_FLAGS.CannonArena); } set { SetFlag(GAME_FLAGS.CannonArena, value); } }

        public int ToInt()
        {
            return (int)gameFlags;
        }

        public static bool IsFlagSet(GAME_FLAGS flag, int state)
        {
            return (state & (int)flag) != 0;
        }

        private bool IsFlagSet(GAME_FLAGS f)
        {
            return (gameFlags & f) == f;
        }

        private void SetFlag(GAME_FLAGS f, bool b)
        {
            gameFlags = b ? gameFlags | f : gameFlags & ~f;
        }

        public void SetFlags(int flags)
        {
            gameFlags = (GAME_FLAGS)flags;
        }

        public void ResetFlags()
        {
            gameFlags = 0;
        }
    }
}