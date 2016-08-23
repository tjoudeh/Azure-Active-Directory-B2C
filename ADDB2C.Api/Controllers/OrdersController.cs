using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace ADDB2C.Api.Controllers
{

    [Authorize]
    [RoutePrefix("api/Orders")]
    public class OrdersController : ApiController
    {
        CloudTable cloudTable = null;

        public OrdersController()
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            cloudTable = tableClient.GetTableReference("orders");

            // Create the table if it doesn't exist.
            // Uncomment the below line if you are not sure if the table has been created already
            // No need to keep checking that table exixts or not.
            //cloudTable.CreateIfNotExists();
        }

        [Route("")]
        public IHttpActionResult Get()
        {
         
            //This will be read from the access token claims.
            var userId = User.Identity.Name;

            TableQuery <OrderEntity> query = new TableQuery<OrderEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId));

            var orderEntitis = cloudTable.ExecuteQuery(query).Select(
                o => new OrderModel() {
                OrderID = o.RowKey,
                ShipperName = o.ShipperName,
                ShipperCity = o.ShipperCity,
                TS = o.Timestamp
                });

            return Ok(orderEntitis);
        }

        [Route("")]
        public IHttpActionResult Post (OrderModel order)
        {
            //This will be read from the access token claims.
            var userId = User.Identity.Name;

            OrderEntity orderEntity = new OrderEntity(userId);

            orderEntity.ShipperName = order.ShipperName;
            orderEntity.ShipperCity = order.ShipperCity;

            TableOperation insertOperation = TableOperation.Insert(orderEntity);

            // Execute the insert operation.
            cloudTable.Execute(insertOperation);

            order.OrderID = orderEntity.RowKey;

            order.TS = orderEntity.Timestamp;

            return Ok(order);
        }
    }

    #region Classes

    public class OrderModel
    {
        public string OrderID { get; set; }
        public string ShipperName { get; set; }
        public string ShipperCity { get; set; }
        public DateTimeOffset TS { get; set; }
    }

    public class OrderEntity : TableEntity
    {
        public OrderEntity(string userId)
        {
            this.PartitionKey = userId;
            this.RowKey = Guid.NewGuid().ToString("N");
        }

        public OrderEntity() { }

        public string ShipperName { get; set; }

        public string ShipperCity { get; set; }

    }

    #endregion
}