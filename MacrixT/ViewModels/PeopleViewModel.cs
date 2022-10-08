using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using MacrixT.Loader;
using MacrixT.Models;
using MacrixT.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace MacrixT.ViewModels;

public class PeopleViewModel : ObservableRecipient
{
    private readonly IPeopleService _peopleService;

    public AsyncRelayCommand SaveCommand { get; }
    public RelayCommand CancelCommand { get; }
    public RelayCommand<object> RemoveCommand { get; }
    public RelayCommand AddItemCommand { get; }

    private NotifyTaskCompletion<ObservableCollection<Person>?> _peopleTask;
    public NotifyTaskCompletion<ObservableCollection<Person>?> PeopleTask
    {
        get => _peopleTask;
        set => SetProperty(ref _peopleTask, value);
    }

    public ObservableCollection<Person> PeopleResult { get; }

    private bool _isInEdit;
    private bool IsInEdit
    {
        get => _isInEdit;
        set
        {
            _isInEdit = value;
            if (!value)
            {
                return;
            }
            
            SaveCommand.NotifyCanExecuteChanged();
            CancelCommand.NotifyCanExecuteChanged();
            RemoveCommand.NotifyCanExecuteChanged();
        }
    }

    public PeopleViewModel(IPeopleService peopleService)
    {
        _peopleService = peopleService;

        PeopleTask = new NotifyTaskCompletion<ObservableCollection<Person>?>(Load(), AssignData);
        PeopleResult = new ObservableCollection<Person>();

        SaveCommand = new AsyncRelayCommand(Save, CanSave);
        CancelCommand = new RelayCommand(Cancel, () => IsInEdit);
        RemoveCommand = new RelayCommand<object>(DeleteRow, CanDeleteRow);
        AddItemCommand = new RelayCommand(AddItem);
    }

    private void AddItem()
    {
        var person = new Person { Id = Guid.NewGuid() };
        person.PropertyChanged += (_, __) =>
        {
            IsInEdit = true;
        };
        
        PeopleResult.Add(person);
    }

    private static bool CanDeleteRow(object? obj) => obj is Person person && person.IsValid();

    private bool CanSave() => IsInEdit && PeopleResult.All(p => p.IsValid());

    private void AssignData(object? _, PropertyChangedEventArgs args)
    {
        if (args.PropertyName?.Equals(nameof(PeopleTask.IsSuccessfullyCompleted)) != true
            || !PeopleTask.IsSuccessfullyCompleted)
        {
            return;
        }
        
        PeopleResult.Clear();
        foreach (var person in PeopleTask.Result!)
        {
            PeopleResult.Add(person);
        }

        PeopleResult.CollectionChanged += (_, colArgs) =>
        {
            if (colArgs.Action == NotifyCollectionChangedAction.Add)
            {
                var newItems = colArgs.NewItems?.Cast<Person>() ?? Enumerable.Empty<Person>();
                var oldItems = colArgs.OldItems?.Cast<Person>() ?? Enumerable.Empty<Person>();
                var addedItem = newItems.Except(oldItems);
                foreach (var person in addedItem)
                {
                    person.PropertyChanged += (_, __) =>
                    {
                        IsInEdit = true;
                    };
                }
            }
            IsInEdit = true;
        };
        
        foreach (var person in PeopleResult)
        {
            person.PropertyChanged += (_, __) => IsInEdit = true;
        }
    }


    private async Task<ObservableCollection<Person>> Load()
    {
        var storedPeople = await _peopleService.GetStoredPeople();
        return new ObservableCollection<Person>(storedPeople);
    }

    private void Cancel() => PeopleTask = new NotifyTaskCompletion<ObservableCollection<Person>?>(Load(), AssignData);

    private async Task Save()
    {
        await _peopleService.DumpPeople(PeopleResult);
        PeopleTask = new NotifyTaskCompletion<ObservableCollection<Person>?>(Load(), AssignData);
    }

    private void DeleteRow(object? obj)
    {
        if (obj is Person person)
        {
            PeopleResult.Remove(person);
        }
    }
}