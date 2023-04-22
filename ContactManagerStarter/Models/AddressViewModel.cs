using ContactManagerStarter.Provider.Domain.Entities;

namespace ContactManager.Models
{
    public class AddressViewModel
    {
        public AddressType Type { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int Zip { get; set; }
    }
}
