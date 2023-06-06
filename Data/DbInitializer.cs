using TreeNodes.Models;

namespace TreeNodes.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Trees.Any())
            {
                return;
            }

            var tree1 = new Tree { Name = "Tree 1" };
            var tree2 = new Tree { Name = "Tree 2" };

            var node1 = new Node { Name = "Node 1", Tree = tree1 };
            var node2 = new Node { Name = "Node 2", Tree = tree1 };
            var node3 = new Node { Name = "Node 3", Tree = tree2 };

            context.Trees.AddRange(tree1, tree2);
            context.Nodes.AddRange(node1, node2, node3);

            context.SaveChanges();
        }
    }
}
