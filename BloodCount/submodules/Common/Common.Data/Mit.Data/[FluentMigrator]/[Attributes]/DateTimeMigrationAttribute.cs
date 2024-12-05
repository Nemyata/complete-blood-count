using FluentMigrator;

namespace Common.Data;

public class DateTimeMigrationAttribute : MigrationAttribute
{
    public DateTimeMigrationAttribute(long dateTime, string description)
        : base(dateTime, description)
    {
    }
    public DateTimeMigrationAttribute(long dateTime, string author, string description)
        : base(dateTime, CreateDescription(author, description))
    {
    }
    public DateTimeMigrationAttribute(int year, int month, int day, int hour, int minute, string author, string description)
        : base(CalculateValue(year, month, day, hour, minute), CreateDescription(author, description))
    {
    }

    private static long CalculateValue(int year, int month, int day, int hour, int minute)
    {
        return year * 100000000L + month * 1000000L + day * 10000L + hour * 100L + minute;
    }

    private static string CreateDescription(string author, string description)
    {
        return $"Migration: \"{description}\" created by: {author}";
    }
}