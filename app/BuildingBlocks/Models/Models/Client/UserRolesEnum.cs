namespace WTM.Models;

[Flags]
public enum UserRoleEnum : short
{
    None = 0,
    WPHOperator = 1 << 5,
    AffiliateFather = 1 << 9,
    ECAdmin = 1 << 11,
    SupportTeamAdmin = 1 << 12,
    CompanyAdmin = 1 << 14
}