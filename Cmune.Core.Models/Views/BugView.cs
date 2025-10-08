
namespace Cmune.DataCenter.Common.Entities
{
    [System.Serializable]
    public class BugView
    {
        #region Properties

        public string Content { get; set; }

        public string Subject { get; set; }

        #endregion Properties

        #region Constructors

        public BugView()
        {
        }

        public BugView(string subject, string content)
        {
            this.Subject = subject.Trim();
            this.Content = content.Trim();
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            string bugContent = "[Bug: [Subject: " + this.Subject + "][Content :" + this.Content + "]]";

            return bugContent;
        }

        #endregion Methods
    }
}