using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> entities, Filter filter) {
            var orderBy = ParseOrdering(filter.OrderBy);
            var orderedQuery = orderBy.IsDescending ? entities.OrderByDescending(orderBy.PropertyName) : entities.OrderBy(orderBy.PropertyName);

            if (filter.ThenBy == null || filter.ThenBy.Count() == 0) {
                return orderedQuery;
            }

            var currentOrderByIndex = 0;
            while (currentOrderByIndex < filter.ThenBy.Count()) {
                orderBy = ParseOrdering(filter.ThenBy[currentOrderByIndex]);
                orderedQuery = orderBy.IsDescending ? orderedQuery.ThenByDescending(orderBy.PropertyName) : orderedQuery.ThenBy(orderBy.PropertyName);
                currentOrderByIndex += 1;
            }

            return orderedQuery;
        }

        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> entities, string propertyName) {
            var propertyInfo = entities
                .First()
                .GetType()
                .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            return entities.OrderBy(e => propertyInfo.GetValue(e, null));
        }

        public static IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> entities, string propertyName) {
            var propertyInfo = entities
                .First()
                .GetType()
                .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            return entities.OrderByDescending(e => propertyInfo.GetValue(e, null));
        }

        public static IOrderedEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> entities, string propertyName) {
            var propertyInfo = entities
                .First()
                .GetType()
                .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            return entities.ThenBy(e => propertyInfo.GetValue(e, null));
        }

        public static IOrderedEnumerable<T> ThenByDescending<T>(this IOrderedEnumerable<T> entities, string propertyName) {
            var propertyInfo = entities
                .First()
                .GetType()
                .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            return entities.ThenByDescending(e => propertyInfo.GetValue(e, null));
        }
    }
}