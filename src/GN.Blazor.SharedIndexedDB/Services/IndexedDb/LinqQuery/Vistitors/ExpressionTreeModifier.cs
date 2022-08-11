using System.Linq;
using System.Linq.Expressions;

namespace GN.Blazor.SharedIndexedDB.Services.LinqQuery.Vistitors
{
    //internal class ExpressionTreeModifier : ExpressionVisitor
    //{
    //    private IQueryable<Place> queryablePlaces;

    //    internal ExpressionTreeModifier(IQueryable<Place> places)
    //    {
    //        queryablePlaces = places;
    //    }

    //    protected override Expression VisitConstant(ConstantExpression c)
    //    {
    //        // Replace the constant QueryableTerraServerData arg with the queryable Place collection. 
    //        if (c.Type == typeof(QueryableTerraServerData<Place>))
    //            return Expression.Constant(queryablePlaces);
    //        else
    //            return c;
    //    }
    //}

}
