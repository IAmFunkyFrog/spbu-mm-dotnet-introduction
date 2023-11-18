using System.Collections.Concurrent;

namespace ThreadPool;

public class FixedThreadPool : IDisposable
{
    private readonly ConcurrentQueue<Action> _tasks;
    private readonly Thread[] _threads;
    private readonly EventWaitHandle _waitTaskHandle;
    private readonly CancellationTokenSource _cancellationTokenSource;
    public int SuspendedCount
    {
        get
        {
            int count = 0;
            foreach (var thread in _threads)
            {
                if (thread.ThreadState == ThreadState.WaitSleepJoin) count++;
            }
            return count;
        }
    }

    public FixedThreadPool(int threadCount)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _tasks = new ConcurrentQueue<Action>();
        _waitTaskHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        _threads = new Thread[threadCount];
        for (int i = 0; i < threadCount; i++)
        {
            var token = _cancellationTokenSource.Token;
            var thread = new Thread(() =>
            {
                WaitHandle[] handles = new WaitHandle[] { _waitTaskHandle, token.WaitHandle };
                while (!token.IsCancellationRequested || !_tasks.IsEmpty)
                {
                    if (!token.IsCancellationRequested) WaitHandle.WaitAny(handles);

                    _tasks.TryDequeue(out Action? task);
                    task?.Invoke();
                }
            });
            _threads[i] = thread;
            thread.Start();
        }
    }

    public IMyTask<TResult> Enqueue<TResult>(Func<TResult> task)
    {
        ThrowExceptionIfCancellationRequested();
        var packedTask = new MyTask<TResult>(() => task.Invoke(), this);
        _tasks.Enqueue(() => packedTask.Invoke());
        _waitTaskHandle.Set();
        return packedTask;
    }

    public IMyTask<object?> Enqueue(Action task)
    {
        ThrowExceptionIfCancellationRequested();
        var packedTask = new MyTask<object?>(() => task.Invoke(), this);
        _tasks.Enqueue(() => packedTask.Invoke());
        _waitTaskHandle.Set();
        return packedTask;
    }

    private void ThrowExceptionIfCancellationRequested()
    {
        if (_cancellationTokenSource.IsCancellationRequested)
        {
            throw new ArgumentException("Cancellation is requested, not expected to enqueue new tasks");
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        foreach (var thread in _threads)
        {
            thread.Join();
        }
    }
}
