namespace Domain
{
    public class Activity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Category { get; set; } // Category is a string because we are not going to create a Category class
        public string City { get; set; }
        public string Venue { get; set; }
        
    }
}