using System;

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

    public class ExpenseItem
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Expense { get; set; }
        public string Category { get; set; }
        public double Amount { get; set; }
    }

    public class ExpenseByCategory
    {
        public string Category { get; set; }
        public double TotalAmount { get; set; }
    }
}