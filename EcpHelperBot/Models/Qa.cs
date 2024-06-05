using System.ComponentModel.DataAnnotations.Schema;

namespace EcpHelperBot.Models
{
    public class Qa
    {
        public int QaId { get; set; }
        public string Quest { get; set; }
        public string? Answ { get; set; }
        public string? Hyper { get; set; }
        public int? Number { get; set; }
        public string? UserMessage { get; set; }
        public int? Status { get; set; }
        public string? StatusMessage { get; set; }
        public int Active { get; set; }
        public string? Org { get; set; }
        public string? Contact { get; set; }

    }
}
