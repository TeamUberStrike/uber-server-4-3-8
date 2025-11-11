
namespace UberStrike.UnitySdk
{
    public static class Api
    {
        private static readonly string _version = typeof(Api).Assembly.GetName().Version.ToString(3);

        /// <summary>
        /// 3 digit version string of the Cmune API [major, minor, revision]
        /// </summary>
        public static string Version
        {
            get { return _version; }
        }
    }
}