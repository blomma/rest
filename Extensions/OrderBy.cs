using System;
using System.Linq;
using System.Linq.Expressions;
using rest.Models;

namespace rest.Extensions {
    public static class OrderByExtensions {
        private static (string PropertyName, bool IsDescending) ParseOrdering(string value) {
            var startOfOrderingIndex = value.IndexOf(":");
            if (startOfOrderingIndex == -1) {
                return (value, false);
            }

            if (startOfOrderingIndex + 1 == value.Count()) {
                throw new ArgumentOutOfRangeException($"Missing ordering for sort, {value}");
            }

            var ordering = value.Substring(startOfOrderingIndex + 1);
            if (ordering.ToLowerInvariant() == "desc") {
                var propertyName = value.Substring(0, startOfOrderingIndex);
                return (propertyName, true);
            }

            throw new NotImplementedException($"Ordering:{ordering} for sorting is not implemented");
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> entities, Filter filter) {
            var orderBy = ParseOrdering(filter.OrderBy);
            var orderedQuery = orderBy.IsDescending ? entities.OrderByMemberDescending(orderBy.PropertyName) : entities.OrderByMember(orderBy.PropertyName);

            if (filter.ThenBy == null || filter.ThenBy.Count() == 0) {
                return orderedQuery;
            }

            var currentOrderByIndex = 0;
            while (currentOrderByIndex < filter.ThenBy.Count()) {
                orderBy = ParseOrdering(filter.ThenBy[currentOrderByIndex]);
                orderedQuery = orderBy.IsDescending ? orderedQuery.ThenByMemberDescending(orderBy.PropertyName) : orderedQuery.ThenByMember(orderBy.PropertyName);
                currentOrderByIndex += 1;
            }

            return orderedQuery;
        }

        public static IOrderedQueryable<T> OrderByMember<T>(this IQueryable<T> source, string memberPath) {
            return source.OrderByMemberUsing(memberPath, "OrderBy");
        }

        public static IOrderedQueryable<T> OrderByMemberDescending<T>(this IQueryable<T> source, string memberPath) {
            return source.OrderByMemberUsing(memberPath, "OrderByDescending");
        }

        public static IOrderedQueryable<T> ThenByMember<T>(this IOrderedQueryable<T> source, string memberPath) {
            return source.OrderByMemberUsing(memberPath, "ThenBy");
        }

        public static IOrderedQueryable<T> ThenByMemberDescending<T>(this IOrderedQueryable<T> source, string memberPath) {
            return source.OrderByMemberUsing(memberPath, "ThenByDescending");
        }

        private static IOrderedQueryable<T> OrderByMemberUsing<T>(this IQueryable<T> source, string memberPath, string method) {
            var parameter = Expression.Parameter(typeof(T), "item");
            var member = memberPath.Split('.')
                .Aggregate((Expression)parameter, Expression.PropertyOrField);
            var keySelector = Expression.Lambda(member, parameter);
            var methodCall = Expression.Call(
                typeof(Queryable), method, new[] { parameter.Type, member.Type },
                source.Expression, Expression.Quote(keySelector));

            return (IOrderedQueryable<T>)source.Provider.CreateQuery(methodCall);
        }
    }
}