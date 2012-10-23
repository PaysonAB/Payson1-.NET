namespace PaysonIntegration.Utils
{
    public enum GuaranteeStatus
    {
        WaitingForSend,
        WaitingForAcceptance,
        WaitingForReturn,
        WaitingForReturnAcceptance,
        ReturnNotAccepted,
        NotReceived,
        ReturnNotReceived,
        MoneyReturnedToSender,
        ReturnAccepted,
    }
}