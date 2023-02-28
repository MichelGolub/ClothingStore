namespace Identity.Entities
{
    public class OperationResult
    {
        public Statuses Status { get; set; }
        public string Message { get; set; }
        public enum Statuses
        { 
            Ok,
            Error
        }
    }
}
