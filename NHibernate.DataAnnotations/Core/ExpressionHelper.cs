using System;
using System.Linq.Expressions;

namespace NHibernate.DataAnnotations.Core
{
    internal static class ExpressionHelper
    {
        internal static MemberExpression GetMemberInfoFromExpression(Expression method)
        {
            var lambda = method as LambdaExpression;
            if (ReferenceEquals(lambda, null)) throw new ArgumentNullException("method");
            MemberExpression memberExpr = null;
            switch (lambda.Body.NodeType)
            {
                case ExpressionType.Convert:
                    memberExpr = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
                    break;
                case ExpressionType.MemberAccess:
                    memberExpr = lambda.Body as MemberExpression;
                    break;
            }
            if (ReferenceEquals(memberExpr, null)) throw new ArgumentException("method");
            return memberExpr;
        }
    }
}