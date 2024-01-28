namespace BaltaDataAccess.Models
{
  public class CareerItem
  {
    public Guid Id { get; set; }
    public String Title { get; set; }
    public Course Course { get; set; }
  }
}