using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WSB.Activity.Common
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, string propertyName)
        {
            return QueryableHelper<T>.OrderBy(queryable, propertyName, false);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, string propertyName, bool isAsc)
        {
            return QueryableHelper<T>.OrderBy(queryable, propertyName, isAsc);
        }

        static class QueryableHelper<T>
        {
            private static Dictionary<string, LambdaExpression> cache = new Dictionary<string, LambdaExpression>();

            private static LambdaExpression GetLambdaExpression(string propertyName)
            {
                if (cache.ContainsKey(propertyName))
                {
                    return cache[propertyName];
                }
                var param = Expression.Parameter(typeof(T));
                var body = Expression.Property(param, propertyName);
                var keySelector = Expression.Lambda(body, param);
                cache[propertyName] = keySelector;
                return keySelector;
            }

            public static IQueryable<T> OrderBy(IQueryable<T> queryable, string propertyName, bool isAsc)
            {
                dynamic keySelector = GetLambdaExpression(propertyName);
                return isAsc ? Queryable.OrderByDescending(queryable, keySelector) : Queryable.OrderBy(queryable, keySelector);
            }
        }

    }
}
