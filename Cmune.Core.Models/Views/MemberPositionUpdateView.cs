
namespace Cmune.DataCenter.Common.Entities
{
    [System.Serializable]
    public class MemberPositionUpdateView
    {
        #region Properties

        public int GroupId { get; set; }
        
        public int CmidMakingAction { get; set; }
        
        public int MemberCmid { get; set; }
        
        public GroupPosition Position { get; set; }

        #endregion Properties

        #region Constructors

        public MemberPositionUpdateView()
        {
        }

        public MemberPositionUpdateView(int groupId, int cmidMakingAction, int memberCmid, GroupPosition position)
        {
            this.GroupId = groupId;
            this.CmidMakingAction = cmidMakingAction;
            this.MemberCmid = memberCmid;
            this.Position = position;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            string display = "[MemberPositionUpdateView: [GroupId:" + this.GroupId + "][CmidMakingAction:" + this.CmidMakingAction + "][MemberCmid:" + this.MemberCmid;
            display += "][Position:" + Position + "]]";

            return display;
        }


        #endregion Methods
    }
}
