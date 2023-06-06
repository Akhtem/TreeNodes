using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TreeNodes.Models
{
    public class Tree
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<Node>? Nodes { get; set; }
    }

    public class TreeRequest
    {
        public string? Name { get; set; }
    }
}
