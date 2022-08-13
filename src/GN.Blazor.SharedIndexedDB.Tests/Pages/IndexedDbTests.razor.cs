using GN.Blazor.SharedIndexedDB.Models;
using GN.Blazor.SharedIndexedDB.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GN.Blazor.SharedIndexedDB;
using Microsoft.AspNetCore.Components;
using GN.Blazor.SharedIndexedDB.Services;
using System.Diagnostics;
using GN.Blazor.SharedIndexedDB.Tests.Models;

namespace GN.Blazor.SharedIndexedDB.Tests.Pages
{
    public partial class IndexedDbTests
    {
        [Inject]
        public IIndexedDbFactory DbFactory { get; set; }


        public async Task CreateStore1()
        {

            var dbName = "test_db_" + new Random().Next(1, 100);
            //var db = await DbFactory.CreateDatabase(dbName);
            //await db.EnsureDatabase();

            var storeName = "test_store_" + new Random().Next(1, 100).ToString();
            var storeName2 = "test_store_" + new Random().Next(1, 100).ToString();
            var schema = new StoreSchema
            {
                StoreName = storeName,
                PrimaryKey = new IndexData("id")
            };

            var schema2 = new StoreSchema
            {
                StoreName = storeName2,
                PrimaryKey = new IndexData("id")
            };

            // We can create the store with the specified schema

            var response = await this.Bus.CreateContext(new CreateStoreMessage(dbName, schema))
                .Request()
                .TimeoutAfter(5000);
            //var store1 = await db.GetStore(schema).TimeoutAfter(5000);
            Log($"Store 1 Successfully Created. {response}");




            //var store2 = await db.GetStore(schema2);
            var response2 = await this.Bus.CreateContext(new CreateStoreMessage(dbName, schema2))
                .Request()
                .TimeoutAfter(5000);

            Log($"Store2 Successfully Created. {response2}");

            /// We may now get the database schema
            /// and make sure both stores exist
            /// 
            var schema_respones = await this.Bus.CreateContext(new GetDatabaseSchema(dbName))
                .Request()
                .TimeoutAfter(5000);
            var schema_retrieved = schema_respones.GetPayload<DatabaseSchema>();
            Assert(schema_retrieved.Stores.Length == 2, "We should have 2 stores");
            Log($"Schema successfully retrieved. Stores Count:{schema_retrieved.Stores.Length} ");
            Log($"Test Successfully Completed.\r\n=======================");
        }

        public async Task CreateStore()
        {

            var dbName = "test_db_" + new Random().Next(1, 100);
            var db = await DbFactory.GetDatabase(dbName);
            //await db.EnsureDatabase();

            var storeName = "test_store_" + new Random().Next(1, 100).ToString();
            var storeName2 = "test_store_" + new Random().Next(1, 100).ToString();
            var schema = new StoreSchema
            {
                StoreName = storeName,
                PrimaryKey = new IndexData("id")
            };

            var schema2 = new StoreSchema
            {
                StoreName = storeName2,
                PrimaryKey = new IndexData("id")
            };

            var store1 = await db.GetStore<object>(schema).TimeoutAfter(5000);
            Log($"Store 1 Successfully Created. {store1.Name} ");

            var store2 = await db.GetStore(schema2);

            Log($"Store2 Successfully Created. {store2.Name}");

            /// We may now get the database schema
            /// and make sure both stores exist
            /// 
            await Task.Delay(2000);

            var schema_retrieved = await db.GetSchema(true);// '//' schema_respones.GetPayload<DatabaseSchema>();
            Assert(schema_retrieved.Stores.Length == 2, "We should have 2 stores");
            Log($"Schema successfully retrieved. Stores Count:{schema_retrieved.Stores.Length} ");
            Log($"Deleting Database");
            await DbFactory.DeleteDatabase(dbName);
            Log($"Database Successfully Deleted.");

            ///// Trying to delete database
            ///// 
            //var delete_req = await this.Bus.CreateContext(new DeleteDatabaseMessage(dbName))
            //    .Request()
            //    .TimeoutAfter(200000);


            //Log($"Database successfully deleted. {delete_req}");

            Log($"Test Successfully Completed.\r\n=======================");
        }

        public async Task DeleteDatabase()
        {
            var dbName = "test" + new Random().Next(1, 1000);
            var exists = await DbFactory.DatabaseExists(dbName);
            Assert(!exists, "DatabaseExits should return false.");
            var db = await DbFactory.GetDatabase(dbName);
            await db.GetStore(new StoreSchema { StoreName = "test", PrimaryKey = new IndexData("id", "id") });
            Log($"Database Creared. {dbName}");
            await Task.Delay(500);
            exists = await DbFactory.DatabaseExists(dbName);
            Assert(exists, "DatabaseExists should return true.");
            var schema = await db.GetSchema();
            Log($"Database schema is retrieved. DbName: {schema.DbName}.");
            await DbFactory.DeleteDatabase(dbName);
            exists = await DbFactory.DatabaseExists(dbName);
            Log($"Database Deleted");
            Assert(!exists, "DatabaseExists should return false.");
            Log($"Test Successfully Completed.\r\n=============================");





        }

        public async Task Put()
        {
            var dbName = "db_" + new Random().Next(1, 10000);
            var schema = new StoreSchema { StoreName = "contacts", PrimaryKey = new IndexData("id") };
            var db = await DbFactory.GetDatabase(dbName);
            var store = await db.GetStore<PersonModel>();
            var count = 1000;
            var items = Enumerable.Range(0, count)
                .Select(x => new PersonModel
                {
                    Id = x.ToString(),
                    Name = $"Name {Guid.NewGuid()}",
                    Age = new Random().Next(10, 80)

                })
                .ToArray();
            var dt = Stopwatch.StartNew();

            await store.Put(items);
            Log($" {count} items inserted in {dt.ElapsedMilliseconds} milliseconds.");


            var _schema = await db.GetSchema(true);
            var get_count_result = await store.Count();
            Assert(get_count_result == count, $"Count should be {count} it is {get_count_result}");
            var id12 = items[0].Id;
            var age = items[0].Age;
            //var lst = await store.GetQueryable().Where(x => x.Id == id12 && x.Age == age).FirstOrDefaultAsync();
            //var cnt = lst.Length;

            dt = Stopwatch.StartNew();
            var items_fetched = await store.FetchAll();

            Log($" {items_fetched.Count()} Items fetched in {dt.ElapsedMilliseconds} milliseconds");

            dt = Stopwatch.StartNew();

            items_fetched = await store.FetchAll(q => q.Skip(5).Take(10));


            Log($" {items_fetched.Count()} Items fetched in {dt.ElapsedMilliseconds} milliseconds. {items_fetched.ToArray()[0].Id}");

            items_fetched = await store.FetchAll(q =>
            {
                q.Filter(Filter.Eq(Filter.Prop("age"), Filter.Val(21)));

            });
            items_fetched.ToList().ForEach(x => Assert(x.Age == 21, "should be 21"));

            items_fetched = await store.FetchAll(q =>
            {
                q.Filter(Filter.LT(Filter.Prop("age"), Filter.Val(21)));

            });
            items_fetched.ToList().ForEach(x => Assert(x.Age < 21, "should < 21"));

            items_fetched = await store.FetchAll(q =>
            {
                q.Filter(Filter.GT(Filter.Prop("age"), Filter.Val(21)));

            });
            items_fetched.ToList().ForEach(x => Assert(x.Age > 21, "should > 21"));

            items_fetched = await store.FetchAll(q =>
            {
                q.Filter(Filter.Contains(Filter.Prop("id"), Filter.Val("9")));

            });



            items_fetched.ToList().ForEach(x => Assert(x.Id.Contains("9"), "should contain 9"));

            items_fetched = await store.FetchAll(q =>
            {
                q.Filter(Filter.Like(Filter.Prop("id"), Filter.Val("1*9")));

            });
            items_fetched.ToList().ForEach(x => Assert(x.Id.StartsWith("1") && x.Id.EndsWith("9"), "should be 1*9"));
            //Assert(expected1.Count() == items_fetched.Count(), $"should be equal {expected1.Count()}, {items_fetched.Count()}");



            items_fetched = await store.FetchAll(q =>
            {
                q.Filter(Filter.Or(Filter.Eq(Filter.Prop("age"), Filter.Val(21)), Filter.Eq(Filter.Prop("age"), Filter.Val(41))));

            });

            var expected = items.Where(x => x.Age == 21 || x.Age == 41);
            Assert(expected.Count() == items_fetched.Count(), $"Should be equal, {expected.Count()}!={items_fetched.Count()}");

            items_fetched.ToList().ForEach(x => Assert(x.Age == 21 || x.Age == 41, "should be 21"));

            items_fetched = await store.FetchAll(q =>
            {
                q.Filter(Filter.And(Filter.Eq(Filter.Prop("age"), Filter.Val(21)), Filter.Eq(Filter.Prop("age"), Filter.Val(41))));

            });
            //var lst = await store.GetQueryable().ToListAsync();

            Assert(items_fetched.Count() == 0, "Should be zero");

            dt = Stopwatch.StartNew();
            /// Order by
            /// 
            items_fetched = await store.FetchAll(q => q.OrderBy("Age"));
            var items_ordered = items.OrderBy(x => x.Age).ToArray();
            Log($"{items_fetched.Count()} items fetched with order by age in {dt.ElapsedMilliseconds} milliseconds");
            Assert(items_fetched.Where((x, i) => items_ordered[i].Age != x.Age).FirstOrDefault() == null, "Order mismatch");

            /// Select Keys
            /// 
            dt = Stopwatch.StartNew();
            items_fetched = await store.FetchAll(q => q.SelectKeys());
            Log($"{items_fetched.Count()} items fetched with Select Keys in {dt.ElapsedMilliseconds} milliseconds");
            var fff = items_fetched.ToArray()[0];










            await Task.Delay(1000);

            await DbFactory.DeleteDatabase(dbName).TimeoutAfter(2000, default, false);

            Log($"Test Successfully Completed.\r\n===================");
        }

        public async Task FindByID()
        {
            var dbName = "parias_test_" + new Random().Next(1, 100);
            var schema = new StoreSchema
            {
                StoreName = dbName,
                PrimaryKey = new IndexData("guid")
            };

            var store = await
                (await DbFactory.GetDatabase(dbName))
                .GetStore<TestModel>(schema);

            await store.Put(TestModel.GetSampleData());

            var res = await store.GetByID("2");


        }
        public async Task DeleteById()
        {
            var context = new DeleteRecordByID("id");
            var res = await this.Bus.CreateContext(context).Request();


        }



        public async Task Linq()
        {
            var dbName = "lnq_tests_" + new Random().Next(1, 1000).ToString();
            var db = await DbFactory.GetDatabase(dbName);
            var store = await db.GetStore<Account>();
            var seed = Account.Seed(5000);
            var dt = Stopwatch.StartNew();

            Log($"Store Successfully Created. Database:{dbName}, Store {store.Name}\r\n Inserting seeds. Count:{seed.Length}");
            await store.Put(seed);
            Log($"'{seed.Length}' inserted in {dt.ElapsedMilliseconds} milliseconds.");


            var queryable = store.GetQueryable();
            //var ___items = await queryable.Where(x => x.Id < 200).OrderBy(x => x.Name).ToArrayAsync();


            var id = seed[10].Id;
            var items = await queryable.Where(x => x.Id == id).ToArrayAsync();
            Log($"There is {items.Length} item where Id= {id}.");

            var account = seed[5];
            id = account.Id;
            var name = account.Name;
            var item = await queryable.Where(x => x.Id == id && x.Name == name).FirstOrDefaultAsync();

            Log($"Item with {item.Name}=={account.Name} ");
            dt = Stopwatch.StartNew();

            items = await queryable.Where(x => x.Id > 2000).ToArrayAsync();
            Log($"{items.Count()} items fetched with id>2000 in {dt.ElapsedMilliseconds} milliseconds.");

            dt = Stopwatch.StartNew();
            items = await queryable.Where(x => x.Id == 12 || x.Id == 120).ToArrayAsync();
            Log($"{items.Count()} items fetched with id==12 || id==120 {dt.ElapsedMilliseconds} milliseconds.");

            /// SKip and Take
            /// 
            var expected = seed.Where(x => x.Id > 100).Skip(10).Take(5).ToArray();
            dt = Stopwatch.StartNew();
            items = await store.GetQueryable().Where(x => x.Id > 100).Skip(10).Take(5).ToArrayAsync();
            Assert(items.Length == expected.Length, "Should be equal.");
            Assert(items[0].Id == expected[0].Id, "Should be equal.");
            Log($"{items.Length} Items fetched with skip(10), take(5) in {dt.ElapsedMilliseconds} milliseconds.");

            dt = Stopwatch.StartNew();
            items = await queryable.Where(x => x.Id < 200).OrderBy(x => x.Name).ToArrayAsync();
            var actual = seed.Where(x => x.Id < 200).OrderBy(x => x.Name).ToArray();
            for (var i = 0; i < items.Length; i++)
            {
                var a = actual[i];
                var _i = items[i];
                Assert(items[i].Id == actual[i].Id && items[i].Name == actual[i].Name, "Should be equal");
            }



            await Task.Delay(1000);
            Log($"Deleting Database...");
            await DbFactory.DeleteDatabase(dbName);
            Log($"Test Successfully Completed.\r\n================");
        }

        public async Task StoreEvents()
        {
            await Bus.Subscribe("put", m =>
            {
            }, true);

            var dbName = "hhh";
            var db = await DbFactory.GetDatabase(dbName);
            var store = await db.GetStore<Account>();
            await store.Put(new Account[]
            {
                new Account
                {
                    Id = 1,
                    Name = "lll"
                }
            });
            await Task.Delay(1000);
        }
    }
}
