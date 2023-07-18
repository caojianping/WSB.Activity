using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WSB.Activity.Common
{
    /// <summary>
    /// 枚举扩展
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 获取备注信息
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetRemark(this Enum value)
        {
            string remark = string.Empty;
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());
            try
            {
                object[] attrs = fieldInfo.GetCustomAttributes(typeof(RemarkAttribute), false);
                RemarkAttribute attr = (RemarkAttribute)attrs.FirstOrDefault(a => a is RemarkAttribute);
                if (attr == null)
                {
                    remark = fieldInfo.Name;
                }
                else
                {
                    remark = attr.Remark;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"枚举扩展方法GetRemark出现异常：{ex}");
            }
            return remark;
        }

        /// <summary>
        /// 获取全部备注信息
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> GetAllRemarks(this Enum value)
        {
            Type type = value.GetType();
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            foreach (var field in type.GetFields())
            {
                if (field.FieldType.IsEnum)
                {
                    object temp = field.GetValue(value);
                    Enum enumValue = (Enum)temp;
                    int intValue = (int)temp;
                    result.Add(new KeyValuePair<string, string>(intValue.ToString(), enumValue.GetRemark()));
                }
            }
            return result;
        }
    }

    /// <summary>
    /// 备注特性
    /// </summary>
    public class RemarkAttribute : Attribute
    {
        private string _remark = string.Empty;
        public RemarkAttribute(string remark)
        {
            _remark = remark;
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get
            {
                return _remark;
            }
            set
            {
                _remark = value;
            }
        }

        public string Description { get; set; }
    }
}
