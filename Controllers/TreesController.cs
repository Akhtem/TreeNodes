using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeNodes.Data;
using TreeNodes.Exceptions;
using TreeNodes.Models;

namespace TreeNodes.Controllers
{
    [Route("api/trees")]
    [ApiController]
    public class TreesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public TreesController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetTrees()
        {
            List<Tree> trees = _dbContext.Trees.Include(t => t.Nodes).ToList();
            return Ok(trees);
        }

        [HttpGet("{id}")]
        public IActionResult GetTree(int id)
        {
            var tree = _dbContext.Trees.Include(t => t.Nodes).FirstOrDefault(q => q.Id == id);
            if (tree == null)
            {
                return NotFound();
            }
            return Ok(tree);
        }

        [HttpPost]
        public IActionResult CreateTree(TreeRequest tree)
        {
            if (string.IsNullOrEmpty(tree.Name))
            {
                throw new SecureException("Tree Name cannot be null or empty", Guid.NewGuid().ToString());
            }

            var _tree = new Tree { Name = tree.Name };
            _dbContext.Trees.Add(_tree);
            _dbContext.SaveChanges();
            return CreatedAtAction(nameof(GetTree), new { id = _tree.Id }, _tree);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTree(int id, TreeRequest updatedTree)
        {
            var tree = _dbContext.Trees.Find(id);
            if (tree == null)
            {
                throw new SecureException("Tree is not found", Guid.NewGuid().ToString());
            }

            tree.Name = updatedTree.Name;
            _dbContext.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTree(int id)
        {
            var tree = _dbContext.Trees.Find(id);
            if (tree == null)
            {
                throw new SecureException("Tree is not found", Guid.NewGuid().ToString());
            }

            if (_dbContext.Nodes.Any(n => n.TreeId == id))
            {
                throw new SecureException("You have to delete all children nodes first", Guid.NewGuid().ToString());
            }

            _dbContext.Trees.Remove(tree);
            _dbContext.SaveChanges();

            return NoContent();
        }
    }
}
