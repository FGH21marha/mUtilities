// Written by Martin Halldin (https://github.com/FGH21marha/mUtilities/mActions)

using System;
using System.Linq;
using System.Reflection;

[Serializable] public class mAction
{
    private event Action myEvent;

    public int listenerCount { get { return GetListenerCount(); } }
    public int uniqueListenerCount { get { return GetUniqueListenerCount(); } }

    public void Invoke() => myEvent?.Invoke();
    public void InvokeOn(object target)
    {
        for (int i = 0; i < myEvent.GetInvocationList().Length; i++)
        {
            Action listener = GetListener(i);

            if (listener.Target == target)
            {
                listener.Method.Invoke(listener.Target, listener.Method.GetParameters());
                break;
            }
        }
    }
    public void AddListener(Action listener) => myEvent += listener;
    public void AddListeners(Action[] listeners)
    {
        foreach (Action listener in listeners)
            myEvent += listener;
    }
    public void RemoveListener(Action listener) => myEvent -= listener;
    public void RemoveListeners(Action[] listeners)
    {
        foreach (Action listener in listeners)
            myEvent -= listener;
    }

    public int GetListenerCount() => myEvent.GetInvocationList().Length;
    public int GetUniqueListenerCount() => myEvent.GetInvocationList().GroupBy(x => x.Target).Select(x => x.First()).ToArray().Length;
    public Action GetListener(int index) => (Action)myEvent.GetInvocationList().GetValue(index);
    public T GetTargetOfType<T>() where T : class
    {
        T a = null;

        for (int i = 0; i < myEvent.GetInvocationList().Length; i++)
        {
            if(GetListener(i).Target is T)
            {
                a = GetListener(i).Target as T;
                break;
            }
        }

        return a == null ? default : a;
    }
    public T GetTargetOfType<T>(out T target) where T : class
    {
        T a = null;
        target = null;

        for (int i = 0; i < myEvent.GetInvocationList().Length; i++)
        {
            if (GetListener(i).Target is T)
            {
                a = GetListener(i).Target as T;
                target = GetListener(i).Target as T;
                break;
            }
        }

        return a == null ? default : a;
    }
    public void ForeachListener(Action onGetListener)
    {
        for (int i = 0; i < myEvent.GetInvocationList().Length; i++)
            onGetListener?.Invoke();
    }
    public void ForeachListener(Action onGetListener, out int index)
    {
        index = 0;
        for (int i = 0; i < myEvent.GetInvocationList().Length; i++)
        {
            onGetListener?.Invoke();
            index = i;
        }
    }
    public void ForeachListener(Action<MethodInfo, object> onGetListener)
    {
        for (int i = 0; i < myEvent.GetInvocationList().Length; i++)
            onGetListener?.Invoke(GetListener(i).Method, GetListener(i).Target);
    }
    public void ForeachTarget(Action<object> onGetTarget)
    {
        Delegate[] delegates = myEvent.GetInvocationList().GroupBy(x => x.Target).Select(y => y.First()).ToArray();

        for (int i = 0; i < delegates.Length; i++)
        {
            onGetTarget?.Invoke(GetListener(i).Target);
        }
    }
    public void ClearListeners()
    {
        foreach (Action i in myEvent.GetInvocationList())
            myEvent -= i;
    }
}

[Serializable] public class mAction<T>
{
    private event Action<T> myEvent;

    public int listenerCount { get { return GetListenerCount(); } }
    public int uniqueListenerCount { get { return GetUniqueListenerCount(); } }

    public void Invoke(T parameter) => myEvent?.Invoke(parameter);
    public void InvokeOn(object target)
    {
        for (int i = 0; i < myEvent.GetInvocationList().Length; i++)
        {
            Action<T> listener = GetListener(i);

            if (listener.Target == target)
            {
                listener.Method.Invoke(listener.Target, listener.Method.GetParameters());
                break;
            }
        }
    }
    public void AddListener(Action<T> listener) => myEvent += listener;
    public void AddListeners(Action<T>[] listeners)
    {
        foreach (Action<T> listener in listeners)
            myEvent += listener;
    }
    public void RemoveListener(Action<T> listener) => myEvent -= listener;
    public void RemoveListeners(Action<T>[] listeners)
    {
        foreach (Action<T> listener in listeners)
            myEvent += listener;
    }

    public int GetListenerCount() => myEvent.GetInvocationList().Length;
    public int GetUniqueListenerCount() => myEvent.GetInvocationList().GroupBy(x => x.Target).Select(x => x.First()).ToArray().Length;
    public Action<T> GetListener(int index) => (Action<T>)myEvent.GetInvocationList().GetValue(index);
    public W GetTargetOfType<W>() where W : class
    {
        W a = null;

        for (int i = 0; i < myEvent.GetInvocationList().Length; i++)
        {
            if (GetListener(i).Target is W)
            {
                a = GetListener(i).Target as W;
                break;
            }
        }

        return a == null ? default : a;
    }
    public W GetTargetOfType<W>(out W target) where W : class
    {
        W a = null;
        target = null;

        for (int i = 0; i < myEvent.GetInvocationList().Length; i++)
        {
            if (GetListener(i).Target is W)
            {
                a = GetListener(i).Target as W;
                target = GetListener(i).Target as W;
                break;
            }
        }

        return a == null ? default : a;
    }
    public void ForeachListener(Action onGetListener)
    {
        for (int i = 0; i < myEvent.GetInvocationList().Length; i++)
            onGetListener?.Invoke();
    }
    public void ForeachListener(Action onGetListener, out int index)
    {
        index = 0;
        for (int i = 0; i < myEvent.GetInvocationList().Length; i++)
        {
            onGetListener?.Invoke();
            index = i;
        }
    }
    public void ForeachListener(Action<MethodInfo, object> onGetListener)
    {
        for (int i = 0; i < myEvent.GetInvocationList().Length; i++)
            onGetListener?.Invoke(GetListener(i).Method, GetListener(i).Target);
    }
    public void ForeachTarget(Action<object> onGetTarget)
    {
        Delegate[] delegates = myEvent.GetInvocationList().GroupBy(x => x.Target).Select(y => y.First()).ToArray();

        for (int i = 0; i < delegates.Length; i++)
        {
            onGetTarget?.Invoke(GetListener(i).Target);
        }
    }
    public void ClearListeners()
    {
        foreach (Action<T> i in myEvent.GetInvocationList())
            myEvent -= i;
    }
}
