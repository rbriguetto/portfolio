namespace SmartApps.Jobs.Domain;

[Serializable]
public class JobAlreadyStartedException: Exception
{
    public JobAlreadyStartedException()
    {

    }

    public JobAlreadyStartedException(string message)
        : base(message) 
    {

    }

    public JobAlreadyStartedException(string message, Exception inner)
        : base(message, inner)
    {

    }
}