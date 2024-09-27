namespace ThreadPool;

internal class MyTask<TResult> : IMyTask<TResult>
{
    private readonly Func<TResult> _task;
    private readonly FixedThreadPool _pool;
    private readonly EventWaitHandle _handle;
    private volatile object? _result = null;
    private volatile Exception? _exception = null;
    private volatile bool _completed = false;

    public MyTask(Func<TResult> task, FixedThreadPool pool)
    {
        _task = task;
        _handle = new EventWaitHandle(false, EventResetMode.ManualReset);
        _pool = pool;
    }

    public MyTask(Action task, FixedThreadPool pool)
    {
        _task = () =>
        {
            task.Invoke();
            return default;
        };
        _handle = new EventWaitHandle(false, EventResetMode.ManualReset);
        _pool = pool;
    }

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation)
    {
        return _pool.Enqueue(() =>
        {
            return continuation.Invoke(Result());
        });

    }

    public bool isCompleted()
    {
        return _completed;
    }

    public TResult Result()
    {
        _handle.WaitOne();

        if (_exception == null) return (TResult)_result;

        throw new AggregateException(_exception);
    }

    public void Invoke()
    {
        try
        {
            var result = _task.Invoke();
            _result = result;
        }
        catch (Exception e)
        {
            _exception = e;
        }
        finally
        {
            _completed = true;
            _handle.Set();
        }
    }
}