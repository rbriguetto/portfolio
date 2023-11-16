namespace SmartApps.Jobs.Domain;

[Serializable]
public class JobAlreadyFinishedException: Exception
{
    public JobAlreadyFinishedException()
    {
    }

    public JobAlreadyFinishedException(string message)
        : base(message)
    {
    }

    public JobAlreadyFinishedException(string message, Exception inner)
        : base(message, inner)
    {
    }
}