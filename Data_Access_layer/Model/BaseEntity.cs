namespace Data_Access_layer.Model
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int CreatedBy { get; internal set; }
    }
}