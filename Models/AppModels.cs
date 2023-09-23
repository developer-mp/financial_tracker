using System;

namespace FinancialTracker.Models
{
    public class DbConnection
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DbName { get; set; }
    }

    public class DbQuery
    {
        public string Query { get; set; }
    }

    public class Category
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

    public class DefaultSorting
    {
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
    }

    public class ErrorMessage
    {
        public string Success { get; set; }
        public string Error { get; set; }
    }

    public class Filter
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Category { get; set; }
        public double MinAmount { get; set; }
        public double MaxAmount { get; set; }
    }
}