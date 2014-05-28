using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ReplayMocks.ExampleDomain
{
    public class ContactListResult
    {
        [XmlArray("contacts")]
        [XmlArrayItem("contact")]
        public List<Contact> Contacts { get; set; } 
    }

    public class Contact
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address HomeAddress { get; set; }
        public List<Order> Orders { get; set; }
    }

    public class Address
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
    }

    public class Order
    {
        public long Id { get; set; }
        public decimal Value { get; set; }
    }

    public class ContactsWithMissingInformationByFiltersRequest
    {
        public bool? HasOrders { get; set; }
        public bool? HasAddress { get; set; }
        public int MaxResults { get; set; }
    }


    public class Repository : IRepository
    {
        private List<Contact> _inMemoryContactCollection = new List<Contact>()
        {
            new Contact() { Id = 15223, FirstName = "John", LastName = "Smith", 
                HomeAddress = new Address() { StreetAddress = "19 Somestreet", Postcode = "SM15 7ET"} },
            new Contact() { Id = 15224, FirstName = "Jane", LastName = "Smith", 
                HomeAddress = new Address() { StreetAddress = "19 Somestreet", Postcode = "SM15 7ET"},
                Orders = new List<Order>() { new Order() { Id = 16346133, Value = 140.22m}, new Order() { Id = 134624564, Value = 44.13m} }},
            new Contact() { Id = 12343, FirstName = "Tony", LastName = "Todd", 
                Orders = new List<Order>() { new Order() { Id = 1345134, Value = 123.33m}}},
            new Contact() { Id = 23422, FirstName = "Gates", LastName = "McFadden", 
                HomeAddress = new Address() { StreetAddress = "7 Rigel St", Postcode = "RG7 7RG"} },
        };

        public List<Contact> GetContactsByPostcode(string postcode)
        {
            return _inMemoryContactCollection
                .Where(x => x.HomeAddress != null && x.HomeAddress.Postcode == postcode)
                    .ToList();
        }

        public Contact GetContactById_OrNullIfMissing(int id)
        {
            return _inMemoryContactCollection.FirstOrDefault(x => x.Id == id);
        }

        public ContactListResult GetContactsWithMissingInformationByFilters(ContactsWithMissingInformationByFiltersRequest request)
        {
            Thread.Sleep(70); // imagine this is a slow operation
            var list =  _inMemoryContactCollection.
                Where(x => (!request.HasAddress.HasValue || (x.HomeAddress != null) == request.HasAddress.Value )
                && (!request.HasOrders.HasValue || (x.Orders != null) == request.HasOrders.Value))
                .Take(request.MaxResults)
                .ToList();
            return new ContactListResult() { Contacts = list};
        }
    }
}
