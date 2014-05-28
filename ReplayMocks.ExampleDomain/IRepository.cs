using System.Collections.Generic;

namespace ReplayMocks.ExampleDomain
{
    public interface IRepository
    {
        List<Contact> GetContactsByPostcode(string postcode);
        Contact GetContactById_OrNullIfMissing(int id);
        ContactListResult GetContactsWithMissingInformationByFilters(ContactsWithMissingInformationByFiltersRequest request);
    }
}