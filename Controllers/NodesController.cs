using Microsoft.AspNetCore.Mvc;
using TreeNodes.Data;
using TreeNodes.Exceptions;
using TreeNodes.Models;

namespace TreeNodes.Controllers
{
    [ApiController]
    [Route("api/nodes")]
    public class NodesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public NodesController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetNodes(int? treeId)
        {
            List<Node> nodes;

            if (treeId.HasValue)
            {
                nodes = _dbContext.Nodes.Where(n => n.TreeId == treeId.Value).ToList();
            }
            else
            {
                nodes = _dbContext.Nodes.ToList();
            }

            return Ok(nodes);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateNode(int id, NodeRequest updatedNode)
        {
            var node = _dbContext.Nodes.Find(id);
            if (node == null)
            {
                throw new SecureException("Node is not found", Guid.NewGuid().ToString());
            }

            node.Name = updatedNode.Name;
            node.TreeId = updatedNode.TreeId;
            _dbContext.SaveChanges();

            return NoContent();
        }

        [HttpPost]
        public IActionResult CreateNode([FromBody] NodeRequest node)
        {
            if (string.IsNullOrEmpty(node.Name))
            {
                throw new SecureException("Node Name can not be null", Guid.NewGuid().ToString());
            }

            _dbContext.Nodes.Add(new Node { Name = node.Name, TreeId = node.TreeId});
            _dbContext.SaveChanges();

            return Ok(node);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteNode(int id)
        {
            var node = _dbContext.Nodes.Find(id);
            if (node == null)
            {
                throw new SecureException("Node is not found", Guid.NewGuid().ToString());
            }

            _dbContext.Nodes.Remove(node);
            _dbContext.SaveChanges();

            return NoContent();
        }
    }
}
