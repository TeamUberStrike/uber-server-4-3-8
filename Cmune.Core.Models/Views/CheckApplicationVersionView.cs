
namespace Cmune.DataCenter.Common.Entities
{
    [System.Serializable]
    public class CheckApplicationVersionView
    {
        #region Properties

        public ApplicationView ClientVersion { get; set; }

        public ApplicationView CurrentVersion { get; set; }

        #endregion Properties

        #region Constructors

        public CheckApplicationVersionView() { }

        public CheckApplicationVersionView(ApplicationView clienVersion, ApplicationView currentVersion)
        {
            this.ClientVersion = clienVersion;
            this.CurrentVersion = currentVersion;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            string display = "[CheckApplicationVersionView: [ClientVersion: " + this.ClientVersion + "][CurrentVersion: " + this.CurrentVersion + "]]";

            return display;
        }

        #endregion Methods
    }
}