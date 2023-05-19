namespace WpfApp.Logic;

public class JobManager<T>
{
    private CancellationTokenSource? _ctSource;
    private bool _isStarted;
    
    public JobManager()
    {
        _isStarted = false;
    }

    public void StartRepeatJob(Action action, TimeSpan withInterval)
    {
        if (!_isStarted)
        {
            _ctSource = new CancellationTokenSource();
            Task.Run(() =>
            {
                while (!_ctSource.Token.IsCancellationRequested)
                {
                    action.Invoke();
                    Thread.Sleep(withInterval);
                }
            });
            _isStarted = true;
        }
    }

    public void StopRepeatJob()
    {
        if (_isStarted)
        {
            _ctSource?.Cancel();
            _isStarted = false;
        }
    }
}