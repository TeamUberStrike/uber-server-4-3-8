using System.Collections;

namespace Cmune.Unity.Client
{
    public interface IPlayer
    {
        /// <summary>
        /// 
        /// </summary>
        string PlayerName { get; }

        /// <summary>
        /// 
        /// </summary>
        int ActorId { get; }

        /// <summary>
        /// 
        /// </summary>
        float SpeedChange { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool CanFly { get; set; }

        /// <summary>
        /// 
        /// </summary>
        short Health { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //sbyte TeamDegree { get; }

        /// <summary>
        /// 
        /// </summary>
        byte CurrentFireMode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        byte CurrentWeaponID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        void ShootWeapon();
    }
}