﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Messaging
{
    public class Message
    {
        public Dictionary<string, string> Headers { get; set; }
        public string Id { get; set; }
        public string Subject { get; set; }
        public object Payload { get; set; }
        public string ReplyTo { get; set; }
        public int? StatusCode { get; set; }
        public string From { get; set; }
        public Message()
        {
            Id = new Random().Next(1, 100000).ToString();
        }
        public string GetHeaderValue(string key)
        {
            return Headers != null && Headers.TryGetValue(key, out var res) ? res : null;
        }
        public void SetHeaderValue(string key, string value)
        {
            Headers = Headers ?? new Dictionary<string, string>();
            Headers[key] = value;
        }
        public Message(string subject, object paylad, string id = null, string replyTo = null, string from = null, int? statusCode = null)
        {
            Subject = subject;
            Payload = paylad;
            Id = id ?? new Random().Next(1, 100000).ToString();
            From = from;
            StatusCode = statusCode;
        }
        public T GetPayload<T>()
        {
            return ShilaFeaturesExtensions.SafeCast<T>(Payload);
            //var options = new JsonSerializerOptions
            //{
            //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            //};
            //var str = JsonSerializer.Serialize(Payload, options);
            //return Payload == null
            //    ? default
            //    : ShilaFeaturesExtensions.SafeDeserialize<T>(JsonSerializer.Serialize(Payload, options), options);
        }
        public bool IsReply() => !string.IsNullOrWhiteSpace(ReplyTo);
        public bool IsError() => IsReply() && StatusCode.HasValue && StatusCode.Value != 0;
        public override string ToString()
        {
            return $"Subject: {Subject}, Payload:'{ShilaFeaturesExtensions.SafeSerialize(Payload)}'";
        }
    }
    public class Message<T> : Message
    {
        public new T Payload { get; set; }
        public Message()
        {

        }
        public Message(string subject, T paylad, string id = null, string replyTo = null)
            : base(subject, paylad, id, replyTo)
        {
            Payload = paylad;
        }

    }
}

