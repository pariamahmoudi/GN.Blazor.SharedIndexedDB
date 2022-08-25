using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GN.Blazor.SharedIndexedDB.IndexedDB;
using GN.Blazor.SharedIndexedDB.Messaging.Commands;
using GN.Blazor.SharedIndexedDB.Tests.Models;
using Microsoft.AspNetCore.Components;

namespace GN.Blazor.SharedIndexedDB.Tests.Pages
{
    public partial class IndexedDbTests
    {
        [Inject]
        public IIndexedDbFactory DbFactory { get; set; }

        public async Task CreateStore1()
        {
            var dbName = "test_db_" + new Random().Next(1, 100);
            var storeName = "test_store_" + new Random().Next(1, 100).ToString();
            var storeName2 = "test_store_" + new Random().Next(1, 100).ToString();
            var schema = new StoreSchema
            {
                StoreName = storeName,
                PrimaryKey = new IndexData("id"),
            };

            var schema2 = new StoreSchema
            {
                StoreName = storeName2,
                PrimaryKey = new IndexData("id"),
            };

            var response = await this.Bus.CreateContext(new CreateStore(dbName, schema))
                .Request()
                .TimeoutAfter(5000);

            // var store1 = await db.GetStore(schema).TimeoutAfter(5000);
            this.Log($"Store 1 Successfully Created. {response}");

            var response2 = await this.Bus.CreateContext(new CreateStore(dbName, schema2))
                .Request()
                .TimeoutAfter(5000);

            this.Log($"Store2 Successfully Created. {response2}");

            var schema_respones = await this.Bus.CreateContext(new GetDatabaseSchema(dbName))
                .Request()
                .TimeoutAfter(5000);
            var schema_retrieved = schema_respones.GetPayload<DatabaseSchema>();
            this.Assert(schema_retrieved.Stores.Length == 2, "We should have 2 stores");
            this.Log($"Schema successfully retrieved. Stores Count:{schema_retrieved.Stores.Length} ");
            this.Log($"Test Successfully Completed.\r\n=======================");
        }

        public async Task CreateStore()
        {
            var dbName = $"test_db_{new Random().Next(1, 100)}";
            var db = await this.DbFactory.GetDatabase(dbName);

            var storeName = "test_store_" + new Random().Next(1, 100).ToString();
            var storeName2 = "test_store_" + new Random().Next(1, 100).ToString();
            var schema = new StoreSchema
            {
                StoreName = storeName,
                PrimaryKey = new IndexData("id"),
            };

            var schema2 = new StoreSchema
            {
                StoreName = storeName2,
                PrimaryKey = new IndexData("id"),
            };

            var store1 = await db.GetStore<object>(schema).TimeoutAfter(5000);
            this.Log($"Store 1 Successfully Created. {store1.Name} ");

            var store2 = await db.GetStore(schema2);

            this.Log($"Store2 Successfully Created. {store2.Name}");
            await Task.Delay(2000);

            var schema_retrieved = await db.GetSchema(true);
            this.Assert(schema_retrieved.Stores.Length == 2, "We should have 2 stores");
            this.Log($"Schema successfully retrieved. Stores Count:{schema_retrieved.Stores.Length} ");
            this.Log($"Deleting Database");
            await this.DbFactory.DeleteDatabase(dbName);
            this.Log($"Database Successfully Deleted.");
            this.Log($"Test Successfully Completed.\r\n=======================");
        }

        public async Task DeleteDatabase()
        {
            var dbName = "test" + new Random().Next(1, 1000);
            var exists = await this.DbFactory.DatabaseExists(dbName);
            this.Assert(!exists, "DatabaseExits should return false.");
            var db = await this.DbFactory.GetDatabase(dbName);
            await db.GetStore(new StoreSchema { StoreName = "test", PrimaryKey = new IndexData("id", "id") });
            this.Log($"Database Creared. {dbName}");
            await Task.Delay(500);
            exists = await this.DbFactory.DatabaseExists(dbName);
            this.Assert(exists, "DatabaseExists should return true.");
            var schema = await db.GetSchema();
            this.Log($"Database schema is retrieved. DbName: {schema.DbName}.");
            await this.DbFactory.DeleteDatabase(dbName);
            exists = await this.DbFactory.DatabaseExists(dbName);
            this.Log($"Database Deleted");
            this.Assert(!exists, "DatabaseExists should return false.");
            this.Log($"Test Successfully Completed.\r\n=============================");
        }

        public async Task Put()
        {
            var dbName = "db_" + new Random().Next(1, 10000);
            var schema = new StoreSchema { StoreName = "contacts", PrimaryKey = new IndexData("id") };
            var db = await this.DbFactory.GetDatabase(dbName);
            var store = await db.GetStore<PersonModel>();
            var count = 1000;
            var items = Enumerable.Range(0, count)
                .Select(x => new PersonModel
                {
                    Id = x.ToString(),
                    Name = $"Name {Guid.NewGuid()}",
                    Age = new Random().Next(10, 80),
                })
                .ToArray();
            var dt = Stopwatch.StartNew();

            await store.Put(items);
            this.Log($" {count} items inserted in {dt.ElapsedMilliseconds} milliseconds.");

            var schemaa = await db.GetSchema(true);
            var get_count_result = await store.Count();
            this.Assert(get_count_result == count, $"Count should be {count} it is {get_count_result}");
            var id12 = items[0].Id;
            var age = items[0].Age;

            dt = Stopwatch.StartNew();
            var items_fetched = await store.FetchAll();

            this.Log($" {items_fetched.Count()} Items fetched in {dt.ElapsedMilliseconds} milliseconds");

            dt = Stopwatch.StartNew();

            items_fetched = await store.FetchAll(q => q.Skip(5).Take(10));

            this.Log($" {items_fetched.Count()} Items fetched in {dt.ElapsedMilliseconds} milliseconds. {items_fetched.ToArray()[0].Id}");

            items_fetched = await store.FetchAll(q =>
            {
                q.Filter(Filter.Eq(Filter.Prop("age"), Filter.Val(21)));
            });
            items_fetched.ToList().ForEach(x => this.Assert(x.Age == 21, "should be 21"));

            items_fetched = await store.FetchAll(q =>
            {
                q.Filter(Filter.LT(Filter.Prop("age"), Filter.Val(21)));
            });
            items_fetched.ToList().ForEach(x => this.Assert(x.Age < 21, "should < 21"));

            items_fetched = await store.FetchAll(q =>
            {
                q.Filter(Filter.GT(Filter.Prop("age"), Filter.Val(21)));
            });
            items_fetched.ToList().ForEach(x => this.Assert(x.Age > 21, "should > 21"));

            items_fetched = await store.FetchAll(q =>
            {
                q.Filter(Filter.Contains(Filter.Prop("id"), Filter.Val("9")));
            });

            items_fetched.ToList().ForEach(x => this.Assert(x.Id.Contains("9"), "should contain 9"));

            items_fetched = await store.FetchAll(q =>
            {
                q.Filter(Filter.Like(Filter.Prop("id"), Filter.Val("1*9")));
            });
            items_fetched.ToList().ForEach(x => this.Assert(x.Id.StartsWith("1") && x.Id.EndsWith("9"), "should be 1*9"));

            items_fetched = await store.FetchAll(q =>
            {
                q.Filter(Filter.Or(Filter.Eq(Filter.Prop("age"), Filter.Val(21)), Filter.Eq(Filter.Prop("age"), Filter.Val(41))));
            });

            var expected = items.Where(x => x.Age == 21 || x.Age == 41);
            this.Assert(expected.Count() == items_fetched.Count(), $"Should be equal, {expected.Count()}!={items_fetched.Count()}");

            items_fetched.ToList().ForEach(x => this.Assert(x.Age == 21 || x.Age == 41, "should be 21"));

            items_fetched = await store.FetchAll(q =>
            {
                q.Filter(Filter.And(Filter.Eq(Filter.Prop("age"), Filter.Val(21)), Filter.Eq(Filter.Prop("age"), Filter.Val(41))));
            });

            this.Assert(items_fetched.Count() == 0, "Should be zero");

            dt = Stopwatch.StartNew();

            items_fetched = await store.FetchAll(q => q.OrderBy("Age"));
            var items_ordered = items.OrderBy(x => x.Age).ToArray();
            this.Log($"{items_fetched.Count()} items fetched with order by age in {dt.ElapsedMilliseconds} milliseconds");
            this.Assert(items_fetched.Where((x, i) => items_ordered[i].Age != x.Age).FirstOrDefault() == null, "Order mismatch");

            dt = Stopwatch.StartNew();
            items_fetched = await store.FetchAll(q => q.SelectKeys());
            this.Log($"{items_fetched.Count()} items fetched with Select Keys in {dt.ElapsedMilliseconds} milliseconds");
            var fff = items_fetched.ToArray()[0];

            await Task.Delay(1000);

            await this.DbFactory.DeleteDatabase(dbName).TimeoutAfter(2000, default, false);

            this.Log($"Test Successfully Completed.\r\n===================");
        }

        // Items can be fetched by their IDs
        public async Task FindByID()
        {
            // Creating test environment
            var dbName = $"ID_Fetch_Test{new Random().Next(1, 100)}";
            var store = await
                (await this.DbFactory.GetDatabase(dbName))
                .GetStore<TestModel>();

            var itemCount = 10;
            var data = TestModel.GetSampleData(itemCount);
            await store.Put(data);
            this.Assert(await this.DbFactory.DatabaseExists(dbName), "DatabaseExists should return true");
            this.Assert((await store.Count()) == itemCount, "Data is not what was expected to continue with the test");

            // Get Item
            var item = data[0];
            var res = await store.GetByID(item.GuID.ToString());
            this.Assert(res.GuID.ToString() == item.GuID.ToString(), "Item fetched incorrectly!");
            this.Assert(res.Number == item.Number, "Just Double Cheking, Item is undoubtedly incorrect!");
            this.Log("Test Passed, Item was successfully fetched");
            this.Log($"Expected = {item.GuID}");
            this.Log($"Result = {res.GuID}");

            // Get nonexistent item should return null
            var tempItem = new TestModel { GuID = Guid.NewGuid(), Number = 55 };
            var nullItem = await store.GetByID(tempItem.GuID.ToString());
            this.Assert(nullItem == null, "Item should not have been found!");
            this.Log("Test Passed, Nonexistent ItemGet returned null");

            // Test CleanUp
            await this.DbFactory.DeleteDatabase(dbName);
            var exists = await this.DbFactory.DatabaseExists(dbName);
            this.Assert(!exists, "DatabaseExists should NOT return TRUE!.");
            this.Log($"Test Successfully Completed.\r\n=============================");
        }

        // Items can be deleted by their IDs
        public async Task DeleteById()
        {
            // Creating test environment
            var dbName = $"ID_Delete_Test{new Random().Next(1, 100)}";
            var store = await
                (await this.DbFactory.GetDatabase(dbName))
                .GetStore<TestModel>();
            var itemCount = 10;
            var data = TestModel.GetSampleData(itemCount);
            await store.Put(data);
            this.Assert(await this.DbFactory.DatabaseExists(dbName), "DatabaseExists should return true");
            this.Assert((await store.Count()) == itemCount, "Data is not what was expected to continue with the test");

            // delete item
            var item = data[0];
            var res = await store.DeleteByID(item.GuID.ToString());
            var storeCount = await store.Count();
            this.Assert(res, "Success of the deletation should have been true");
            this.Assert(await store.GetByID(item.GuID.ToString()) == null, "GetItem should have returned null!");
            this.Assert(storeCount == itemCount - 1, "store items haven't been reduced!");
            this.Log("Test Passed, Item was successfully deleted");
            this.Log($"Expected length = {itemCount - 1}");
            this.Log($"Result = {storeCount}");

            // Test CleanUp
            await this.DbFactory.DeleteDatabase(dbName);
            var exists = await this.DbFactory.DatabaseExists(dbName);
            this.Assert(!exists, "DatabaseExists should NOT return TRUE!.");
            this.Log($"Test Successfully Completed.\r\n=============================");
        }

        // Items can be deleted with expression
        public async Task DeleteExpression()
        {
            // Creating test environment
            var dbName = $"Delete_Expression_Test{new Random().Next(1, 100)}";
            var store = await
                (await this.DbFactory.GetDatabase(dbName))
                .GetStore<TestModel>();
            var itemCount = 10;
            var data = TestModel.GetSampleData(itemCount);
            await store.Put(data);
            this.Assert(await this.DbFactory.DatabaseExists(dbName), "DatabaseExists should return true");
            this.Assert((await store.Count()) == itemCount, "Data is not what was expected to continue with the test");

            // delete
            var res = await store.DeleteWhere(x => x.Number < 5);
            var t1 = data.FirstOrDefault(x => x.Number == 4);
            var t2 = data.FirstOrDefault(x => x.Number == 5);
            var storeCount = await store.Count();
            this.Assert(res, "Success of the deletation should have been true");
            this.Assert(await store.GetByID(t1.GuID.ToString()) == null, "GetItem should have returned null!");
            this.Assert((await store.GetByID(t2.GuID.ToString())).GuID == t2.GuID, "GetItem should not have returned null!");
            this.Assert(storeCount == itemCount - 5, "store items haven't been reduced!");
            this.Log("Test Passed, Item was successfully deleted");
            this.Log($"Expected length = {itemCount - 5}");
            this.Log($"Result = {storeCount}");

            // Test CleanUp
            await this.DbFactory.DeleteDatabase(dbName);
            var exists = await this.DbFactory.DatabaseExists(dbName);
            this.Log($"Test Successfully Completed.\r\n=============================");
            this.Assert(!exists, "DatabaseExists should NOT return TRUE!.");
        }

        public async Task Linq()
        {
            var dbName = $"lnq_tests_{new Random().Next(1, 1000)}";
            var db = await this.DbFactory.GetDatabase(dbName);
            var store = await db.GetStore<Account>();
            var seed = Account.Seed(5000);
            var dt = Stopwatch.StartNew();

            this.Log($"Store Successfully Created. Database:{dbName}, Store {store.Name}\r\n Inserting seeds. Count:{seed.Length}");
            await store.Put(seed);
            this.Log($"'{seed.Length}' inserted in {dt.ElapsedMilliseconds} milliseconds.");

            var queryable = store.GetQueryable();
            var id = seed[10].Id;
            var items = await queryable.Where(x => x.Id == id).ToArrayAsync();
            this.Log($"There is {items.Length} item where Id= {id}.");

            var account = seed[5];
            id = account.Id;
            var name = account.Name;
            var item = await queryable.Where(x => x.Id == id && x.Name == name).FirstOrDefaultAsync();

            this.Log($"Item with {item.Name}=={account.Name} ");
            dt = Stopwatch.StartNew();

            items = await queryable.Where(x => x.Id > 2000).ToArrayAsync();
            this.Log($"{items.Length} items fetched with id>2000 in {dt.ElapsedMilliseconds} milliseconds.");

            dt = Stopwatch.StartNew();
            items = await queryable.Where(x => x.Id == 12 || x.Id == 120).ToArrayAsync();
            this.Log($"{items.Length} items fetched with id==12 || id==120 {dt.ElapsedMilliseconds} milliseconds.");

            // SKip and Take
            var expected = seed.Where(x => x.Id > 100).Skip(10).Take(5).ToArray();
            dt = Stopwatch.StartNew();
            items = await store.GetQueryable().Where(x => x.Id > 100).Skip(10).Take(5).ToArrayAsync();
            this.Assert(items.Length == expected.Length, "Should be equal.");
            this.Assert(items[0].Id == expected[0].Id, "Should be equal.");
            this.Log($"{items.Length} Items fetched with skip(10), take(5) in {dt.ElapsedMilliseconds} milliseconds.");

            dt = Stopwatch.StartNew();
            items = await queryable.Where(x => x.Id < 200).OrderBy(x => x.Name).ToArrayAsync();
            var actual = seed.Where(x => x.Id < 200).OrderBy(x => x.Name).ToArray();
            for (var i = 0; i < items.Length; i++)
            {
                var a = actual[i];
                this.Assert(items[i].Id == actual[i].Id && items[i].Name == actual[i].Name, "Should be equal");
            }

            await Task.Delay(1000);
            this.Log($"Deleting Database...");
            await this.DbFactory.DeleteDatabase(dbName);
            this.Log($"Test Successfully Completed.\r\n================");
        }

        public async Task StoreEvents()
        {
            await this.Bus.Subscribe("put", m => { }, true);

            var dbName = "hhh";
            var db = await this.DbFactory.GetDatabase(dbName);
            var store = await db.GetStore<Account>();
            await store.Put(new Account[]
            {
                new Account
                {
                    Id = 1,
                    Name = "lll",
                },
            });
            await Task.Delay(1000);
            var a = new StoreSchema { StoreName = "storename", PrimaryKey = new IndexData { Name = "id", KeyPath = "id", Unique = true } };
        }
    }
}
