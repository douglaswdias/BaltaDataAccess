namespace BaltaDataAccess.Models
{
  public class Career
  {
    public Career()
    {
        CareerItems = new List<CareerItem>();
    }

    public Guid Id { get; set; }
    public String Title { get; set; }
    public IList<CareerItem> CareerItems { get; set; }
  }
}