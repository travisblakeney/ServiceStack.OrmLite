using System;

namespace ServiceStack.OrmLite.ModelConfiguration
{
    public static class ModelDefinitionExtensions
    {
         public static ModelDefinition GetModelDefinition(this Type modelType, IModelConfigExpression config)
         {
             return OrmLiteConfigExtensions.GetModelDefinition(modelType, config);
         }
    }
}