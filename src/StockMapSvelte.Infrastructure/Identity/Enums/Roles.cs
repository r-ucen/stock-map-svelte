namespace StockMapSvelte.Infrastructure.Identity.Enums;

public enum Roles
{
    Admin,
    Manager,
    // Customer role is not used (there is an existing IsCustomer policy)
    Customer
}