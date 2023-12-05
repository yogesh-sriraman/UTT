namespace Client
{
    //Make class serializable to be able to save it in a file
    [System.Serializable]
    public class ClientIPPortTuple
    {
        public string clientIP;
        public int clientPort;
    }
}