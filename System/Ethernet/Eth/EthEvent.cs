namespace System.Deal
{
    [Serializable]
    public class EthEvent : Deputy
    {
        public EthEvent(string MethodName, object TargetClassObject, params object[] parameters)
            : base(TargetClassObject, MethodName)
        {
            base.ParameterValues = parameters;
        }

        public EthEvent(string MethodName, string TargetClassName, params object[] parameters)
            : base(TargetClassName, MethodName)
        {
            base.ParameterValues = parameters;
        }
    }
}
