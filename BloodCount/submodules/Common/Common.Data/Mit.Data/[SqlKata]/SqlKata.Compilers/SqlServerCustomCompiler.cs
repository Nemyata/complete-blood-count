using SqlKata;
using SqlKata.Compilers;

namespace Common.Data.SqlKata.Compilers;

public class SqlServerCustomCompiler : SqlServerCompiler
{
    protected virtual string CompileInRawCondition(SqlResult ctx, InRawCondition item)
    {
        string str1 = Wrap(item.Column);
        if (string.IsNullOrWhiteSpace(item.Value))
            return !item.IsNot ? "1 = 0 /* IN [empty list] */" : "1 = 1 /* NOT IN [empty list] */";
        string str2 = item.IsNot ? "NOT IN" : "IN";
        string str3 = item.Value;
        return str1 + " " + str2 + " (" + str3 + ")";
    }
}