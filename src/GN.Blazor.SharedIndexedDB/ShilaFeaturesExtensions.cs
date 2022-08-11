using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using GN.Blazor.SharedIndexedDB.Services;
using System.Threading;
using System.Text.Json;
using System.Reflection;
using GN.Blazor.SharedIndexedDB.Models;
using System.Buffers;

namespace GN.Blazor.SharedIndexedDB
{
    public static class ShilaFeaturesExtensions
    {
        public static IServiceCollection AddShilaFeatures(this IServiceCollection services)
        {
            services.AddMediatR(typeof(ShilaFeaturesExtensions).Assembly);
            services.AddTransient<IShilaDispatcher, ShilaDispatcher>();
            services.AddSingleton<ISharedWorkerAdapterFactory, SharedWorkerAdapterFactory>();
            services.AddSingleton<ISharedWorkerAdapter>(sp => sp.GetService<ISharedWorkerAdapterFactory>().CreateAdapter("SharedWorker"));
            services.AddSingleton<IIndexedDbFactory, IndexedDbFactory>();
            services.AddSingleton<IMessageBus, MessageBus>();

            return services;
        }
        internal static async ValueTask SafeDisposeAsync(this IAsyncDisposable disposable)
        {
            if (disposable != null)
                await disposable.DisposeAsync();

        }

        internal static string SafeSerialize(object obj, JsonSerializerOptions options = null)
        {
            if (obj == null)
                return null;
            try
            {
                return JsonSerializer.Serialize(obj, options ?? DefaultSerializationOptions);
            }
            catch
            {

            }
            return null;

        }
        // https://www.meziantou.net/avoid-performance-issue-with-jsonserializer-by-reusing-the-same-instance-of-json.htm
        // Avoid new instances of options because of performance issues.
        internal static JsonSerializerOptions DefaultSerializationOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        internal static T SafeDeserialize<T>(string text, JsonSerializerOptions options = null)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(text, options ?? DefaultSerializationOptions);
            }
            catch { }
            try
            {
                if (typeof(T) == typeof(string))
                {
                    return (T)(object)text;
                }
            }
            catch { }
            return default(T);
        }
        public static T SafeCast<T>(object input, JsonSerializerOptions options = null)
        {
            if (input != null && input is JsonElement je)
            {
                return je.ToObject<T>(options ?? DefaultSerializationOptions);
            }
            return input == null
                ? default
                : SafeDeserialize<T>(SafeSerialize(input));
        }
        public static T ToObject<T>(this JsonElement element, JsonSerializerOptions options = null)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            using (var writer = new Utf8JsonWriter(bufferWriter))
            {
                element.WriteTo(writer);
            }

            return JsonSerializer.Deserialize<T>(bufferWriter.WrittenSpan, options);
        }
        public static T SafeCastWithKey<T>(object input, PropertyInfo keyProp)
        {
            var result = Activator.CreateInstance<T>();
            if (input != null && input is JsonElement je)
            {
                if (keyProp.PropertyType == typeof(string))
                {
                    keyProp.SetValue(result, je.GetString());
                }
                else if (keyProp.PropertyType == typeof(int))
                {
                    keyProp.SetValue(result, je.GetInt32());
                }
                else
                {
                    throw new Exception($"Unsupported Key Type {keyProp.PropertyType.Name}");
                }
            }
            return result;
        }
        internal static PropertyInfo KeyProp(Type type)
        {
            return type.GetProperties().FirstOrDefault(x => x.GetCustomAttribute<KeyAttribute>() != null) ??
               type.GetProperty("Id");

        }

        public static async Task TimeoutAfter(this Task task, int millisecondsTimeout, CancellationToken token = default, bool Throw = true)
        {
            if (task == await Task.WhenAny(task, Task.Delay(millisecondsTimeout, token)))
                await task.ConfigureAwait(false);
            else if (Throw)
                throw new TimeoutException();

        }
        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, int millisecondsTimeout, CancellationToken token = default, bool Throw = true)
        {
            if (task == await Task.WhenAny(task, Task.Delay(millisecondsTimeout, token)))
                return await task.ConfigureAwait(false);
            else if (Throw)
                throw new TimeoutException();
            return default(TResult);
        }
        public static bool PatternMatchesSubject(string pattern, string subject)
        {
            return pattern == "*" || pattern == subject;
        }
        internal static string ToCamel(this string str)
        {
            if (!string.IsNullOrWhiteSpace(str) && str.Length > 0)
            {
                return Char.ToLowerInvariant(str[0]) + str.Substring(1);
            }
            return str;
        }
    }
}
