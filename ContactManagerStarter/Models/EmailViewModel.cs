using ContactManagerStarter.Provider.Domain.Entities;

namespace ContactManager.Models
{
    public class EmailViewModel
    {
        public EmailType Type { get; set; }
        public string Email { get; set; }
        public bool IsPrimary { get; set; }
    }
}
