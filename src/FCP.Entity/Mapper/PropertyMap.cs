using System;
using System.Reflection;

namespace FCP.Entity
{
    /// <summary>
    /// 属性映射
    /// </summary>
    public class PropertyMap : IPropertyMap
    {
        public PropertyMap(PropertyInfo propertyInfo)
        {
            this.propertyInfo = propertyInfo;
            columnName = propertyInfo.Name;
        }

        /// <summary>
        /// Gets the name of the property by using the specified propertyInfo.
        /// </summary>
        public string name
        {
            get { return propertyInfo.Name; }
        }

        /// <summary>
        /// Gets the column name for the current property.
        /// </summary>
        public string columnName { get; private set; }

        /// <summary>
        /// Gets the key type for the current property.
        /// </summary>
        public KeyType keyType { get; private set; }

        /// <summary>
        /// Gets the ignore status of the current property. If ignored, the current property will not be included in queries.
        /// </summary>
        public bool ignored { get; private set; }

        /// <summary>
        /// Gets the read-only status of the current property. If read-only, the current property will not be included in INSERT and UPDATE queries.
        /// </summary>
        public bool isReadOnly { get; private set; }

        /// <summary>
        /// Gets the delete flag status of the current property
        /// </summary>
        public bool isDeleteFlag { get; private set; }

        /// <summary>
        /// Gets the true value of the delete flag
        /// </summary>
        public object deleteFlagTrueValue { get; private set; }

        /// <summary>
        /// Gets the property info for the current property.
        /// </summary>
        public PropertyInfo propertyInfo { get; private set; }

        /// <summary>
        /// Fluently sets the column name for the property.
        /// </summary>
        /// <param name="columnName">The column name as it exists in the database.</param>
        public PropertyMap column(string columnName)
        {
            this.columnName = columnName;
            return this;
        }

        /// <summary>
        /// Fluently sets the key type of the property.
        /// </summary>
        /// <param name="columnName">The column name as it exists in the database.</param>
        public PropertyMap key(KeyType keyType)
        {
            if (ignored)
            {
                throw new ArgumentException(string.Format("'{0}' is ignored and cannot be made a key field. ", name));
            }

            if (isReadOnly)
            {
                throw new ArgumentException(string.Format("'{0}' is readonly and cannot be made a key field. ", name));
            }

            if (isDeleteFlag)
            {
                throw new ArgumentException(string.Format("'{0}' is delete flag and cannot be made a key field. ", name));
            }

            this.keyType = keyType;
            return this;
        }

        /// <summary>
        /// Fluently sets the ignore status of the property.
        /// </summary>
        public PropertyMap ignore()
        {
            if (keyType != KeyType.notAKey)
            {
                throw new ArgumentException(string.Format("'{0}' is a key field and cannot be ignored.", name));
            }

            ignored = true;
            return this;
        }

        /// <summary>
        /// Fluently sets the read-only status of the property.
        /// </summary>
        public PropertyMap readOnly()
        {
            if (keyType != KeyType.notAKey)
            {
                throw new ArgumentException(string.Format("'{0}' is a key field and cannot be marked readonly.", name));
            }

            if (isDeleteFlag)
            {
                throw new ArgumentException(string.Format("'{0}' is a delete flag field and cannot be marked readonly.", name));
            }

            isReadOnly = true;
            return this;
        }

        /// <summary>
        /// Fluently sets the delete flag status of the property
        /// </summary>
        /// <returns></returns>
        public PropertyMap deleteFlag(object trueValue)
        {
            if (keyType != KeyType.notAKey)
            {
                throw new ArgumentException(string.Format("'{0}' is a key field and cannot be marked delete flag.", name));
            }

            if (isReadOnly)
            {
                throw new ArgumentException(string.Format("'{0}' is a readonly field and cannot be marked delete flag.", name));
            }

            if (trueValue == null)
            {
                throw new ArgumentNullException(nameof(trueValue), string.Format("'{0}' field should provide true value to be marked delete flag", name));
            }

            isDeleteFlag = true;
            deleteFlagTrueValue = trueValue;
            return ignore();
        }
    }
}
