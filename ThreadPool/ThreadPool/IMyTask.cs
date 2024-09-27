namespace ThreadPool;

public interface IMyTask<TResult>
{
    bool isCompleted();
    TResult Result();
    IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation);
}
