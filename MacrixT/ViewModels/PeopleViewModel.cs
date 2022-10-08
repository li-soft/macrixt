using System;
using System.Collections.ObjectModel;
using MacrixT.Models;
using MacrixT.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace MacrixT.ViewModels;

public class PeopleViewModel : ObservableRecipient
{
    private readonly IPeopleService _peopleService;

    public ObservableCollection<Person> People { get; set; } = new ();
    
    public PeopleViewModel(IPeopleService peopleService)
    {
        _peopleService = peopleService;
        
        People.Add(new Person
        {
            FirstName = "fi",
            LastName = "sadasd",
            ApartmentNumber = "2b",
            DateOfBirth = new DateTime(1985, 06, 06),
            HouseNumber = "14ca",
            PhoneNumber = "+4872761545",
            PostalCode = "22-22",
            StreetName = "hhha"
        });
    }
}