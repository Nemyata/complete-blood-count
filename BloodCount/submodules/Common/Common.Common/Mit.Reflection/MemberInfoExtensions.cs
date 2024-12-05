using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Common.Reflection
{
    public static class MemberInfoExtensions
    {
        #region Creation of Lambda expressions
        // Методы для создания Lambda-выражений, предоставляющих быстрый доступ к getter/setter'ам объектов типа PropertyInfo.
        // Оригинал заимствован отсюда: [c# - Is it possible to speed this method up? - Stack Overflow](https://stackoverflow.com/a/17669142/3905944)

        public static Func<T, object?> BuildUntypedGetter<T>(this MemberInfo memberInfo)
        {
            var targetType = memberInfo.DeclaringType;
            if (targetType == null)
                throw new ArgumentException($"Parameter {nameof(memberInfo)} should have {nameof(memberInfo.DeclaringType)} property defined.");

            var exInstance = Expression.Parameter(targetType, "t");

            var exMemberAccess = Expression.MakeMemberAccess(exInstance, memberInfo);       // t.PropertyName
            var exConvertToObject = Expression.Convert(exMemberAccess, typeof(object));     // Convert(t.PropertyName, typeof(object))
            var lambda = Expression.Lambda<Func<T, object?>>(exConvertToObject, exInstance);

            var action = lambda.Compile();
            return action;
        }
        public static Action<T, object?> BuildUntypedSetter<T>(this MemberInfo memberInfo)
        {
            var targetType = memberInfo.DeclaringType;
            if (targetType == null)
                throw new ArgumentException($"Parameter {nameof(memberInfo)} should have {nameof(memberInfo.DeclaringType)} property defined.");

            var exInstance = Expression.Parameter(targetType, "t");

            var exMemberAccess = Expression.MakeMemberAccess(exInstance, memberInfo);

            // t.PropertValue(Convert(p))
            var exValue = Expression.Parameter(typeof(object), "p");
            var exConvertedValue = Expression.Convert(exValue, memberInfo.GetUnderlyingType());
            var exBody = Expression.Assign(exMemberAccess, exConvertedValue);

            var lambda = Expression.Lambda<Action<T, object>>(exBody, exInstance, exValue);
            var action = lambda.Compile();
            return action;
        }

        private static Type GetUnderlyingType(this MemberInfo member)
        {
            string GetTargetTypePropertyName()
            {
                return member.MemberType switch
                {
                    MemberTypes.Event => nameof(EventInfo.EventHandlerType),
                    MemberTypes.Field => nameof(FieldInfo.FieldType),
                    MemberTypes.Method => nameof(MethodInfo.ReturnType),
                    MemberTypes.Property => nameof(PropertyInfo.PropertyType),
                    _ => string.Empty
                };
            }

            var type = member.MemberType switch
            {
                MemberTypes.Event => ((EventInfo)member).EventHandlerType,
                MemberTypes.Field => ((FieldInfo)member).FieldType,
                MemberTypes.Method => ((MethodInfo)member).ReturnType,
                MemberTypes.Property => ((PropertyInfo)member).PropertyType,
                _ => throw new ArgumentException("Input MemberInfo must be of type EventInfo, FieldInfo, MethodInfo or PropertyInfo")
            };

            if (type == null)
                throw new ArgumentException($"Parameter {nameof(member)} of type {nameof(member.MemberType)} should have corresponding property {GetTargetTypePropertyName()} with type of its target defined.");

            return type;
        }
        #endregion

        #region Getting of MemberInfo
        public static MemberExpression? GetMemberExpression(Expression memberExpression, bool required = false)
        {
            var me = memberExpression as MemberExpression;
            if (me == null && memberExpression is UnaryExpression { NodeType: ExpressionType.Convert } ue)
                me = ue.Operand as MemberExpression;

            if (me == null && required)
                throw new ArgumentException("No MemberExpression was found.", nameof(memberExpression));
            return me;
        }
        public static MemberExpression? GetMemberExpression<TSource, TPropType>(Expression<Func<TSource, TPropType>> memberExpression, bool required = false)
        {
            var me = memberExpression as MemberExpression;
            if (me == null && memberExpression is UnaryExpression { NodeType: ExpressionType.Convert } ue)
                me = ue.Operand as MemberExpression;
            else if (memberExpression.Body is MemberExpression bodyMemberExp)
                me = bodyMemberExp;
            else if (memberExpression.Body is UnaryExpression bodyUnaryExp)
                me = (MemberExpression)bodyUnaryExp.Operand;

            if (me == null && required)
                throw new ArgumentException("No MemberExpression was found.", nameof(memberExpression));
            return me;
        }

        /// <remarks>
        /// <see href="https://stackoverflow.com/questions/5015830/get-the-value-of-displayname-attribute">
        /// c# - get the value of DisplayName attribute - Stack Overflow</see></remarks>
        public static MemberInfo? GetPropertyOrFieldMemberInfo(Expression memberExpression, bool required = false)
        {
            var me = GetMemberExpression(memberExpression);
            if (me?.Member.MemberType is MemberTypes.Field or MemberTypes.Property)
                return me.Member;

            if (required)
                throw new ArgumentException("No property reference expression was found.", nameof(memberExpression));
            return null;
        }

        public static FieldInfo? GetFieldInfo<T>(Expression<Func<T, object>> fieldExpression)
        {
            var memberInfo = GetFieldInfo(fieldExpression.Body, true);
            return memberInfo;
        }
        public static FieldInfo? GetFieldInfo(Expression fieldExpression, bool required = false)
        {
            var me = GetMemberExpression(fieldExpression);
            return GetFieldInfo(me, nameof(fieldExpression), required);
        }
        public static FieldInfo? GetFieldInfo<TSource, TPropType>(Expression<Func<TSource, TPropType>> fieldExpression, bool required = false)
        {
            var me = GetMemberExpression(fieldExpression);
            return GetFieldInfo(me, nameof(fieldExpression), required);
        }
        private static FieldInfo? GetFieldInfo(MemberExpression? me, string fieldExpressionName, bool required = false)
        {
            if (me?.Member.MemberType is MemberTypes.Field)
                return (FieldInfo)me.Member;

            if (required)
                throw new ArgumentException("No field reference expression was found.", fieldExpressionName);
            return null;
        }

        public static PropertyInfo? GetPropertyInfo<T>(Expression<Func<T, object>> propertyExpression)
        {
            var memberInfo = GetPropertyInfo(propertyExpression.Body, true);
            return memberInfo;
        }
        public static PropertyInfo? GetPropertyInfo(Expression propertyExpression, bool required = false)
        {
            var me = GetMemberExpression(propertyExpression);
            return GetPropertyInfo(me, nameof(propertyExpression), required);
        }
        public static PropertyInfo? GetPropertyInfo<TSource, TPropType>(Expression<Func<TSource, TPropType>> propertyExpression, bool required = false)
        {
            var me = GetMemberExpression(propertyExpression);
            return GetPropertyInfo(me, nameof(propertyExpression), required);
        }
        private static PropertyInfo? GetPropertyInfo(MemberExpression? me, string propertyExpressionName, bool required = false)
        {
            if (me?.Member.MemberType is MemberTypes.Property)
                return (PropertyInfo)me.Member;

            if (required)
                throw new ArgumentException("No property reference expression was found.", propertyExpressionName);
            return null;
        }

        public static MemberInfo GetPropertyOrFieldInfo<T>(Expression<Func<T, object>> memberExpression)
        {
            var memberInfo = GetPropertyOrFieldInfo(memberExpression.Body, true)!;
            return memberInfo;
        }
        public static MemberInfo? GetPropertyOrFieldInfo(Expression memberExpression, bool required = false)
        {
            var me = GetMemberExpression(memberExpression);
            return GetPropertyOrFieldInfo(me, nameof(memberExpression), required);
        }
        public static MemberInfo? GetPropertyOrFieldInfo<TSource, TPropType>(Expression<Func<TSource, TPropType>> memberExpression, bool required = false)
        {
            var me = GetMemberExpression(memberExpression);
            return GetPropertyOrFieldInfo(me, nameof(memberExpression), required);
        }
        private static MemberInfo? GetPropertyOrFieldInfo(MemberExpression? me, string memberExpressionName, bool required = false)
        {
            if (me?.Member.MemberType is MemberTypes.Property)
                return (PropertyInfo)me.Member;
            if (me?.Member.MemberType is MemberTypes.Field)
                return (FieldInfo)me.Member;

            if (required)
                throw new ArgumentException("No property or field reference expression was found.", memberExpressionName);
            return null;
        }
        #endregion

        #region Reading attibutes of MemberInfo
        /// <remarks>
        /// <see href="https://stackoverflow.com/questions/5015830/get-the-value-of-displayname-attribute">
        /// c# - get the value of DisplayName attribute - Stack Overflow</see></remarks>
        public static T? GetAttribute<T>(this MemberInfo? member, bool required = false)
            where T : Attribute
        {
            var attribute = member.GetCustomAttributes(typeof(T), false).SingleOrDefault();

            if (attribute == null && required)
                throw new ArgumentException($"The '{typeof(T).Name}' attribute must be defined on member {member.Name}.");

            return attribute as T;
        }

        public static string GetPropertyOrFieldName<T>(Expression<Func<T, string>> memberExpression)
        {
            var memberInfo = GetPropertyOrFieldMemberInfo(memberExpression.Body, true)!;
            return memberInfo.Name;
        }

        /// <remarks>
        /// <see href="https://stackoverflow.com/questions/5015830/get-the-value-of-displayname-attribute">
        /// c# - get the value of DisplayName attribute - Stack Overflow</see>
        /// <br />
        /// Example: <c>string displayName = GetPropertyDisplayName&lt;UbiEmployee&gt;(i =&gt; i.Email);</c></remarks>
        public static string? GetPropertyOrFieldDisplayName<T>(Expression<Func<T, object>> memberExpression)
        {
            var memberInfo = GetPropertyOrFieldMemberInfo(memberExpression.Body, true)!;

            var attr = memberInfo.GetAttribute<DisplayAttribute>();
            return attr == null ? memberInfo.Name : attr.GetName();
        }
        public static string? GetPropertyOrFieldDisplayGroupName<T>(Expression<Func<T, object>> memberExpression)
        {
            var memberInfo = GetPropertyOrFieldMemberInfo(memberExpression.Body, true)!;

            var attr = memberInfo.GetAttribute<DisplayAttribute>();
            return attr == null ? memberInfo.Name : attr.GroupName;
        }
        #endregion
    }
}
