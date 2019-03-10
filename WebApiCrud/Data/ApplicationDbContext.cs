using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using WebApiPaginatedCrud.Entities;
using Database = System.Data.Entity.Database;

namespace WebApiPaginatedCrud.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("name=SqlServer")
        {
            Database.SetInitializer(new TodosInitializer());
            //Database.Initialize(true);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbSet<Todo> Todos { get; set; }

        public class TodosInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
        {
            protected override void Seed(ApplicationDbContext context)
            {
                int todosCount = context.Todos.Count();
                int todosToSeed = 32;
                todosToSeed -= todosCount;
                if (todosToSeed > 0)
                {
                    Console.WriteLine($"[+] Seeding ${todosToSeed} Todos");
                    var faker = new Faker<Todo>()
                        .RuleFor(a => a.Title, f => String.Join(" ", f.Lorem.Words(f.Random.Int(2, 5))))
                        .RuleFor(a => a.Description, f => f.Lorem.Sentences(f.Random.Int(min: 1, max: 10)))
                        .RuleFor(t => t.Completed, f => f.Random.Bool(0.4f))
                        .RuleFor(a => a.CreatedAt,
                            f => f.Date.Between(DateTime.Now.AddYears(-5), DateTime.Now.AddDays(-1)))
                        .FinishWith(async (f, todoInstance) =>
                        {
                            todoInstance.UpdatedAt =
                                f.Date.Between(start: todoInstance.CreatedAt.Value, end: DateTime.Now);
                        });

                    List<Todo> todos = faker.Generate(todosToSeed);
                    context.Todos.AddRange(todos);
                    context.SaveChanges();
                }

                base.Seed(context);
            }
        }

        public override int SaveChanges()
        {
            HandleTimeStamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            HandleTimeStamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void HandleTimeStamps()
        {
            var now = DateTime.Now;
            foreach (var dbEntityEntry in ChangeTracker.Entries())
            {
                if (dbEntityEntry.Entity is ITimestampedEntity entity)
                {
                    switch (dbEntityEntry.State)
                    {
                        case EntityState.Added:
                            if (entity.CreatedAt == null)
                                entity.CreatedAt = now;
                            if (entity.UpdatedAt == null)
                            {
                                entity.UpdatedAt = now;
                            }

                            break;
                        case EntityState.Modified:
                            Entry(entity).Property(e => e.CreatedAt).IsModified = false;
                            entity.UpdatedAt = now;
                            break;
                    }
                }
            }
        }
    }
}