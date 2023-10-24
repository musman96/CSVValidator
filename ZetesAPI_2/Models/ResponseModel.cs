namespace ZetesAPI_2.Models
{
    public class ResponseModel
    {
        public string status { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public string data { get; set; }
        public bool isSuccessful { get; set; }
        public string stackTrace { get; set; }
        public string innerException { get; set; }
        public string innerExceptionStackTrace { get; set; }

    }
}
