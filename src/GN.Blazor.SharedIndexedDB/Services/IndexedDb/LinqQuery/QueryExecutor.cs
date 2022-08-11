using GN.Blazor.SharedIndexedDB.Models.Messages;
using GN.Blazor.SharedIndexedDB.Services.LinqQuery.Vistitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Services.LinqQuery
{
    interface IQueryExecutor
    {
        Task<object> Execute(Expression expression, bool IsEnumerable);
    }
    class QueryExecutor<T> : IQueryExecutor where T : class
    {
        private readonly IndexedDbStore<T> store;

        public QueryExecutor(IndexedDbStore<T> store)
        {
            this.store = store;
        }
        private static bool IsQueryOverDataSource(Expression expression)
        {
            // If expression represents an unqueried IQueryable data source instance, 
            // expression is of type ConstantExpression, not MethodCallExpression. 
            return (expression is MethodCallExpression);
        }

        public async Task<object> Execute(Expression expression, bool IsEnumerable)
        {
            // The expression must represent a query over the data source. 
            if (!IsQueryOverDataSource(expression))
                throw new InvalidProgramException("No query over the data source was specified.");

            // Find the call to Where() and get the lambda expression predicate.
            var skip = new SkipFinder().FindValue(expression);
            var take = new TakeFinder().FindValue(expression);
            var orderBy = new OrderByFinder().FindValue(expression);
            var orderByDescending = new OrderByDescendingFinder().FindValue(expression);
            InnermostWhereFinder whereFinder = new InnermostWhereFinder();
            MethodCallExpression whereExpression = whereFinder.GetInnermostWhere(expression);

            var filterEvaluator = new FilterEvaluator();
            if (whereExpression != null)
            {
                filterEvaluator.Evaluate(whereExpression);

                //if (IsEnumerable)
                //    return  queryablePlaces.Provider.CreateQuery(newExpressionTree);
                //else
                //    return  queryablePlaces.Provider.Execute(newExpressionTree);
            }
            var res = await this.store.FetchAll(q =>
            {
                if (take.HasValue)
                {
                    q.Take(take.Value);
                }
                if (skip.HasValue)
                {
                    q.Skip(skip.Value);
                }
                if (filterEvaluator?.Filter != null)
                {
                    q.Filter(filterEvaluator.Filter);
                }
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    q.OrderBy(orderBy);
                }
                if (!string.IsNullOrWhiteSpace(orderByDescending))
                {
                    q.OrderBy(orderByDescending,true);
                }

            });

            return res.ToArray();


        }
    }
    class TerraServerQueryContext
    {
        // Executes the expression tree that is passed to it. 
        internal static object Execute(Expression expression, bool IsEnumerable)
        {
            // The expression must represent a query over the data source. 
            if (!IsQueryOverDataSource(expression))
                throw new InvalidProgramException("No query over the data source was specified.");

            // Find the call to Where() and get the lambda expression predicate.
            InnermostWhereFinder whereFinder = new InnermostWhereFinder();
            MethodCallExpression whereExpression = whereFinder.GetInnermostWhere(expression);
            var ff = new FilterEvaluator();
            //var q = new StoreFetchPayload();
            ff.Evaluate(whereExpression);

            LambdaExpression lambdaExpression = (LambdaExpression)((UnaryExpression)(whereExpression.Arguments[1])).Operand;

            //// Send the lambda expression through the partial evaluator.
            //lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

            //// Get the place name(s) to query the Web service with.
            //LocationFinder lf = new LocationFinder(lambdaExpression.Body);
            //List<string> locations = lf.Locations;
            //if (locations.Count == 0)
            //    throw new InvalidQueryException("You must specify at least one place name in your query.");

            //// Call the Web service and get the results.
            //Place[] places = WebServiceHelper.GetPlacesFromTerraServer(locations);

            //// Copy the IEnumerable places to an IQueryable.
            //IQueryable<Place> queryablePlaces = places.AsQueryable<Place>();

            // Copy the expression tree that was passed in, changing only the first 
            // argument of the innermost MethodCallExpression.
            //ExpressionTreeModifier treeCopier = new ExpressionTreeModifier(queryablePlaces);
            //Expression newExpressionTree = treeCopier.Visit(expression);

            // This step creates an IQueryable that executes by replacing Queryable methods with Enumerable methods. 
            //if (IsEnumerable)
            //    return queryablePlaces.Provider.CreateQuery(newExpressionTree);
            //else
            //    return queryablePlaces.Provider.Execute(newExpressionTree);
            return new string[] { };
        }

        private static bool IsQueryOverDataSource(Expression expression)
        {
            // If expression represents an unqueried IQueryable data source instance, 
            // expression is of type ConstantExpression, not MethodCallExpression. 
            return (expression is MethodCallExpression);
        }
    }
}
