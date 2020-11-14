namespace LightTable.Controller.Connection
{
    public enum ConnectionState
    {
        Disconnected,
        Connected,
        Enumerating,
        Connecting,
        Failure,
        Enumerated
    }
}