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
                Type joinTable = DiscoverJoinTableType(exp);

                if (IsEnumarablePropertyExpression(propertyExpression, typeof(TModel), joinTable))
                {
                    var tableDef = joinTable.GetModelDefinition(null);

                    if (tableDef != null && tableDef.ConfigExpression != null)
                    {
                        var matchedExpression = FindMatchingModelProperty(tableDef, typeof(TModel));
                        if (matchedExpression != null)
                        {
                            if (IsEnumarablePropertyExpression(matchedExpression, joinTable, typeof(TModel)))
                            {
                                // we have a many-to-many
                                var modelDef = typeof (TModel).GetModelDefinition(null);
                                string joinTableName = modelDef.Name + tableDef.Name;
                                string leftId = modelDef.Name + "Id";
                                string rightId = tableDef.Name + "Id";
                                
                                // JOIN UserRole on User.UserId = UserRole.UserId JOIN Role ON UserRole.RoleId = Role.RoleId
                                _joinStatement = string.Format(" JOIN {0} ON {1}.{2} = {0}.{2} JOIN {3} ON {0}.{4} = {3}.{4}", joinTableName, modelDef.Name, leftId, tableDef.Name, rightId);
                            }
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

        private bool IsEnumarablePropertyExpression(ModelPropertyConfigExpression propertyExpression, Type modelType, Type joinTable)
        {
            var expressionType = propertyExpression.GetType();
            Type enumType = typeof(EnumerablePropertyConfigExpression<,>).MakeGenericType(modelType, joinTable);
            return expressionType.IsGenericType && expressionType == enumType;
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