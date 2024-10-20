namespace WTM.Models;

public class Company : BaseEntity
{
    public Status Status { get; set; }
    public string Name { get; set; }
    public string NationalCode { get; set; }
    public string AkiteoCode { get; set; }
    public string FinessGeoCode { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public int? PhoneCountryCode { get; set; }
    public string City { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string PostalCode { get; set; }
}