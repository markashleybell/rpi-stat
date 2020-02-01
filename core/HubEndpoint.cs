namespace core
{
    public static class HubEndpoint
    {
        public static readonly string SendMessage = "SendMessage";
        public static readonly string ReceiveMessage = "ReceiveMessage";
        public static readonly string SendTemperature = "SendTemperature";
        public static readonly string ReceiveTemperature = "ReceiveTemperature";
        public static readonly string RequestHeatingState = "RequestHeatingState";
        public static readonly string ReceiveHeatingStateRequest = "ReceiveHeatingStateRequest";
        public static readonly string ConfirmHeatingState = "ConfirmHeatingState";
        public static readonly string ReceiveHeatingStateConfirmation = "ReceiveHeatingStateConfirmation";
    }
}
