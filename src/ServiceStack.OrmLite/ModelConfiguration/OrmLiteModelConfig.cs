using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ServiceStack.OrmLite.ModelConfiguration
{
    public abstract class OrmLiteModelConfig
    {
        public abstract void Load(ModelConfigContext context);
    }

    public class ModelConfigContext
    {
        public Dictionary<Type, IModelConfigExpression> ConfigExpressions { get; private set; }

        public ModelConfigContext()
        {
            ConfigExpressions = new Dictionary<Type,IModelConfigExpression>();
        }

        public ModelConfigExpression<T> Model<T>()
        {
            var exp = new ModelConfigExpression<T>();

            // last one wins?
            if (ConfigExpressions.ContainsKey(typeof(T)))
            {
                ConfigExpressions[typeof (T)] = exp;
            }
            else
            {
                ConfigExpressions.Add(typeof(T), exp);
            }

            return exp;
        }
    }

    public interface IModelConfigExpression
    {
        List<ModelPropertyConfigExpression> PropertyExpressions { get; }
    }

    public class ModelConfigExpression<T> : IModelConfigExpression
    {
        public List<ModelPropertyConfigExpression> PropertyExpressions { get; private set; }

        public ModelConfigExpression()
        {
            PropertyExpressions = new List<ModelPropertyConfigExpression>();
        }

        public ModelConfigExpression<T> HasMany<TProperty>(Expression<Func<T, IEnumerable<TProperty>>> expression)
        {
            // add an EnumerablePropertyConfigExpression
            PropertyExpressions.Add(new EnumerablePropertyConfigExpression<T, TProperty>(expression));
            return this;
        }
    }

    public class ModelPropertyConfigExpression
    {
        public ModelPropertyConfigExpression(Type type, MemberExpression expression)
        {
            Type = type;
            Expression = expression;
        }

        public Type Type { get; protected set; }
        public PropertyInfo Property { get; set; }
        public MemberExpression Expression { get; protected set; }
    }

    public class EnumerablePropertyConfigExpression<T,TProperty> : ModelPropertyConfigExpression
    {
        public EnumerablePropertyConfigExpression(Expression<Func<T, IEnumerable<TProperty>>> expression) : base(typeof(TProperty), expression.Body as MemberExpression)
        {
            // cast the expression.Body to a MemberExpression to get the property info object
            Property = ((MemberExpression) expression.Body).Member as PropertyInfo;
        }
    }
}