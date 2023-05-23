using System.Diagnostics;

namespace WpfApp.Logic;

public class JobManager<T>
{
    private CancellationTokenSource? _ctSource;
    private bool _isStarted;
    private int? _threadId;

    public JobManager()
    {
        _isStarted = false;
    }

    public int? ThreadId => _threadId;

    /// <summary>
    /// Запустить выполнение делегата в отдельном потоке с указанным интервалом, только если еще не запущено.
    /// </summary>
    /// <param name="action"><see cref="Action"/></param>
    /// <param name="withInterval">Интервал</param>
    public void StartRepeatJob(Action action, TimeSpan withInterval)
    {
        if (!_isStarted)
        {
            _ctSource = new CancellationTokenSource();
            new Thread(() =>
            {
                while (!_ctSource.Token.IsCancellationRequested)
                {
                    Stopwatch stopWatch = new();
                    stopWatch.Start();
                    _threadId = Thread.CurrentThread.ManagedThreadId;
                    action.Invoke();
                    stopWatch.Stop();
                    Thread.Sleep(withInterval - stopWatch.Elapsed);
                }
            }).Start();

            _isStarted = true;
        }
    }

    /// <summary>
    /// Остановить выполнение делегата, если было запущено.
    /// </summary>
    public void StopRepeatJob()
    {
        if (_isStarted)
        {
            _threadId = null;
            _ctSource?.Cancel();
            _isStarted = false;
        }
    }
}