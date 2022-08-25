using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.IndexedDB.LinqQuery.Vistitors
{

    internal class ConstantsEvaluator : ExpressionVisitor
    {
        // https://stackoverflow.com/questions/44127341/how-do-expression-trees-allow-consumers-to-evaluate-variables
        public Filter Filter { get; private set; }
        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value != null)
            {
                var ttt = node.Value.GetType();
                

            }
            return base.VisitConstant(node);
        }
        
        public void Evaluate(Expression expression)
        {
            Visit(expression);
        }
    }

}
