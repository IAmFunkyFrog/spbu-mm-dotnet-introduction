using System.Linq.Expressions;
using System.Reflection;

namespace Reflection;
public static class ReflectionDelegateFactory
{
    public static Func<ObjT, object?> MakePropertyAccessor<ObjT>(string path)
    {
        var accessor = MakePropertyAccessorLinqImpl<ObjT, object?>(path);
        return accessor;
    }

    public static Func<ObjT, ReturnT?> MakePropertyAccessor<ObjT, ReturnT>(string path)
    {
        var accessor = MakePropertyAccessorLinqImpl<ObjT, ReturnT>(path);
        return accessor;
    }

    private static Func<ObjT, ReturnT> MakePropertyAccessorLinqImpl<ObjT, ReturnT>(string path)
    {
        var param = Expression.Parameter(typeof(ObjT), "obj");
        var props = path.Split(".");

        Type lastType = typeof(ObjT);
        Expression access = param;
        foreach (var prop in props)
        {
            PropertyInfo? propertyInfo;
            propertyInfo = lastType.GetProperty(prop, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (propertyInfo != null)
            {
                access = Expression.Property(access, propertyInfo);
            }
            else
            {
                // Try find static property if not found instance property
                propertyInfo = lastType.GetProperty(prop, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                        ?? throw new ArgumentException(String.Format("{0} is not property in {1} type", prop, lastType.Name));
                access = Expression.Property(null, propertyInfo);
            }
            lastType = propertyInfo.PropertyType;
        }

        if (!lastType.IsAssignableTo(typeof(ReturnT)))
        {
            throw new ArgumentException(String.Format("{0} property have not {1} type, it have {2} type",
                                        props[props.Length - 1], typeof(ReturnT).Name, lastType)
            );
        }

        Expression conversion = Expression.Convert(access, typeof(ReturnT));
        var compiledLambda = Expression.Lambda(typeof(Func<ObjT, ReturnT>), conversion, param).Compile();
        return (Func<ObjT, ReturnT>)compiledLambda;
    }
}
