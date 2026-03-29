using System.ComponentModel.DataAnnotations.Schema;

namespace StockMapSvelte.Domain.Entities;

[Table(nameof(DeploymentTest))]
public class DeploymentTest : Entity<int>
{
    public string TestValue { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}