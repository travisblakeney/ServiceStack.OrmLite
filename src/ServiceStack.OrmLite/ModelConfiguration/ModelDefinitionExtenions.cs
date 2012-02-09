using System;

namespace ServiceStack.OrmLite.ModelConfiguration
{
    public static class ModelDefinitionExtenions
    {
        public static ModelDefinition GetModelDefinition(this Type modelType, IModelConfigExpression config)
        {
            var definition = modelType.GetModelDefinition();
            definition.ParseModelConfig(config);
            return definition;
        }
    }
}