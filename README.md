
<!-- # GN.Blazor.SharedIndexedDB

## What is it

## how it Works -->

## Prerequisites

  The project adds capabilities to the browser’s built in IndexedDB and doesn’t require any additional installations. 
  Although the example project is implemented using blazor server.

## Getting started

  You can clone the whole repository to run the example project, 
  access the source code or even add your own functionalities.
  
  otherwise,
<!--   add the nuget package to your project -->
Add 

```c#
using GN.Blazor.SharedIndexedDB.IndexedDB.
public static async Task Main(string[] args)
{
	var builder = WebAssemblyHostBuilder.CreateDefault(args);
	builder.Services.AddShilaFeatures();
	//codes removed for brevity 
	await builder.Build().RunAsync();
}
```
To your program.cs file.

The above code is copied from blazor server program.cs file.If you are using other or older versions, 
you may not find it. Just keep in mind you should AddShilaFeatures() before the build of your Host.

## Create database and store

Firstly you need to create the database. Databases can hold any number of stores.

To create a database use
```c#
string dbName = "{your_database_name}";
IIndexedDb dataBase = await IIndexedDbFactory.DbFactory.GetDatabase(dbName);
IIndexedDbStore store = await dataBase.GetStore();
```
Stores are created within a database. This is where your data is actually stored.
Most of the time you are working with the store object.
The IIndexedDb.GetStore is a generic function. By default it will create the store for System.Object, but this is just a fallback. You should specify your own class type.

## Data Type

Indexed db is a noSQl database. Which means you don’t actually need a data type to store data in it. 
It is a key value store. so the data is inevitably converted to string.
But most of the time you’re gonna need your own data type to work with.
To define your custom data type, you should create a Schema or use Tags.

### Schema

Stores are created with their own schema. Schemas define the store name, 
its primary key and the properties which should be indexed for ease of access.

#### IndexData

Is a DTO object to specify the property name in your object, 
its key name in the store or whether or not its value is unique to every record or not.

#### StoreSchem

To explicitly define your schema use 
```c#
StoreSchema schema = new StoreSchema 
{ 
    StoreName = "storename" , 
    PrimaryKey = new IndexData { Name="id" , KeyPath = "id" , Unique= true} ,
    Indexes = [],
};
```
Note that primary keys should be unique.

Give your schema the IIndexedDb.GetStore.

```c#
	IIndexedDb.GetStore(schema)
```

#### AttributeUsageAttribute

You can also define your store schema by using tags in your model class
```c#
[Table("your_store_name")]
public class TestModel
{
        [Key]
        public Guid GuID { get; set; }
        [Index]
	public int Number { get; set; }

}
```
You can use [Key(“ID”)] and [Index(“NumberOfSomething”)] to set a different name 
than your c# property name for your model in the store.

In this case the schema is automatically created for the store and 
you don’t need to pass anything to the IIndexedDb.GetStore function if:

```c#
	IIndexedDb.GetStore<TestModel>();
```

	
## Store API
	
After creating your store object. You can start using the database as you please.
Note that you don’t need to keep redefining the store. Keep the object.
  
```c#
string dbName = $"Test_{new Random().Next(1, 100)}";
IIndexedDb dataBase = await this.DbFactory.GetDatabase(dbName);
IIndexedDbStore<TestModel> store = await dataBase.GetStore<TestModel>();
```
	
### Put
	
To add records to your store, give your array of objects to the put function.
```c#
TestModel[] data = {TestModel1,TestModel2,TestModel3};
await store.Put(data);
```
	
### Count
	
Use count to get the length of your store
```c#
long storeLength = await store.Count();

```
  
### Find 
	
You can fetch records either by using their primary key or using a Queryable object.

#### Using Primary Key
	
Find the record by providing its unique primary key.
```c#
TestModel res = await store.GetByID(TestModel1.GuID.ToString());
```
Returns the record as T.

#### IAsyncQueryable
	
You can also request a LinQ **link** IAsyncQueryable<T> to run linq queries on your store.
```c#
IAsyncQueryable<TestModel> queryable = store.GetQueryable();
TestModel[] res = queryable
	.Where(x=>x.Number > 5)
	.Skip(5)
	.Take(10)
	.ToArray();
```
  
### Delete

There are two ways you can delete records from your store.
By using its primary key or by using an expression.


#### Using Primary Key 
	
You can delete a record by providing its unique primary key.
  ```c#
bool success = await store.DeleteByID(TestModel1.GuID.ToString());
  ```
return the succession of deletation.

#### Using Expressions
	
You can also use simple LinQ **link** Expressions to delete records if they meet a certain condition.
```c#
bool success = await store.DeleteWhere(x => x.Number < 5 );
```
return the succession of deletation.
							
## Advanced
							
The library was written in a way that the expansion and customization would be possible.
A detailed explanation of how you can achieve that is being documented and will be uploaded ASAP.



## License and contributions 
							
This is a free open-source project and does ot require a license to use.
Any suggestions, questions, Issues and contributions are welcomed in advance.
In case of usage in your project, a credit would be appriciated.

							
This project is developed under the direct supervision of [Babak Mahmoudi](https://github.com/BabakMahmoudi)
							

Happy codding!
