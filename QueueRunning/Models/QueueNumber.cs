using System.ComponentModel.DataAnnotations;

namespace QueueRunning.Models
{
    public class QueueNumber
    {
        [Key]
        public int Id { get; set; }
        public string CurrentQueue { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
