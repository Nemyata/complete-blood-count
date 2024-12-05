using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace Common.Data
{
    public class DapperToolkit
    {
        private const string WhereSqlClause = @"
WHERE {0}";

        public static void BuildWhereClause(dynamic filter, Dictionary<string, string>? columnMapping, out string? whereClause, out string? parameterEnumeration, bool any)
        {
            var whereReadableParams = new List<string>();
            whereClause = null;
            parameterEnumeration = null;

            var whereOperands = new List<string>();

            foreach (KeyValuePair<string, object> kvp in filter)
            {
                var key = kvp.Key;
                var value = kvp.Value;

                var columnName = (columnMapping != null && columnMapping.TryGetValue(key, out var value1))
                    ? value1
                    : key.CapitalizeFirstLetter();
                whereOperands.Add($"[{columnName}] = @{key}");

                var nameSplit = key.SplitByCapitalizedLetters()
                    .Select(e => e.ToLower());
                var nameTitle = string.Join(" ", nameSplit.ToArray());
                whereReadableParams.Add($"{nameTitle} is '{value}'");
            }

            if (whereOperands.Count > 0)
            {
                var operand = any ? "OR" : "AND";
                var filterValue = string.Join($"\n    {operand} ", whereOperands.ToArray());
                whereClause = string.Format(WhereSqlClause, filterValue);
            }


            if (whereReadableParams.Count < 1)
                return;

            parameterEnumeration = " whose ";
            if (any)
            {
                parameterEnumeration += string.Join(", or ", whereReadableParams.ToArray());
            }
            else
            {
                whereReadableParams = whereReadableParams.Select((e, i) => (i > 0 ? (i < (whereReadableParams.Count - 1) ? ", " : ", and ") : "") + e).ToList();
                parameterEnumeration += string.Join("", whereReadableParams);
            }
        }
    }
}