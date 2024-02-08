namespace MISBackend.DAL.Entity
{
    public class Base
    {
        public DateTime DateEntry { get; set; }
        public Guid UserIdEntry { get; set; }
        public DateTime DateEdit { get; set; }
        public Guid UserIdEdit { get; set; }
        public DateTime DateDelete { get; set; }
        public Guid UserIdDelete { get; set; }
        public bool Deleted { get; set; }
    }
}
