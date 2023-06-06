using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TreeNodes.Models
{
    public class Node
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public int TreeId { get; set; }
        public Tree? Tree { get; set; }
    }

    public class NodeRequest
    {
        public string? Name { get; set; }
        public int TreeId { get; set; }
    }
}
