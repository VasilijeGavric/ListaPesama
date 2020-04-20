using SQLite;

namespace ListaPesama_SignalRClient.Models
{
    [Table("Songs")]
    public class Song
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int OrderNumber { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
    }
}