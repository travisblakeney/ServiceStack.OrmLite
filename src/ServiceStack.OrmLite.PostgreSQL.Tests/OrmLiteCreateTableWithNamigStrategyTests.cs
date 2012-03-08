using System;
using NUnit.Framework;
using ServiceStack.Common.Tests.Models;


namespace ServiceStack.OrmLite.Tests
{

	[TestFixture]
	public class OrmLiteCreateTableWithNamigStrategyTests 
		: OrmLiteTestBase
	{

		[Test]
		public void Can_create_TableWithNamigStrategy_table_prefix()
		{
			OrmLite.OrmLiteConfig.DialectProvider.NamingStrategy = new PrefixNamingStrategy
			{
				 TablePrefix ="tab_",
				 ColumnPrefix = "col_",
			};
			
			using (var db = ConnectionString.OpenDbConnection())
			using (var dbCmd = db.CreateCommand())
			{
				dbCmd.CreateTable<ModelWithOnlyStringFields>(true);
			}
		}

		[Test]
		public void Can_create_TableWithNamigStrategy_table_lowered()
		{
			OrmLite.OrmLiteConfig.DialectProvider.NamingStrategy = new LowercaseNamingStrategy();

			using (var db = ConnectionString.OpenDbConnection())
			using (var dbCmd = db.CreateCommand())
			{
				dbCmd.CreateTable<ModelWithOnlyStringFields>(true);
			}
		}


		[Test]
		public void Can_create_TableWithNamigStrategy_table_nameUnderscoreCoumpound()
		{
			OrmLite.OrmLiteConfig.DialectProvider.NamingStrategy = new UnderscoreSeparatedCompoundNamingStrategy();

			using (var db = ConnectionString.OpenDbConnection())
			using (var dbCmd = db.CreateCommand())
			{
				dbCmd.CreateTable<ModelWithOnlyStringFields>(true);
			}
		}

	}

	public class PrefixNamingStrategy : OrmLiteNamingStrategyBase
	{

		public string TablePrefix { get; set; }

		public string ColumnPrefix { get; set; }

		public override string GetTableName(string name)
		{
			return TablePrefix + name;
		}

		public override string GetColumnName(string name)
		{
			return ColumnPrefix + name;
		}

	}

	public class LowercaseNamingStrategy : OrmLiteNamingStrategyBase
	{

		public override string GetTableName(string name)
		{
			return name.ToLower();
		}

		public override string GetColumnName(string name)
		{
			return name.ToLower();
		}

	}

	public class UnderscoreSeparatedCompoundNamingStrategy : OrmLiteNamingStrategyBase
	{

		public override string GetTableName(string name)
		{
			return toUnderscoreSeparatedCompound(name);
		}

		public override string GetColumnName(string name)
		{
			return toUnderscoreSeparatedCompound(name);
		}


		string toUnderscoreSeparatedCompound(string name)
		{

			string r = char.ToLower(name[0]).ToString();

			for (int i = 1; i < name.Length; i++)
			{
				char c = name[i];
				if (char.IsUpper(name[i]))
				{
					r += "_";
					r += char.ToLower(name[i]);
				}
				else
				{
					r += name[i];
				}
			}
			return r;
		}

	}



}