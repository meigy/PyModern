using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Share.Utils
{
    public static class Reflectionutils
    {
        //1、得到私有字段的值：
        public static T GetPrivateField<T>(this object instance, string fieldname) { BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic; Type type = instance.GetType(); FieldInfo field = type.GetField(fieldname, flag); return (T)field.GetValue(instance); }
        //2、得到私有属性的值：
        public static T GetPrivateProperty<T>(this object instance, string propertyname) { BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic; Type type = instance.GetType(); PropertyInfo field = type.GetProperty(propertyname, flag); return (T)field.GetValue(instance, null); }
        //3、设置私有成员的值：
        public static void SetPrivateField(this object instance, string fieldname, object value)
        {
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType();
            FieldInfo field = type.GetField(fieldname, flag);
            field.SetValue(instance, value);
        }
        //4、设置私有属性的值： 
        public static void SetPrivateProperty(this object instance, string propertyname, object value)
        {
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType();
            PropertyInfo field = type.GetProperty(propertyname, flag);
            field.SetValue(instance, value, null);
        }
        //5、调用私有方法：
        public static T CallPrivateMethod<T>(this object instance, string name, params object[] param)
        {
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType();
            MethodInfo method = type.GetMethod(name, flag);
            return (T)method.Invoke(instance, param);
        }

        public static void CallPrivateMethod(this object instance, string name, params object[] param)
        {
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType();
            MethodInfo method = type.GetMethod(name, flag);
            method.Invoke(instance, param);
        }

        public static void CallPrivateMethod_Overload(this object instance, string name, Type[] paramtyps, params object[] param)
        {
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType();
            MethodInfo method = type.GetMethod(name, flag, null, paramtyps, null);
            method.Invoke(instance, param);
        }

        public static object CallPrivateStaticMethod(System.Type type, string name, params object[] param)
        {
            return type.InvokeMember(name, BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static, null, null, param);
        }

        public static T CallPrivateStaticMethod<T>(System.Type type, string name, params object[] param)
        {
            BindingFlags flag = BindingFlags.Static | BindingFlags.NonPublic;
            MethodInfo method = type.GetMethod(name, flag);
            return (T)method.Invoke(null, param);
        }

    }
}
