using System.Collections.Specialized;
using WpfApp.Logic.GeneratedValueHandlers;

namespace WpfApp.UnitTests;

internal class HandlerWrapper
{
    private BaseGeneratedValueHandler OriginalHandler { get; init; }

    private HandleState HandleState { get; init; }

    public HandlerWrapper(BaseGeneratedValueHandler originalHandler, HandleState handleState)
    {
        OriginalHandler = originalHandler;
        HandleState = handleState;
    }

    public void GeneratedValues_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var task = Task.Run(async () =>
        {
            await OriginalHandler.GeneratedValues_CollectionChanged(sender, e);
        });

        task.Wait();
        HandleState.WorkCount = Interlocked.Increment(ref HandleState.WorkCount);
    }

    public void WaitWhenHandlerEndWork(int workCount)
    {
        while (HandleState.WorkCount < 2)
        {
            Thread.Sleep(100);
        }
    }
}

internal class HandleState
{
    public int WorkCount = 0;
}