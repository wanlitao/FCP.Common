using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FCP.Util
{
    /// <summary>
    /// 数据校验助手
    /// </summary>
    public static class DataValidateHelper
    {
        /// <summary>
        /// 默认验证
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static IEnumerable<ValidationResult> validate<T>(this T entity)
        {
            return validate<T>(entity, false, null);
        }
        /// <summary>
        /// 验证实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="modelIsValid">绑定验证结果 true则不做基础验证</param>
        /// <param name="Others">其他验证委托</param>
        /// <returns></returns>
        public static IEnumerable<ValidationResult> validate<T>(this T entity, bool modelIsValid, Func<T, List<ValidationResult>> Others)
        {
            List<ValidationResult> vrs = null;
            bool isValid = true;
            if (!modelIsValid)
            {
                isValid = entity.validate(out vrs);
            }
            if (Others != null)
            {
                //获取其他验证
                List<ValidationResult> ovrs = Others(entity);
                if (vrs.isNotEmpty() && ovrs.isNotEmpty()) 
                {
                    vrs.AddRange(ovrs);
                }
                else
                {
                    vrs = ovrs;
                }
            }
            return vrs ?? new List<ValidationResult>();
        }

        /// <summary>
        /// 验证实体 返回错误信息列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="validationResults"></param>
        /// <returns></returns>
        public static bool validate<T>(this T entity, out List<ValidationResult> validationResults)
        {
            var _validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(entity, null, null);
            bool isValid = Validator.TryValidateObject(entity, validationContext, _validationResults, true);
            validationResults = _validationResults;
            return isValid;
        }

        /// <summary>
        /// 生成错误信息
        /// </summary>
        /// <param name="errorMessage">错误信息</param>
        /// <param name="args">可变字符组</param>
        /// <returns></returns>
        public static ValidationResult validResult(string errorMessage, params string[] args)
        {
            IEnumerable<string> memberNames = args.Select(s => s);
            return new ValidationResult(errorMessage, memberNames);
        }
    }
}
