﻿namespace System.Threading.Workflow
{
    using System.Linq;
    using System.Series;

    public class Workout
    {
        public Aspect Aspect;
        public Case Case;
        public WorkItem Work;
        public Workspace Workator;

        public Workout(
            bool safeClose,
            string className,
            string methodName,
            out object result,
            params object[] input
        ) : this(1, safeClose, Summon.New(className), methodName, input)
        {
            result = Work.GetOutput();
        }

        public Workout(IDeputy method) : this(1, false, method) { }

        public Workout(IDeputy method, bool safe, params object[] input)
            : this(1, safe, method, input) { }

        public Workout(IDeputy method, params object[] input) : this(1, false, method, input) { }

        public Workout(int workersCount, bool safeClose, IDeck<IDeputy> _methods)
        {
            Case = new Case();
            Aspect = Case.Aspect("FirstWorkNow");
            foreach (var am in _methods)
                Aspect.AddWork(am);
            Aspect.Allocate(workersCount);

            Workator = Aspect.Workator;
            foreach (WorkItem am in Aspect)
                am.Run(am.ParameterValues);

            Aspect.Workator.Close(safeClose);
        }

        public Workout(int workersCount, bool safeClose, IDeputy method, params object[] input)
        {
            Case = new Case();
            Aspect = Case.Aspect("FirstWorkNow").AddWork(method).Aspect.Allocate(workersCount);

            Workator = Aspect.Workator;
            Work = Aspect.AsValues().ElementAt(0);
            Case.Run(method.Name, input);
            Workator.Close(safeClose);
        }

        public Workout(
            int workersCount,
            bool safeClose,
            object classObject,
            string methodName,
            params object[] input
        )
        {
            IDeputy am = new Deputy(classObject, methodName);
            Case = new Case();
            Aspect = Case.Aspect("FirstWorkNow").AddWork(am).Aspect.Allocate(workersCount);

            Workator = Aspect.Workator;
            Work = Aspect.AsValues().ElementAt(0);
            Case.Run(am.Name, input);
            Workator.Close(safeClose);
        }

        public Workout(
            int workersCount,
            int evokerCount,
            bool safeClose,
            IDeputy method,
            IDeputy evoker
        )
        {
            Case = new Case();
            Aspect = Case.Aspect("FirstWorkNow").AddWork(method).Aspect.Allocate(workersCount);
            Case.Aspect("SecondWorkNow").AddWork(evoker).Aspect.Allocate(evokerCount);

            Workator = Aspect.Workator;
            Work = Aspect.AsValues().ElementAt(0);
            Work.FlowTo(Case.AsValues().Skip(1).FirstOrDefault().AsValues().FirstOrDefault());
            Case.Run(method.Name, method.ParameterValues);
            Workator.Close(safeClose);
        }

        public Workout(
            object classObject,
            string methodName,
            out object result,
            params object[] input
        ) : this(1, false, classObject, methodName, input)
        {
            result = Work.GetOutput();
        }

        public Workout(string className, string methodName, params object[] input)
            : this(1, false, Summon.New(className), methodName, input) { }

        public static Workout Run<TClass>()
        {
            return new Workout(new Deputy<TClass>());
        }

        public static Workout Run<TClass>(bool safeThread, params object[] input)
        {
            return new Workout(new Deputy<TClass>(), safeThread, input);
        }

        public static Workout Run<TClass>(object[] constructorParams, params object[] input)
        {
            return new Workout(new Deputy<TClass>(constructorParams), input);
        }

        public static Workout Run<TClass>(
            string methodName,
            object[] constructorParams,
            params object[] input
        )
        {
            return new Workout(new Deputy<TClass>(methodName, constructorParams), input);
        }

        public static Workout Run<TClass>(string methodName, params object[] input)
        {
            return new Workout(new Deputy<TClass>(methodName), input);
        }

        public static Workout Run<TClass>(
            string methodName,
            Type[] parameterTypes,
            object[] constructorParams,
            params object[] input
        )
        {
            return new Workout(
                new Deputy<TClass>(methodName, parameterTypes, constructorParams),
                input
            );
        }

        public static Workout Run<TClass>(
            string methodName,
            Type[] parameterTypes,
            params object[] input
        )
        {
            return new Workout(new Deputy<TClass>(methodName, parameterTypes), input);
        }

        public static Workout Run<TClass>(
            Type[] parameterTypes,
            object[] constructorParams,
            params object[] input
        )
        {
            return new Workout(new Deputy<TClass>(parameterTypes, constructorParams), input);
        }

        public static Workout Run<TClass>(Type[] parameterTypes, params object[] input)
        {
            return new Workout(new Deputy<TClass>(parameterTypes), input);
        }

        public void Close(bool safeClose = false)
        {
            Aspect.Workator.Close(safeClose);
        }

        public void Run()
        {
            Workator.Run(Work);
        }

        public void Run(params object[] input)
        {
            this.Work.SetInput(input);
            Workator.Run(this.Work);
        }
    }
}
