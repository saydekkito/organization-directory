namespace organization_directory.Models;

public class Organization {
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string INN { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string ReceptionPhone { get; set; } = string.Empty;

    public string HRPhone { get; set; } = string.Empty;

    public string AccountingPhone { get; set; } = string.Empty;
}