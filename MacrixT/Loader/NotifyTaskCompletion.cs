using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace MacrixT.Loader;

public class NotifyTaskCompletion<TResult> : ObservableRecipient
{
    public TResult? Result => (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : default;
    public TaskStatus Status => Task.Status;
    public bool IsCompleted => Task.IsCompleted;
    public bool IsNotCompleted => !Task.IsCompleted;
    public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;
    public bool IsCanceled => Task.IsCanceled;
    public bool IsFaulted => Task.IsFaulted;
    public AggregateException? Exception => Task.Exception;
    public Exception? InnerException => Exception?.InnerException;
    public string? ErrorMessage => InnerException?.Message;
    public Task<TResult?> Task { get; }

    public NotifyTaskCompletion(Task<TResult?> task, Action<object?, PropertyChangedEventArgs> onTaskDoneDataAssign)
    {
        PropertyChanged += (o, a) => onTaskDoneDataAssign(o, a);
        Task = task;
        _ = WatchTaskAsync(task);
    }

    private async Task WatchTaskAsync(Task task)
    {
        try
        {
            await task;
        }
        catch (Exception e)
        {
            // log
        }
    
        OnPropertyChanged(nameof(Status));
        OnPropertyChanged(nameof(IsCompleted));
        OnPropertyChanged(nameof(IsNotCompleted));

        if (task.IsCanceled)
        {
            OnPropertyChanged(nameof(IsCanceled));
        }
        else if(task.IsFaulted)
        {
            OnPropertyChanged(nameof(IsFaulted));
            OnPropertyChanged(nameof(Exception));
            OnPropertyChanged(nameof(InnerException));
            OnPropertyChanged(nameof(ErrorMessage));
        }
        else
        {
            OnPropertyChanged(nameof(IsSuccessfullyCompleted));
            OnPropertyChanged(nameof(Result));
        }
    }
}