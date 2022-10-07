using System.Collections.ObjectModel;
using MacrixT.Models;
using MacrixT.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace MacrixT.ViewModels;

public class MainViewModel : ObservableRecipient
{
    private readonly IPeopleService _peopleService;

    public ObservableCollection<Person> People { get; set; } = new ();
    
    public MainViewModel(IPeopleService peopleService)
    {
        _peopleService = peopleService;
        
        People.Add(new Person
        {
            LastName = "sadasd"
        });
    }
}