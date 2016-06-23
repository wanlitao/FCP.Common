using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace FCP.Util
{
    /// <summary>
    /// 动态转换助手
    /// </summary>
    public static class DynamicConvertHelper
    {
        #region object转换为字典
        /// <summary>
        /// 转换方法委托缓存
        /// </summary>
        private static ConcurrentDictionary<Type, Func<object, IDictionary<string, object>>> _objectDicConverterCache =
           new ConcurrentDictionary<Type, Func<object, IDictionary<string, object>>>();

        /// <summary>
        /// object转换为字典
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="objDicConverterFactory">object字典转换器工厂</param>
        /// <returns></returns>
        private static IDictionary<string, object> toDictionary(this object obj,
            Func<Type, Func<object, IDictionary<string, object>>> objDicConverterFactory)
        {
            if (obj == null) return null;

            var objType = obj.GetType();
            var factory = _objectDicConverterCache.GetOrAdd(objType, objDicConverterFactory);
            return factory(obj);
        }

        #region 通过表达式树转换
        /// <summary>
        /// 通过表达式树 转换object为字典
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IDictionary<string, object> toDictionaryByExpression(this object obj)
        {
            return obj.toDictionary(getObjDicExpressionTreeConverter);
        }

        /// <summary>
        /// 生成object转换字典的 表达式树转换器
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        private static Func<object, IDictionary<string, object>> getObjDicExpressionTreeConverter(Type objType)
        {
            var dictionaryType = typeof(Dictionary<string, object>);
            var dictionaryAddMethod = dictionaryType.GetMethod("Add");

            // define the parameter
            var parameterExpr = Expression.Parameter(typeof(object), "obj");

            // collect the body
            var bodyExprs = new List<Expression>();

            // code: var objDic = new Dictionary<string, object>();
            var dicVariableExpr = Expression.Variable(dictionaryType, "objDic");
            var newDictionaryTypeExpr = Expression.New(dictionaryType);
            var assignDicVariableExpr = Expression.Assign(dicVariableExpr, newDictionaryTypeExpr);
            bodyExprs.Add(assignDicVariableExpr);

            // code: var strongObj = (objType)obj;
            var strongTypedExpr = Expression.Variable(objType, "strongObj");
            var castEntityExpr = Expression.Convert(parameterExpr, objType);
            var assignStrongTypedExpr = Expression.Assign(strongTypedExpr, castEntityExpr);
            bodyExprs.Add(assignStrongTypedExpr);

            // generate code for property value add to dictionary
            var attributes = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
            foreach (var property in objType.GetProperties(attributes).Where(info => info.CanRead))
            {
                // code: objDic.Add(propertyName, strongObj.propertyName)
                var propertyNameExpr = Expression.Constant(property.Name, typeof(string));
                var propertyExpr = Expression.Property(strongTypedExpr, property);
                var addPropertyToDicExpr = Expression.Call(dicVariableExpr, dictionaryAddMethod, propertyNameExpr, propertyExpr);
                bodyExprs.Add(addPropertyToDicExpr);
            }

            // code: return (IDictionary<string, object>)objDic;
            var castResultExpr = Expression.Convert(dicVariableExpr, typeof(IDictionary<string, object>));
            bodyExprs.Add(castResultExpr);

            // code: { ... }
            var methodBodyExpr = Expression.Block(
                typeof(IDictionary<string, object>), /* return type */
                new[] { dicVariableExpr, strongTypedExpr } /* local variables */,
                bodyExprs /* body expressions */);

            // code: entity => { ... }
            var lambdaExpr = Expression.Lambda<Func<object, IDictionary<string, object>>>(methodBodyExpr, parameterExpr);

            return lambdaExpr.Compile();
        }
        #endregion

        #region 通过动态方法emit转换
        /// <summary>
        /// 通过动态方法emit 转换object为字典
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IDictionary<string, object> toDictionaryByDynamicEmit(this object obj)
        {
            return obj.toDictionary(getObjDicDynamicEmitConverter);
        }

        /// <summary>
        /// 生成object转换字典的 动态方法Emit转换器
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        public static Func<object, IDictionary<string, object>> getObjDicDynamicEmitConverter(Type objType)
        {
            var dictionaryType = typeof(Dictionary<string, object>);

            // Dictionary.Add(object key, object value)
            var addMethod = dictionaryType.GetMethod("Add");

            // setup dynamic method
            // Important: make itemType owner of the method to allow access to internal types
            var dynamicMethod = new DynamicMethod(string.Empty, typeof(IDictionary<string, object>), new[] { typeof(object) }, objType);
            var ilGenerator = dynamicMethod.GetILGenerator();

            // create the Dictionary and store it in a local variable
            ilGenerator.DeclareLocal(dictionaryType);
            ilGenerator.Emit(OpCodes.Newobj, dictionaryType.GetConstructor(Type.EmptyTypes));
            ilGenerator.Emit(OpCodes.Stloc_0);

            var attributes = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
            foreach (var property in objType.GetProperties(attributes).Where(info => info.CanRead))
            {
                // load Dictionary (prepare for call later)
                ilGenerator.Emit(OpCodes.Ldloc_0);
                // load key, i.e. name of the property
                ilGenerator.Emit(OpCodes.Ldstr, property.Name);

                // load value of property to stack
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.EmitCall(OpCodes.Callvirt, property.GetGetMethod(), null);
                // perform boxing if necessary
                if (property.PropertyType.IsValueType)
                {
                    ilGenerator.Emit(OpCodes.Box, property.PropertyType);
                }

                // stack at this point
                // 1. string or null (value)
                // 2. string (key)
                // 3. dictionary

                // ready to call dict.Add(key, value)
                ilGenerator.EmitCall(OpCodes.Callvirt, addMethod, null);
            }
            // finally load Dictionary and return
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ret);

            return (Func<object, IDictionary<string, object>>)dynamicMethod.CreateDelegate(typeof(Func<object, IDictionary<string, object>>));
        }
        #endregion

        #endregion

        #region 字典转换为object
        /// <summary>
        /// 字典转换为object
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static dynamic toDynamic(this IDictionary<string, object> dict)
        {
            var expandoObj = new ExpandoObject() as IDictionary<string, object>;
            if (dict.isNotEmpty())
            {
                foreach(var item in dict)
                {
                    expandoObj.Add(item);
                }
            }
            return expandoObj;
        }
        #endregion
    }
}
