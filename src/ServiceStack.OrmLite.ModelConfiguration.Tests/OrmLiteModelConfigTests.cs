using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NUnit.Framework;
using ServiceStack.OrmLite.Sqlite;

namespace ServiceStack.OrmLite.ModelConfiguration.Tests
{
    [TestFixture]
    public class OrmLiteModelConfigTests
    {
        [Test]
        public void Creates_TestModelConfig_and_assigns_ModelConfigExpression_to_ModelDefinition()
        {
            ModelConfigContext context = GetModelConfigContext();

            Assert.AreEqual(context.ConfigExpressions.Count, 1);
            var keyValue = context.ConfigExpressions.FirstOrDefault();
            Type modelType = keyValue.Key;
            var configExpression = keyValue.Value;
            ModelDefinition def = modelType.GetModelDefinition(configExpression);
            Assert.IsNotNull(def.ConfigExpression);
        }
 
        [Test]
        public void JoinSelect_ReturnsObject_FromMultipleTables()
        {
            ModelConfigContext context = GetModelConfigContext();

            foreach (var keyValue in context.ConfigExpressions)
            {
                Type modelType = keyValue.Key;
                modelType.GetModelDefinition(keyValue.Value);
            }

            var command = new FakeDbCommand();
            command.Select<User>(x => x.Include(u => u.Roles));

            string expected = "SELECT \"Id\" ,\"Username\"  \nFROM \"User\" JOIN UserRole ON User.UserId = UserRole.UserId JOIN Role ON UserRole.RoleId = Role.RoleId";
            Assert.AreEqual(command.CommandText, expected);
        }

        private static ModelConfigContext GetModelConfigContext()
        {
            OrmLiteConfig.DialectProvider = SqliteOrmLiteDialectProvider.Instance;
            ModelConfigContext context = new ModelConfigContext();
            TestModelConfig config = new TestModelConfig();
            config.Load(context);

            return context;
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public List<Role> Roles { get; set; }
    }

    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public List<User> Users;
    }

    public class TestModelConfig : OrmLiteModelConfig
    {
        public override void Load(ModelConfigContext context)
        {
            context.Model<User>()
                .HasMany(u => u.Roles);

            context.Model<Role>()
                .HasMany(r => r.Users);
        }
    }

    public class FakeDbCommand : IDbCommand
    {
        public void Cancel()
        {
            
        }

        public string CommandText { get; set; }

        public int CommandTimeout { get; set; }

        public CommandType CommandType { get; set; }

        public IDbConnection Connection { get; set; }

        public IDbDataParameter CreateParameter()
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQuery()
        {
            throw new NotImplementedException();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            throw new NotImplementedException();
        }

        public IDataReader ExecuteReader()
        {
            return new DataTableReader(new DataTable("User"));
        }

        public object ExecuteScalar()
        {
            throw new NotImplementedException();
        }

        public IDataParameterCollection Parameters
        {
            get { throw new NotImplementedException(); }
        }

        public void Prepare()
        {
            
        }

        public IDbTransaction Transaction { get; set; }

        public UpdateRowSource UpdatedRowSource { get; set; }

        public void Dispose()
        {
            
        }
    }
}