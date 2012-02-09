using System;
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
            OrmLiteConfig.DialectProvider = SqliteOrmLiteDialectProvider.Instance;
            ModelConfigContext context = new ModelConfigContext();
            TestModelConfig config = new TestModelConfig();
            config.Load(context);

            Assert.AreEqual(context.ConfigExpressions.Count, 1);
            var keyValue = context.ConfigExpressions.FirstOrDefault();
            Type modelType = keyValue.Key;
            var configExpression = keyValue.Value;
            ModelDefinition def = modelType.GetModelDefinition(configExpression);
            Assert.IsNotNull(def.ConfigExpression);
        }
    }
}