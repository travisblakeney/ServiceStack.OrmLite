namespace ServiceStack.OrmLite.ModelConfiguration
{
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
}