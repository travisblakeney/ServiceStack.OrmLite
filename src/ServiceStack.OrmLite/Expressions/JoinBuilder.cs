using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ServiceStack.OrmLite.ModelConfiguration;

namespace ServiceStack.OrmLite
{
    public class JoinBuilder
    {
        private readonly ModelDefinition _modelDefinition;
        private string _joinStatement = string.Empty;

        public JoinBuilder(ModelDefinition modelDefinition)
        {
            _modelDefinition = modelDefinition;
        }

        public string BuildJoinStatement<TModel,TProperty>(Expression<Func<TModel,TProperty>> exp)
        {
            // do we have a property expression in the model definition that matches this MemberAccessExpression?
            var propertyExpression = FindMatchingConfigExpression(exp);

            if (propertyExpression != null)
            {
                // get the propertyname
                var member = exp.Body as MemberExpression;

                // get the tablename
                Type tableType = DiscoverJoinTableType(exp);
                string tableName = tableType.Name;

                var expressionType = propertyExpression.GetType();

                Type enumType = typeof(EnumerablePropertyConfigExpression<,>).MakeGenericType(typeof(TModel), tableType);

                // account for a many to many
                if (expressionType.IsGenericType && expressionType == enumType)
                {
                    // get the underlying T of the IEnumerable<T>
                    var modelType = propertyExpression.Property.PropertyType
                        .GetGenericArguments().FirstOrDefault();

                    var endDef = modelType.GetModelDefinition(null);

                    if (endDef != null && endDef.ConfigExpression != null)
                    {
                        var matchedExpression = FindMatchingModelProperty(endDef, modelType);
                        if (matchedExpression != null)
                        {
                            
                        }
                    }
                }
            }

            return _joinStatement;
        }

        private ModelPropertyConfigExpression FindMatchingConfigExpression<TModel,TProperty>(Expression<Func<TModel, TProperty>> exp)
        {
            if (_modelDefinition.ConfigExpression != null)
            {
                return _modelDefinition.ConfigExpression.PropertyExpressions
                    .FirstOrDefault(x => x.Property == ((MemberExpression)exp.Body).Member);
            }

            return null;
        }

        private ModelPropertyConfigExpression FindMatchingModelProperty(ModelDefinition modelDef, Type leftSideType)
        {
            return modelDef.ConfigExpression.PropertyExpressions.FirstOrDefault(x => x.Type == leftSideType);
        }

        private Type DiscoverJoinTableType<TModel,TProperty>(Expression<Func<TModel, TProperty>> exp)
        {
            Type tableType = null;

            if (exp.Body.Type.GetInterfaces().Any(x => x.Equals(typeof(IEnumerable))))
            {
                // is it generic?
                if (exp.Body.Type.IsGenericType)
                {
                    var typeDefinition = exp.Body.Type.GetGenericTypeDefinition();
                    if (typeDefinition.GetInterfaces().Any(x => x.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                    {
                        // get the generic type argument
                        tableType = exp.Body.Type.GetGenericArguments().First();
                    }
                }
            }
            else
            {
                tableType = exp.Body.Type;
            }

            return tableType;
        }
    }
}