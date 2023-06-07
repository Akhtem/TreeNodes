using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> GetNodes(int? treeId)
        {
            List<Node> nodes;

            if (treeId.HasValue)
            {
                nodes = await _dbContext.Nodes.Where(n => n.TreeId == treeId.Value).ToListAsync();
            }
            else
            {
                nodes = await _dbContext.Nodes.ToListAsync();
            }

            return Ok(nodes);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNode(int id, NodeRequest updatedNode)
        {
            var node = await _dbContext.Nodes.FindAsync(id);
            if (node == null)
            {
                throw new SecureException(Guid.NewGuid().ToString(), "Node is not found");
            }

            node.Name = updatedNode.Name;
            node.TreeId = updatedNode.TreeId;
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNode([FromBody] NodeRequest node)
        {
            if (string.IsNullOrEmpty(node.Name))
            {
                throw new SecureException(Guid.NewGuid().ToString(), "Node Name can not be null");
            }

            _dbContext.Nodes.Add(new Node { Name = node.Name, TreeId = node.TreeId });
            await _dbContext.SaveChangesAsync();

            return Ok(node);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNode(int id)
        {
            var node = await _dbContext.Nodes.FindAsync(id);
            if (node == null)
            {
                throw new SecureException(Guid.NewGuid().ToString(), "Node is not found");
            }

            _dbContext.Nodes.Remove(node);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
