using FCP.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace FCP.Entity
{
    public static class EntityValidateExtensions
    {
        public static IEnumerable<ValidationResult> ValidateByExclude<T>(this T entity, params Expression<Func<T, object>>[] ignorePropertyExpressions)
        {
            return entity.ValidateInternal(true, ignorePropertyExpressions);
        }

        public static IEnumerable<ValidationResult> ValidateByInclude<T>(this T entity, params Expression<Func<T, object>>[] includePropertyExpressions)
        {
            return entity.ValidateInternal(false, includePropertyExpressions);
        }

        private static IEnumerable<ValidationResult> ValidateInternal<T>(this T entity, bool isExclude, params Expression<Func<T, object>>[] propertyExpressions)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var validateResults = entity.validate();
            if (validateResults.isEmpty())
                return validateResults;

            var propertyNames = GetPropertyNamesByExpression(propertyExpressions);

            Func<string, bool> propertyPredicate = (name) => propertyNames.Contains(name);
            if (isExclude)
            {
                propertyPredicate = (name) => !propertyNames.Contains(name);
            }

            return validateResults.Where(m => propertyPredicate(m.MemberNames.FirstOrDefault())).ToList();
        }

        private static IEnumerable<string> GetPropertyNamesByExpression<T>(params Expression<Func<T, object>>[] propertyExpressions)
        {
            if (propertyExpressions.isNotEmpty())
            {
                foreach (var propertyExpression in propertyExpressions)
                {
                    if (propertyExpression == null)
                        continue;

                    yield return ReflectionHelper.parsePropertyName(propertyExpression);
                }
            }
        }
    }
}
