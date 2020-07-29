using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;

namespace ShoppingCart.EventFeed
{
    public class EventStore:IEventStore
    {
        private static long currentSequenceNumber = 0; 
        private string connectionString = @"server=192.168.18.171;port=3306;uid=user;pwd=!wszlj2390;database=microservice;";

        public Task  Raise(string eventName, object content)
        {
            var writeEventSql = @"insert into EventStore(Name,OccurredAt,Content) values (@Name,@OccurredAt,@Content)";
            var jsonContent = JsonConvert.SerializeObject(content);
            using (var conn=new SqlConnection(connectionString))
            {
                return conn.ExecuteAsync(
                    writeEventSql, new { Name = eventName, OccurredAt = DateTimeOffset.Now, Content = jsonContent }
                    );
            } 
        }

        public async Task<IEnumerable<Event>> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber)
        {
            var readEventsSql =@"select * from EventStore where ID >= @Start and ID <= @End";
            using (var conn = new SqlConnection(connectionString))
            {
                return (await conn.QueryAsync<dynamic>(readEventsSql, new
                {
                    Start = firstEventSequenceNumber,
                    End = lastEventSequenceNumber
                }).ConfigureAwait(false))
                .Select(row =>
                {
                    var content = JsonConvert.DeserializeObject(row.Content);
                    return new Event(row.ID, row.OccurredAt, row.Name, content);
                });
            }
        }
    }
}
