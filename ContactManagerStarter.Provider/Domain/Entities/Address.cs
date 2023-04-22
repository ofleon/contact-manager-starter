namespace ContactManagerStarter.Provider.Domain.Entities;

public class Address : Entity
{
    public string Street1 { get; set; }
    public string Street2 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public int Zip { get; set; }
    public AddressType Type { get; set; }
    public virtual Contact Contact { get; set; }
}
