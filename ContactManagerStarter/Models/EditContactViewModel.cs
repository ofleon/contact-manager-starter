using ContactManagerStarter.Provider.Domain.Entities;

namespace ContactManager.Models
{
    public class EditContactViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public IEnumerable<EmailAddress> EmailAddresses { get; set; }
        public IEnumerable<Address> Addresses { get; set; }
    }
}
