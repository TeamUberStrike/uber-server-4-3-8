using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UberStrike.Core.Types;

namespace UberStrike.DataCenter.Common.Entities
{
    public class TutorialStepView
    {
        #region Properties

        public int Cmid { get; private set; }
        public TutorialStepType StepType { get; private set; }
        public DateTime StepTime { get; private set; }

        #endregion

        #region Constructors

        public TutorialStepView(int cmid, TutorialStepType stepType, DateTime stepTime)
        {
            Cmid = cmid;
            StepType = stepType;
            StepTime = stepTime;
        }

        #endregion
    }
}
