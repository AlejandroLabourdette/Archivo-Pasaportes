﻿using ArchivoDePasaportes.Dto;
using ArchivoDePasaportes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.ViewModels
{
    public class PersonFormViewModel
    {
        public bool ExistOtherInDb { get; set; }
        public bool ValidDate { get; set; }
        public string OldCI { get; set; }
        public bool UserIsAdmin { get; set; }
        public PersonDto PersonDto { get; set; }
        public IEnumerable<Source> Sources { get; set; }
        public List<Person> PeopleList {get;set;}
    }
}
