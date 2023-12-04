using System.Diagnostics;
using ThreadPool;

namespace UnitTest;
public class FixedThreadPoolTest
{
    private volatile bool _work;

    [Test]
    public void AllthreadsSuspendedAfterCreation()
    {
        var pool = new FixedThreadPool(4);
        Assert.That(pool.SuspendedCount, Is.EqualTo(4));
    }

    [Test]
    public void IMyTaskResultReturnResult()
    {
        var pool = new FixedThreadPool(4);
        var task = pool.Enqueue(() => 42);
        Assert.That(task.Result(), Is.EqualTo(42));
    }

    [Test]
    public void IMyTaskContinueWithReturnResult()
    {
        var pool = new FixedThreadPool(4);
        var task = pool.Enqueue(() => 41);
        var continueWithTask = task.ContinueWith((val) => val + 1);
        Assert.That(continueWithTask.Result(), Is.EqualTo(42));
    }

    [Test]
    public void OnlyOneThreadResumesOnOneTask()
    {
        var pool = new FixedThreadPool(4);
        _work = true;
        var task = pool.Enqueue(() =>
        {
            int i = 0;
            while (_work)
            {
                i++;
            }
            return i;
        });
        Thread.Sleep(100); // wait until thread still our task
        Assert.That(pool.SuspendedCount, Is.EqualTo(3));
    }

    [Test]
    public void ThreadSuspendsAfterWorking()
    {
        var pool = new FixedThreadPool(4);
        _work = true;
        var task = pool.Enqueue(() =>
        {
            int i = 0;
            while (_work)
            {
                i++;
            }
            return i;
        });
        Assert.That(pool.SuspendedCount, Is.EqualTo(3));
        _work = false;
        task.Result();
        Assert.That(task.isCompleted(), Is.True);
        Thread.Sleep(1000); // believe thread will be suspended in this time
        Assert.That(pool.SuspendedCount, Is.EqualTo(4));
    }

    [Test]
    public void ResultThrowsExceptionIfItOccuredInTask()
    {
        var pool = new FixedThreadPool(4);
        var task = pool.Enqueue(() =>
        {
            throw new ArgumentException("Random exception");
        });
        try
        {
            task.Result();
        }
        catch (AggregateException e)
        {
            Assert.IsTrue(e.InnerException is ArgumentException);
        }
    }

    [Test]
    public void AddingTasksAfterDisposeThrowsException()
    {
        var pool = new FixedThreadPool(4);
        _work = true;
        var task = pool.Enqueue(() =>
        {
            while (_work) { }
        });
        new Thread(() => pool.Dispose()).Start();
        try
        {
            pool.Enqueue(() => 42);
        }
        catch (Exception e)
        {
            Assert.IsTrue(e is ArgumentException);
        }
    }

    [Test]
    public void AllTasksCompletedBeforeDispose()
    {
        var pool = new FixedThreadPool(4);
        _work = true;
        var tasksCount = 1000;
        var tasks = new IMyTask<int>[tasksCount];
        for (int i = 0; i < tasksCount; i++)
        {
            tasks[i] = pool.Enqueue(() =>
            {
                while (_work) { }
                return 42;
            });
        }
        _work = false;
        bool isCompleted = false;
        int tryCount = 10000; // believe it`s enough to make all tasks
        while (!isCompleted && tryCount-- > 0)
        {
            isCompleted = true;
            foreach (var task in tasks)
            {
                isCompleted = isCompleted && task.isCompleted();
            }
        }

        foreach (var task in tasks)
        {
            Assert.Multiple(() =>
            {
                Assert.That(task.isCompleted(), Is.True);
                Assert.That(task.Result(), Is.EqualTo(42));
            });
        }
    }

    [Test]
    public void AllTasksCompletedAfterDispose()
    {
        var pool = new FixedThreadPool(4);
        _work = true;
        var tasksCount = 100;
        var tasks = new IMyTask<int>[tasksCount];
        for (int i = 0; i < tasksCount; i++)
        {
            tasks[i] = pool.Enqueue(() =>
            {
                while (_work) { }
                return 42;
            });
        }
        _work = false;
        pool.Dispose();
        foreach (var task in tasks)
        {
            Assert.Multiple(() =>
            {
                Assert.That(task.isCompleted(), Is.True);
                Assert.That(task.Result(), Is.EqualTo(42));
            });
        }
    }

    [Test]
    public void ThreadPoolCreatingThreads()
    {
        var threadCountBeforeCreation = Process.GetCurrentProcess().Threads.Count;
        using (var pool = new FixedThreadPool(4))
        {
            Assert.That(Process.GetCurrentProcess().Threads.Count - threadCountBeforeCreation, Is.EqualTo(4));
        }
        Assert.That(Process.GetCurrentProcess().Threads.Count, Is.EqualTo(threadCountBeforeCreation));
    }
}