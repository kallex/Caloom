namespace TheBall.CORE
{
    public interface IAccountInfo
    {
        string AccountID { get; }
        string AccountName { get; }
        string AccountEmail { get; }
    }

    public class CoreAccountData : IAccountInfo
    {
        public string AccountID { get; private set; }
        public string AccountName { get; private set; }
        public string AccountEmail { get; private set; }
        public CoreAccountData(string accountID, string accountName, string accountEmail)
        {
            AccountID = accountID;
            AccountName = accountName;
            AccountEmail = accountEmail;
        }
    }
}