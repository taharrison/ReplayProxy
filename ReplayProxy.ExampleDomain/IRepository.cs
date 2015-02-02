using System.Collections.Generic;

namespace ReplayProxy.ExampleDomain
{
    public interface IRepository
    {
        List<Contact> GetContactsByPostcode(string postcode);
        Contact GetContactById_OrNullIfMissing(int id);
        ContactListResult GetContactsWithMissingInformationByFilters(ContactsWithMissingInformationByFiltersRequest request);
    }
}