using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FCP.Entity
{
    /// <summary>
    /// 属性表达式解析
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyExpressionParser<T>
    {
        private readonly PropertyInfo _property;

        public PropertyExpressionParser(Expression<Func<T, object>> propertyExpression)
        {
            _property = GetProperty(propertyExpression);
        }

        private static PropertyInfo GetProperty(Expression<Func<T, object>> expression)
        {
            var propertyInfo = ReflectionHelper.parseProperty(expression);

            if (propertyInfo != null)
                return propertyInfo;

            throw new ArgumentException(string.Format("Expression '{0}' does not refer to a property.", expression.ToString()));
        }

        public string Name
        {
            get { return _property.Name; }
        }

        public Type Type
        {
            get { return ReflectionHelper.getPropertyType(_property); }
        }

        public object GetItemValue(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return ReflectionHelper.getPropertyValue(item, _property);
        }
    }
}
