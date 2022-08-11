using GN.Blazor.SharedIndexedDB.Models.Messages;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GN.Blazor.SharedIndexedDB.Services.LinqQuery.Vistitors
{
    internal class FilterEvaluator : ExpressionVisitor
    {
        // https://stackoverflow.com/questions/44127341/how-do-expression-trees-allow-consumers-to-evaluate-variables
        public Filter Filter { get; private set; }
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return base.VisitParameter(node);
        }
        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            return base.VisitRuntimeVariables(node);
        }
        internal Tuple<Filter, Filter> GetPropValue(BinaryExpression node, bool Throw = true)
        {
            if (node.Left is MemberExpression member && node.Right is ConstantExpression val)
            {
                return new Tuple<Filter, Filter>(Filter.Prop(member.Member.Name.ToCamel()), Filter.Val(val.Value));
            }
            else if (node.Left is MemberExpression _member && node.Right is MemberExpression _val && _val.Member is FieldInfo field && _val.Expression is ConstantExpression c)
            {
                return new Tuple<Filter, Filter>(Filter.Prop(_member.Member.Name.ToCamel()), Filter.Val(field.GetValue(c.Value)));
            }
            if (Throw)
            {
                throw new Exception($"Inavlid or Complex Expression. {node.ToString()}.");
            }
            return null;
        }
        protected Expression VisitBinary_dep(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.And || node.NodeType == ExpressionType.AndAlso)
            {
                var l = node.Left;
                var r = node.Right;
                var left_eval = new FilterEvaluator();
                var right_eval = new FilterEvaluator();
                left_eval.Evaluate(l);
                right_eval.Evaluate(r);
                this.Filter = Filter.And(left_eval.Filter, right_eval.Filter);
                //return Expression.Empty();
            }
            switch (node.NodeType)
            {
                case ExpressionType.GreaterThan:
                    break;
            }
            if (node.NodeType == ExpressionType.GreaterThan)
            {
                if (this.Filter == null)
                {
                    var propValue = this.GetPropValue(node);
                    this.Filter = Filter.GT(propValue.Item1, propValue.Item2);
                }
            }
            if (node.NodeType == ExpressionType.Equal)
            {
                if (node.Right is MemberExpression kkk)
                {
                    var fffff = kkk.ToString();
                    var fff = kkk.Member;
                    var exp = kkk.Expression;
                }
                if (node.Left is MemberExpression member && node.Right is ConstantExpression val)
                {
                    if (this.Filter == null)
                    {
                        this.Filter = Filter.Eq(Filter.Prop(member.Member.Name.ToCamel()), Filter.Val(val.Value));
                    }
                }
                else if (node.Left is MemberExpression _member && node.Right is MemberExpression _val && _val.Member is FieldInfo field && _val.Expression is ConstantExpression c)
                {
                    var _w = field.GetValue(c.Value);
                    if (this.Filter == null)
                    {
                        this.Filter = Filter.Eq(Filter.Prop(_member.Member.Name.ToCamel()), Filter.Val(_w));
                    }

                    //var p = _val.Member;
                    //var exp = _val.Expression as ConstantExpression;
                    //if (exp != null)
                    //{
                    //    var __val = exp.Value;
                    //    if (p.MemberType == System.Reflection.MemberTypes.Field)
                    //    {
                    //        var __f = (System.Reflection.FieldInfo)p;
                    //        try
                    //        {
                    //            var ____v = __f.GetValue(__val);
                    //            var ggg = 1;
                    //        }
                    //        catch (Exception err)
                    //        {

                    //        }



                    //    }

                    //}
                }
                //else if (node.Left is MemberExpression __member && node.Right is MemberExpression __val && __val.Member is PropertyInfo _prop) 
                //{
                //    var __ff = __val.Expression as ConstantExpression;

                //}
                else
                {
                    throw new Exception($"Invalid Expression {node.ToString()}");
                }
            }
            return base.VisitBinary(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.And || node.NodeType == ExpressionType.AndAlso)
            {
            }
            switch (node.NodeType)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.And:
                    this.Filter = Filter.And(
                            new FilterEvaluator().Evaluate(node.Left),
                            new FilterEvaluator().Evaluate(node.Right));
                    break;
                case ExpressionType.OrElse:
                case ExpressionType.Or:
                    this.Filter = Filter.Or(
                            new FilterEvaluator().Evaluate(node.Left),
                            new FilterEvaluator().Evaluate(node.Right));
                    break;
                default:
                   // throw new Exception("Invalid Where Clause");

                    break;
            }
            /// If filter is not null, we have already handled the node. 
            /// 
            /// Actually we should have stopped further visiting
            /// when above statements has set the filer, for instance 
            /// in case of 'and'. But unfortunately I couldnot figure
            /// it out. 
            /// So we simply check the filter here.

            if (this.Filter == null )
            {
                Tuple<Filter, Filter> propVal = null;
                switch (node.NodeType)
                {
                    case ExpressionType.GreaterThan:
                        propVal = this.GetPropValue(node);
                        this.Filter = Filter.GT(propVal.Item1, propVal.Item2);
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        propVal = this.GetPropValue(node);
                        this.Filter = Filter.GTE(propVal.Item1, propVal.Item2);
                        break;
                    case ExpressionType.Equal:
                        propVal = this.GetPropValue(node);
                        this.Filter = Filter.Eq(propVal.Item1, propVal.Item2);
                        break;
                    case ExpressionType.LessThan:
                        propVal = this.GetPropValue(node);
                        this.Filter = Filter.LT(propVal.Item1, propVal.Item2);
                        break;
                    case ExpressionType.LessThanOrEqual:
                        propVal = this.GetPropValue(node);
                        this.Filter = Filter.LTE(propVal.Item1, propVal.Item2);
                        break;

                    default:
                        throw new Exception("Invalid Where Clause");
                        break;
                }
            }
            return base.VisitBinary(node);
        }

        public Filter Evaluate(Expression expression)
        {
            Visit(expression);
            return this.Filter;
        }
    }

}
