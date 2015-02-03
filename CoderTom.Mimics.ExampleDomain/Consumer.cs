using System;
using System.Linq;
using System.Text;

namespace CoderTom.Mimics.ExampleDomain
{
    public class Consumer
    {
        private IRepository _repository;

        public Consumer(IRepository repository)
        {
            _repository = repository;
        }

        public string GetMissingAddressesReport()
        {
            var contacts = _repository.GetContactsWithMissingInformationByFilters(
                new ContactsWithMissingInformationByFiltersRequest() {HasAddress = false, HasOrders = null, MaxResults = 100 })
                .Contacts;
            var missingcontactIds = contacts.Select(c => c.Id.ToString());
            var sb = new StringBuilder();
            sb.AppendLine("Missing addresses report:");
            sb.AppendLine(String.Format("Total contacts missing addresses: {0}", contacts.Count));
            sb.AppendLine("Contact Ids:" + string.Join(", ", missingcontactIds));
            return sb.ToString();
        }

        public decimal GetOrderTotalForContact(int id)
        {
            var contact = _repository.GetContactById_OrNullIfMissing(id);
            if (contact == null)
            {
                throw new ApplicationException("contact missing");
            }

            return contact.Orders.Sum(o => o.Value);
        }
    }
}
