namespace SmartApps.Jobs.Domain;

[Serializable]
public class JobNotFoundException : Exception
{
    public JobNotFoundException()
    {
    }

    public JobNotFoundException(string message)
        : base(message)
    {
    }

    public JobNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
}