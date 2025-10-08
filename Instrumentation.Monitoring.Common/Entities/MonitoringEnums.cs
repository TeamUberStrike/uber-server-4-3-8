namespace Cmune.Instrumentation.Monitoring.Common.Entities
{
    public enum MonitoringOperationResult
    {
        InvalidName = 0,
        DuplicateName = 1,
        TestNotFound = 2,
        Ok = 3,
        InvalidIP = 4,
        TestInvalidConnectionString = 5,
        TestInvalidSqlQuery = 6,
        TestInvalidUrl = 7,
        TestInvalidPassPhrase = 8,
        TestInvalidInitVector = 9,
        TestInvalidPostContent = 10,
        TestInvalidPostField = 11,
        TestInvalidSocket = 13,
        ServerNotFound = 14,
        DuplicateReport = 15
    }
}