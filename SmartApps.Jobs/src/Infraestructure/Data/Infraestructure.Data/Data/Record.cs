namespace Infraestructure.Data;

public abstract class Record : IRecord
{
    public virtual int Id { get; set; }
}