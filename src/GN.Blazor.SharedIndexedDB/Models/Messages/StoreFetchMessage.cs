using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Models.Messages
{

    public static class FilterOps
    {
        public const string Eq = "EQ";
        public const string PROP = "PROP";
        public const string VAL = "VAL";
        public const string OR = "OR";
        public const string AND = "AND";
        public const string GT = "GT";
        public const string LT = "LT";
        public const string GTE = "GTE";
        public const string LTE = "LTE";
        public const string CONTANIS = "CONTANIS";
        public const string LIKE = "LIKE";
    }
    public class Filter
    {
        public string Operator { get; set; }
        public Filter Left { get; set; }
        public Filter Right { get; set; }
        public object Value { get; set; }

        public Filter()
        {

        }
        public Filter(string op, Filter left, Filter right, object value)
        {
            this.Left = left;
            this.Right = right;
            this.Operator = op;
            this.Value = value;
        }

        public static Filter Prop(string propName)
        {
            return new Filter(FilterOps.PROP, null, null, propName);

        }
        public static Filter Val(object value)
        {
            return new Filter(FilterOps.VAL, null, null, value);

        }
        public static Filter Eq(Filter left, Filter right)
        {
            return new Filter(FilterOps.Eq, left, right, null);

        }
        public static Filter Or(Filter left, Filter right)
        {
            return new Filter(FilterOps.OR, left, right, null);

        }
        public static Filter And(Filter left, Filter right)
        {
            return new Filter(FilterOps.AND, left, right, null);

        }
        public static Filter GT(Filter left, Filter right)
        {
            return new Filter(FilterOps.GT, left, right, null);

        }
        public static Filter LT(Filter left, Filter right)
        {
            return new Filter(FilterOps.LT, left, right, null);

        }
        public static Filter LTE(Filter left, Filter right)
        {
            return new Filter(FilterOps.LTE, left, right, null);

        }
        public static Filter GTE(Filter left, Filter right)
        {
            return new Filter(FilterOps.GTE, left, right, null);

        }

        public static Filter Contains(Filter left, Filter right)
        {
            return new Filter(FilterOps.CONTANIS, left, right, null);

        }
        public static Filter Like(Filter left, Filter right)
        {
            return new Filter(FilterOps.LIKE, left, right, null);

        }


    }



    public class StoreFetchPayload : StoreSchemaWithDbName
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public Filter Filter { get; set; }
        public string OrderBy { get; set; }
        public bool? Descending { get; set; }
        public string Select { get; set; }
       
    }
    public class StoreFetchResult
    {

        public object[] Items { get; set; }


    }

    public class StoreFetchMessage : Message<StoreFetchPayload>
    {
        public StoreFetchMessage(string dbName, StoreSchema schema, Action<StoreFetchPayload> query = null) : base(Subjects.StoreFetch, new StoreFetchPayload
        {
            DbName = dbName,
            Schema = schema,
        })
        {
            query?.Invoke(this.Payload);
        }
    }
}
