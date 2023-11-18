using System.Reflection;

namespace Reflection;
public static class ReflectionDelegateFactory
{
    public static Func<ObjT, object?> MakePropertyAccessor<ObjT>(string path)
    {
        var accessor = MakePropertyAccessorImpl<object>(typeof(ObjT), path);
        return obj => accessor.Invoke(obj);
    }

    public static Func<ObjT, ReturnT?> MakePropertyAccessor<ObjT, ReturnT>(string path)
    {
        var accessor = MakePropertyAccessorImpl<ReturnT>(typeof(ObjT), path);
        return obj => accessor.Invoke(obj);
    }

    private static Func<object?, ReturnT?> MakePropertyAccessorImpl<ReturnT>(Type type, string path)
    {
        var props = path.Split(".");
        var propName = props[0];

        var prop = type.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static) 
                        ?? throw new ArgumentException(String.Format("{0} is not property in {1} type", propName, type.Name));

        if (props.Length == 1) 
        {
            if(prop.PropertyType.IsAssignableTo(typeof(ReturnT))) return obj => 
            {
                return (ReturnT?) prop.GetValue(obj);
            };

            throw new ArgumentException(String.Format("{0} property on {1} have not {2} type, it have {3} type",
                                        path, type.Name, typeof(ReturnT).Name, prop.PropertyType)
            );
        }

        var otherProps = String.Join(".", props.Skip(1).ToArray());
        var outerAccessor = MakePropertyAccessorImpl<ReturnT>(prop.PropertyType, otherProps);

        return obj =>
        {
            return outerAccessor.Invoke(prop.GetValue(obj));
        };
    }
}
