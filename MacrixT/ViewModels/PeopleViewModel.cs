using System;
using System.Collections.ObjectModel;
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

    private NotifyTaskCompletion<Person[]> _peopleTask;
    public NotifyTaskCompletion<Person[]> PeopleTask
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
            SaveCommand.NotifyCanExecuteChanged();
            CancelCommand.NotifyCanExecuteChanged();
        }
    }

    public PeopleViewModel(IPeopleService peopleService)
    {
        _peopleService = peopleService;

        PeopleTask = new NotifyTaskCompletion<Person[]?>(Load(), AssignData);
        PeopleResult = new ObservableCollection<Person>();

        SaveCommand = new AsyncRelayCommand(Save, CanSave);
        CancelCommand = new RelayCommand(Cancel, () => IsInEdit);
        RemoveCommand = new RelayCommand<object>(DeleteRow);
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
            person.PropertyChanged += (_, __) => IsInEdit = true;
            PeopleResult.Add(person);
        }

        if (PeopleResult.Any())
        {
            IsInEdit = false;
        }
        
        PeopleResult.CollectionChanged += (_, __) => IsInEdit = true;
    }

    private async Task<Person[]> Load()
    {
        var storedPeople = await _peopleService.GetStoredPeople();
        return storedPeople;
    }

    private void Cancel()
    {
        PeopleTask = new NotifyTaskCompletion<Person[]>(Load(), AssignData);
        IsInEdit = false;
    }

    private async Task Save()
    {
        await _peopleService.DumpPeople(PeopleResult);
        IsInEdit = false;
    }

    private void DeleteRow(object? obj)
    {
        if (obj is Person person)
        {
            PeopleResult.Remove(person);
        }
    }
}