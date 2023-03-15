using System;
using System.Collections.Generic;

namespace CSToolbox.Weak
{
    /// <summary>
    /// A weak event implementation
    /// </summary>
    public class WeakEvent
    {
        protected List<WeakDelegate> DelegateList { get; }

        public WeakEvent()
        {
            DelegateList = new List<WeakDelegate>();
        }

        /// <summary>
        /// Invokes the event with provided parameters
        /// </summary>
        /// <param name="parameters">The parameters. Same as parameters of <see cref="Delegate.DynamicInvoke(object?[]?)"/>.</param>
        public void Invoke(object?[]? parameters)
        {
            WeakDelegate target;
            for (int i = 0; i < DelegateList.Count;)
            {
                target = DelegateList[i];
                if (target.IsAlive)
                {
                    target.Invoke(parameters);
                    i++;
                }
                else
                {
                    DelegateList[i] = DelegateList[DelegateList.Count - 1];
                    DelegateList.RemoveAt(DelegateList.Count - 1);
                }
            }
        }

        public WeakEvent Subscribe(WeakDelegate target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            DelegateList.Add(target);
            return this;
        }
        public WeakEvent Subscribe(Delegate target) => Subscribe(new WeakDelegate(target));

        public WeakEvent Unsubscribe(WeakDelegate target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            int id = DelegateList.IndexOf(target);
            if (id != -1)
            {
                DelegateList[id] = DelegateList[DelegateList.Count - 1];
                DelegateList.RemoveAt(DelegateList.Count - 1);
            }
            return this;
        }
        public WeakEvent Unsubscribe(Delegate target) => Unsubscribe(new WeakDelegate(target));

        public static WeakEvent operator +(WeakEvent weakEvent, WeakDelegate target) => weakEvent.Subscribe(target);

        public static WeakEvent operator -(WeakEvent weakEvent, WeakDelegate target) => weakEvent.Unsubscribe(target);
    }

    public sealed class WeakEvent<Arg1>
    {
        private WeakEvent Event { get; } = new();

        public void Invoke(Arg1 arg1) => Event.Invoke(new object[] { arg1 });

        public WeakEvent<Arg1> Subscribe(WeakAction<Arg1> target)
        {
            Event.Subscribe(target);
            return this;
        }

        public WeakEvent<Arg1> Unsubscribe(WeakAction<Arg1> target)
        {
            Event.Unsubscribe(target);
            return this;
        }

        public WeakEvent<Arg1> Subscribe(WeakAction<Arg1>.CallType target)
        {
            Event.Subscribe(target);
            return this;
        }

        public WeakEvent<Arg1> Unsubscribe(WeakAction<Arg1>.CallType target)
        {
            Event.Unsubscribe(target);
            return this;
        }

        public static WeakEvent<Arg1> operator +(WeakEvent<Arg1> weakEvent, WeakAction<Arg1> target) => weakEvent.Subscribe(target);
        public static WeakEvent<Arg1> operator -(WeakEvent<Arg1> weakEvent, WeakAction<Arg1> target) => weakEvent.Unsubscribe(target);
    }

    public sealed class WeakEvent<Arg1, Arg2>
    {
        private WeakEvent Event { get; } = new();

        public void Invoke(Arg1 arg1, Arg2 arg2) => Event.Invoke(new object[] { arg1, arg2 });

        public WeakEvent<Arg1, Arg2> Subscribe(WeakAction<Arg1, Arg2> target)
        {
            Event.Subscribe(target);
            return this;
        }

        public WeakEvent<Arg1, Arg2> Unsubscribe(WeakAction<Arg1, Arg2> target)
        {
            Event.Unsubscribe(target);
            return this;
        }

        public WeakEvent<Arg1, Arg2> Subscribe(WeakAction<Arg1, Arg2>.CallType target)
        {
            Event.Subscribe(target);
            return this;
        }

        public WeakEvent<Arg1, Arg2> Unsubscribe(WeakAction<Arg1, Arg2>.CallType target)
        {
            Event.Unsubscribe(target);
            return this;
        }

        public static WeakEvent<Arg1, Arg2> operator +(WeakEvent<Arg1, Arg2> weakEvent, WeakAction<Arg1, Arg2> target) => weakEvent.Subscribe(target);
        public static WeakEvent<Arg1, Arg2> operator -(WeakEvent<Arg1, Arg2> weakEvent, WeakAction<Arg1, Arg2> target) => weakEvent.Unsubscribe(target);
    }
}