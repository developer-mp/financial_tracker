using System;

namespace FinancialTracker.Models
{
    public class DbConnection
    {
        public string Host { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DbName { get; set; } = string.Empty;
    }

    public class DbQuery
    {
        public string Query { get; set; } = string.Empty;
    }

    public class Category
    {
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryColor { get; set; } = string.Empty;
    }

    public class ExpenseItem
    {
        public string Id { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Expense { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double Amount { get; set; }
    }

    public class ExpenseByCategory
    {
        public string Category { get; set; } = string.Empty;
        public double TotalAmount { get; set; }
    }

    public class DefaultSorting
    {
        public string SortColumn { get; set; } = string.Empty;
        public string SortDirection { get; set; } = string.Empty;
    }

    public class ErrorMessage
    {
        public string Success { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }

    public class Filter
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Category { get; set; } = string.Empty;
        public double MinAmount { get; set; }
        public double MaxAmount { get; set; }
    }
}