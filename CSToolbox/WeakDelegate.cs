using System;
using System.Reflection;

namespace CSToolbox
{
    /// <summary>
    /// A delegate which does not hold a reference to the object it's invoked on
    /// Sadly, much slower than <see cref="Delegate"/> because it uses reflection
    /// </summary>
    public class WeakDelegate
    {
        private MethodInfo Method { get; }
        private WeakReference Object { get; }

        /// <summary>
        /// Returns true if the target object is still alive and the delegate can be called
        /// </summary>
        public bool IsAlive => Object.IsAlive;

        public WeakDelegate(MethodInfo method, object target)
        {
            Method = method ?? throw new ArgumentNullException(nameof(method));
            if (Method.IsStatic)
                throw new ArgumentException("Static methods are not supported by WeakDelegate", nameof(method));
            else if (target == null)
                throw new ArgumentNullException(nameof(target));
            Object = new WeakReference(target);
        }

        public WeakDelegate(Delegate del) : this(del?.Method, del?.Target) {}

        /// <summary>
        /// Invokes this delegate.
        /// </summary>
        /// <param name="args">The arguments for the delegate call (similar to args of DynamicInvoke() on Delegate).</param>
        /// <returns>Delegate call result. It a call is impossible, returns null.</returns>
        public object? Invoke(object?[]? args)
        {
            if (!IsAlive)
                return null;
            return Method.Invoke(Object.Target, args);
        }

        public override bool Equals(object? obj)
        {
            return obj is WeakDelegate other && other.Object.Target == Object.Target && other.Method.Equals(Method);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    public sealed class WeakDelegate<RetType> : WeakDelegate
    {
        public delegate RetType CallType();

        public WeakDelegate(CallType del) : base(del) {}

        public RetType Invoke() => (RetType)Invoke(null);
    }

    public sealed class WeakDelegate<RetType, Arg1> : WeakDelegate
    {
        public delegate RetType CallType(Arg1 arg1);

        public WeakDelegate(CallType del) : base(del) {}

        public RetType Invoke(Arg1 arg1) => (RetType)Invoke(new object[] {arg1});
    }

    public sealed class WeakDelegate<RetType, Arg1, Arg2> : WeakDelegate
    {
        public delegate RetType CallType(Arg1 arg1, Arg2 arg2);

        public WeakDelegate(CallType del) : base(del) {}

        public RetType Invoke(Arg1 arg1, Arg2 arg2) => (RetType)Invoke(new object[] {arg1, arg2});
    }

    public sealed class WeakDelegate<RetType, Arg1, Arg2, Arg3> : WeakDelegate
    {
        public delegate RetType CallType(Arg1 arg1, Arg2 arg2, Arg3 arg3);

        public WeakDelegate(CallType del) : base(del) {}

        public RetType Invoke(Arg1 arg1, Arg2 arg2, Arg3 arg3) => (RetType)Invoke(new object[] {arg1, arg2, arg3});
    }

    public sealed class WeakDelegate<RetType, Arg1, Arg2, Arg3, Arg4> : WeakDelegate
    {
        public delegate RetType CallType(Arg1 arg1, Arg2 arg2, Arg3 arg3, Arg4 arg4);

        public WeakDelegate(CallType del) : base(del) {}

        public RetType Invoke(Arg1 arg1, Arg2 arg2, Arg3 arg3, Arg4 arg4) => (RetType)Invoke(new object[] {arg1, arg2, arg3, arg4});
    }

    public sealed class WeakDelegate<RetType, Arg1, Arg2, Arg3, Arg4, Arg5> : WeakDelegate
    {
        public delegate RetType CallType(Arg1 arg1, Arg2 arg2, Arg3 arg3, Arg4 arg4, Arg5 arg5);

        public WeakDelegate(CallType del) : base(del) {}

        public RetType Invoke(Arg1 arg1, Arg2 arg2, Arg3 arg3, Arg4 arg4, Arg5 arg5) => (RetType)Invoke(new object[] {arg1, arg2, arg3, arg4, arg5});
    }

    public sealed class WeakDelegate<RetType, Arg1, Arg2, Arg3, Arg4, Arg5, Arg6> : WeakDelegate
    {
        public delegate RetType CallType(Arg1 arg1, Arg2 arg2, Arg3 arg3, Arg4 arg4, Arg5 arg5, Arg6 arg6);

        public WeakDelegate(CallType del) : base(del) {}

        public RetType Invoke(Arg1 arg1, Arg2 arg2, Arg3 arg3, Arg4 arg4, Arg5 arg5, Arg6 arg6) => (RetType)Invoke(new object[] {arg1, arg2, arg3, arg4, arg5, arg6});
    }

    public sealed class WeakDelegate<RetType, Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7> : WeakDelegate
    {
        public delegate RetType CallType(Arg1 arg1, Arg2 arg2, Arg3 arg3, Arg4 arg4, Arg5 arg5, Arg6 arg6, Arg7 arg7);

        public WeakDelegate(CallType del) : base(del) {}

        public RetType Invoke(Arg1 arg1, Arg2 arg2, Arg3 arg3, Arg4 arg4, Arg5 arg5, Arg6 arg6, Arg7 arg7) => (RetType)Invoke(new object[] {arg1, arg2, arg3, arg4, arg5, arg6, arg7});
    }

    public sealed class WeakDelegate<RetType, Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8> : WeakDelegate
    {
        public delegate RetType CallType(Arg1 arg1, Arg2 arg2, Arg3 arg3, Arg4 arg4, Arg5 arg5, Arg6 arg6, Arg7 arg7, Arg8 arg8);

        public WeakDelegate(CallType del) : base(del) {}

        public RetType Invoke(Arg1 arg1, Arg2 arg2, Arg3 arg3, Arg4 arg4, Arg5 arg5, Arg6 arg6, Arg7 arg7, Arg8 arg8) => (RetType)Invoke(new object[] {arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8});
    }

    public sealed class WeakAction : WeakDelegate
    {
        public delegate void CallType();

        public WeakAction(CallType del) : base(del) {}

        public void Invoke() => Invoke(null);
    }

    public sealed class WeakAction<Arg1> : WeakDelegate
    {
        public delegate void CallType(Arg1 arg1);

        public WeakAction(CallType del) : base(del) {}

        public void Invoke(Arg1 arg1) => Invoke(new object[] {arg1});
    }

    public sealed class WeakAction<Arg1, Arg2> : WeakDelegate
    {
        public delegate void CallType(Arg1 arg1, Arg2 arg2);

        public WeakAction(CallType del) : base(del) {}

        public void Invoke(Arg1 arg1, Arg2 arg2) => Invoke(new object[] {arg1, arg2});
    }

    public sealed class WeakAction<Arg1, Arg2, Arg3> : WeakDelegate
    {
        public delegate void CallType(Arg1 arg1, Arg2 arg2, Arg3 arg3);

        public WeakAction(CallType del) : base(del) {}

        public void Invoke(Arg1 arg1, Arg2 arg2, Arg3 arg3) => Invoke(new object[] {arg1, arg2, arg3});
    }

    public sealed class WeakAction<Arg1, Arg2, Arg3, Arg4> : WeakDelegate
    {
        public delegate void CallType(Arg1 arg1, Arg2 arg2, Arg3 arg3, Arg4 arg4);

        public WeakAction(CallType del) : base(del) {}

        public void Invoke(Arg1 arg1, Arg2 arg2, Arg3 arg3, Arg4 arg4) => Invoke(new object[] {arg1, arg2, arg3, arg4});
    }

    public sealed class WeakAction<Arg1, Arg2, Arg3, Arg4, Arg5> : WeakDelegate
    {
        public delegate void CallType(Arg1 arg1, Arg2 arg2, Arg3 arg3, Arg4 arg4, Arg5 arg5);

        public WeakAction(CallType del) : base(del) {}

        public void Invoke(Arg1 arg1, Arg2 arg2, Arg3 arg3, Arg4 arg4, Arg5 arg5) => Invoke(new object[] {arg1, arg2, arg3, arg4, arg5});
    }

    public sealed class WeakAction<Arg1, Arg2, Arg3, Arg4, Arg5, Arg6> : WeakDelegate
    {
        public delegate void CallType(Arg1 arg1, Arg2 arg2, Arg3 arg3, Arg4 arg4, Arg5 arg5, Arg6 arg6);

        public WeakAction(CallType del) : base(del) {}

        public void Invoke(Arg1 arg1, Arg2 arg2, Arg3 arg3, Arg4 arg4, Arg5 arg5, Arg6 arg6) => Invoke(new object[] {arg1, arg2, arg3, arg4, arg5, arg6});
    }

    public sealed class WeakAction<Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7> : WeakDelegate
    {
        public delegate void CallType(Arg1 arg1, Arg2 arg2, Arg3 arg3, Arg4 arg4, Arg5 arg5, Arg6 arg6, Arg7 arg7);

        public WeakAction(CallType del) : base(del) {}

        public void Invoke(Arg1 arg1, Arg2 arg2, Arg3 arg3, Arg4 arg4, Arg5 arg5, Arg6 arg6, Arg7 arg7) => Invoke(new object[] {arg1, arg2, arg3, arg4, arg5, arg6, arg7});
    }

    public sealed class WeakAction<Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8> : WeakDelegate
    {
        public delegate void CallType(Arg1 arg1, Arg2 arg2, Arg3 arg3, Arg4 arg4, Arg5 arg5, Arg6 arg6, Arg7 arg7, Arg8 arg8);

        public WeakAction(CallType del) : base(del) {}

        public void Invoke(Arg1 arg1, Arg2 arg2, Arg3 arg3, Arg4 arg4, Arg5 arg5, Arg6 arg6, Arg7 arg7, Arg8 arg8) => Invoke(new object[] {arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8});
    }
}