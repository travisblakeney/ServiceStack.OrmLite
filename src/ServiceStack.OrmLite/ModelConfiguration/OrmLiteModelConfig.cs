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
        public ModelConfigExpression<T> Model<T>()
        {
            return new ModelConfigExpression<T>();
        }
    }

    public class ModelConfigExpression<T>
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