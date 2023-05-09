using System.Reflection;
using System.Threading.Tasks;

namespace System
{
    public interface IDeputy : IUnique
    {
        string Name { get; set; }

        string QualifiedName { get; set; }

        object TargetObject { get; set; }

        MethodInfo Info { get; set; }

        ParameterInfo[] Parameters { get; set; }

        object[] ParameterValues { get; set; }

        MethodDeputy MethodDeputy { get; }

        Delegate Method { get; }

        Task Publish(params object[] parameters);
        Task Publish(bool firstAsTarget, object target, params object[] parameters);

        object Execute(params object[] parameters);
        object Execute(bool firstAsTarget, object target, params object[] parameters);

        Task<object> ExecuteAsync(params object[] parameters);
        Task<object> ExecuteAsync(bool firstAsTarget, object target, params object[] parameters);

        Task<T> ExecuteAsync<T>(params object[] parameters);
        Task<T> ExecuteAsync<T>(bool firstAsTarget, object target, params object[] parameters);
    }
}
