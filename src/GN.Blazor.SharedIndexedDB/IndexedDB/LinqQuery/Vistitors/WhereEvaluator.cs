using System.Linq.Expressions;

namespace GN.Blazor.SharedIndexedDB.IndexedDB.LinqQuery.Vistitors
{
    internal class WhereEvaluator : ExpressionVisitor
    {
        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.AndAlso)
            {

            }
            return base.VisitBinary(node);
        }
        public void GetInnermostWhere(Expression expression)
        {
            Visit(expression);

        }
    }

}
