using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Core.ViewModel
{
    [System.Serializable]
    public class RegisterClientApplicationViewModel
    {
        public ApplicationRegistrationResult Result { get; set; }
        public ICollection<int> ItemsAttributed { get; set; }
    }
}
