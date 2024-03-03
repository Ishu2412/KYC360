using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace EmployeeAPI.Entities
{
    public interface IEntity
    {
        public List<Address>? Addresses { get; set; }
        public List<Date> Dates { get; set; }
        public bool Deceased { get; set; }
        public string? Gender { get; set; }

        [Key] // Assuming Id is the primary key
        public string Id { get; set; }

        public List<Name> Names { get; set; }
    }

    public class Employee : IEntity
    {

        public Employee() { }
        public Employee(List<Address> addresses, List<Date> dates, bool deceased, string gender, string id, List<Name> names)
        {
            Addresses = addresses;
            Dates = dates;
            Deceased = deceased;
            Gender = gender;
            Id = id;
            Names = names;
        }

        public List<Address>? Addresses { get; set; }
        public List<Date> Dates { get; set; }
        public bool Deceased { get; set; }
        public string? Gender { get; set; }

        [Key]
        public string Id { get; set; }
        public List<Name> Names { get; set; }
    }

    [Owned]
    public class Address
    {
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }

    [Owned]
    public class Date
    {
        public string? DateType { get; set; }
        public DateTime? DateValue { get; set; }
    }

    [Owned]
    public class Name
    {
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? Surname { get; set; }
    }
}
