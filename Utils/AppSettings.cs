namespace FinancialTracker.Utils
{
    public class DbSettings
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DbName { get; set; }
    }

    public class QuerySettings
    {
        public string Query { get; set; }
    }

    public class CategorySettings
    {
        public string CategoryName { get; set; }
        public string CategoryColor { get; set; }
    }
}