namespace SQLAzureDemo.Database.Models
{
    public class Movie
    {
        public virtual int Id { get; set; }
        public virtual string Title { get; set; }
        public virtual int Year { get; set; }
    }
}