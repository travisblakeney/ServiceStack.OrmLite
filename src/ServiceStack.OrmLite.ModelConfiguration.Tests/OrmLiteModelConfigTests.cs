using System;
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

            var command = new FakeDbCommand();
            command.Select<User>(x => x.Include(u => u.Roles));

            Assert.Equals(command.CommandText, "SELECT * FROM User JOIN Roles ON User.RoleId = Role.RoleId");
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
            throw new NotImplementedException();
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