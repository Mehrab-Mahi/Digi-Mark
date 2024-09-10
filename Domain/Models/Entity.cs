namespace Domain.Models;

public class Entity
{
    public string Id { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime LastModifiedTime { get; set; }
    public Boolean IsDeleted { get; set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
    protected Entity()
        : base()
    {
        Id = Guid.NewGuid().ToString("N");
        CreateTime = DateTime.Now;
    }
}