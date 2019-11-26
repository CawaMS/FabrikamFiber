namespace FabrikamFiber.Web
{
    using System.Configuration;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.Table;
    using DAL.Models;

    public static class FabrikamFiberAzureStorage
    {
        private const string ResourceName = "fabrikamfiber";

        public static void GetStorageBlobData()
        {
            var blobClient = GetStorageAccount().CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(ResourceName);
            container.CreateIfNotExists();
            container.SetPermissions(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });

            var blockBlob = container.GetBlockBlobReference("fabrikam.txt");
            blockBlob.UploadText("TestBlobContent");

            string result = string.Empty;
            foreach (var item in container.ListBlobs(null, false))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    result += string.Format("Block blob of length {0}: {1}", blob.Properties.Length, blob.Uri);

                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {
                    CloudPageBlob pageBlob = (CloudPageBlob)item;
                    result += string.Format("Page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri);

                }
                else if (item.GetType() == typeof(CloudBlobDirectory))
                {
                    CloudBlobDirectory directory = (CloudBlobDirectory)item;
                    result += string.Format("Directory: {0}", directory.Uri);
                }
            }
        }

        public static void GetStorageQueueData()
        {
            var queueClient = GetStorageAccount().CreateCloudQueueClient();
            
            var queue = queueClient.GetQueueReference(ResourceName);
            queue.CreateIfNotExists();

            queue.AddMessage(new CloudQueueMessage("TestMessage"));
            var message = queue.GetMessage();
            if (message != null)
            {
                string result = message.AsString;
            }
        }

        public static void GetStorageTableData(Customer customer)
        {
            var tableClient = GetStorageAccount().CreateCloudTableClient();

            var table = tableClient.GetTableReference(ResourceName);
            table.CreateIfNotExists();

            var customerEntity = new CustomerEntity(customer.LastName, customer.FirstName);

            try
            {
                var insertOperation = TableOperation.Insert(customerEntity);
                table.Execute(insertOperation);
            }
            catch { }

            try
            {
                var getOperation = TableOperation.Retrieve(customerEntity.PartitionKey, customerEntity.RowKey);
                table.Execute(getOperation);
            }
            catch { }
        }

        private static CloudStorageAccount GetStorageAccount()
        {
            return CloudStorageAccount.Parse(
                System.Configuration.ConfigurationManager.ConnectionStrings["FabrikamStorage"].ConnectionString);
        }

        private class CustomerEntity : TableEntity
        {
            public CustomerEntity(string lastName, string firstName)
            {
                this.PartitionKey = lastName;
                this.RowKey = firstName;
            }

            public CustomerEntity() { }
        }
    }
}