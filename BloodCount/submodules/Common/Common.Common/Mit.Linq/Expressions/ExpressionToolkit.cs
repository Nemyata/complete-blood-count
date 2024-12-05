using System.Linq.Expressions;

namespace Common.Linq.Expressions
{
    public static class ExpressionToolkit
    {
        /// <remarks>Taken from <see href="https://stackoverflow.com/a/2088849/3905944">
        /// c# - Expression.GreaterThan fails if one operand is nullable type, other is non-nullable - Stack Overflow</see></remarks>
        public static void ConvertToSameNullability(ref Expression left, ref Expression right)
        {
            if (left.Type.IsNullable() && !right.Type.IsNullable())
                right = Expression.Convert(right, left.Type);
            else if (!left.Type.IsNullable() && right.Type.IsNullable())
                left = Expression.Convert(left, right.Type);
        }
    }
}
