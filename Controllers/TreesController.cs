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
        public async Task<IActionResult> GetTrees()
        {
            List<Tree> trees = await _dbContext.Trees.Include(t => t.Nodes).ToListAsync();

            var result = trees.Select(tree => new
            {
                Id = tree.Id,
                Name = tree.Name,
                Nodes = tree.Nodes?.Select(n => new
                {
                    Id = n.Id,
                    Name = n.Name
                })
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTree(int id)
        {
            var tree = await _dbContext.Trees.Include(t => t.Nodes).FirstOrDefaultAsync(q => q.Id == id);
            if (tree == null)
            {
                return NotFound();
            }

            var result = new
            {
                Id = tree.Id,
                Name = tree.Name,
                Nodes = tree.Nodes?.Select(n => new
                {
                    Id = n.Id,
                    Name = n.Name
                })
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTree(TreeRequest tree)
        {
            if (string.IsNullOrEmpty(tree.Name))
            {
                throw new SecureException(Guid.NewGuid().ToString(), "Tree Name cannot be null or empty");
            }

            var _tree = new Tree { Name = tree.Name };
            _dbContext.Trees.Add(_tree);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTree), new { id = _tree.Id }, _tree);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTree(int id, TreeRequest updatedTree)
        {
            var tree = await _dbContext.Trees.FindAsync(id);
            if (tree == null)
            {
                throw new SecureException(Guid.NewGuid().ToString(), "Tree is not found");
            }

            tree.Name = updatedTree.Name;
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTree(int id)
        {
            var tree = await _dbContext.Trees.FindAsync(id);
            if (tree == null)
            {
                throw new SecureException(Guid.NewGuid().ToString(), "Tree is not found");
            }

            if (await _dbContext.Nodes.AnyAsync(n => n.TreeId == id))
            {
                throw new SecureException(Guid.NewGuid().ToString(), "You have to delete all children nodes first");
            }

            _dbContext.Trees.Remove(tree);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
