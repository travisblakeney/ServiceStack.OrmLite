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

        public ModelConfigExpression<T> HasMany(Expression<Func<T, object>> expression)
        {
            // create and add an EnumerablePropertyConfigExpression

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

        public Type Type { get; private set; }
        public PropertyInfo Property { get; set; }
        public MemberExpression Expression { get; private set; }
    }

    public class EnumerablePropertyConfigExpression<T> : ModelPropertyConfigExpression
    {
        public EnumerablePropertyConfigExpression(Expression<Func<T, IEnumerable>> expression) : base(typeof(T), expression as MemberExpression)
        {
            
        }
    }
}