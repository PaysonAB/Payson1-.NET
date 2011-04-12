namespace PaysonIntegration.Utils
{
    public enum PaymentStatus
    {
        Created,
        Pending,
        Processing,
        Completed,
        Credited,
        Incomplete,
        Error,
        Expired,
        ReversalError,
        Aborted,
    }
}