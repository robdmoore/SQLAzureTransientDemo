Azure SQL Database Transient Demo
=================================

This code gives a demonstration of the impact that [transient errors](http://social.technet.microsoft.com/wiki/contents/articles/4235.retry-logic-for-transient-failures-in-windows-azure-sql-database.aspx) have when using Azure [SQL Database](http://www.windowsazure.com/en-us/services/data-management/).

What is the demo doing / how does it work?
------------------------------------------

The demo consists of a website that allows the user to search for movies within a fictional movie database of 129,600 records that have title and year of manufacture.

The database was [populated by](SQLAzureDemo/Database/Migrations):

1. Grabbing the top 10 results from the [OMDB API](http://omdbapi.com) for movies starting with 0-9 and a-z (returns 360 records)
2. Combinatorially combining the results together - concatenating the titles and adding the year of creation - to quickly generate a large number of records

The website provides a number of search pages over this database that perform a contains search over the title for a given search term and present a paginated table of results.

The website provides a number of HTTP endpoints (note: the [] denotes an optional component of the URL):

* `/` - Dashboard of the status of the demo in the last 5 minutes
* `/TransientNHibernate[?q=${SEARCH_TERM}[&page=${PAGE}]]` - Search for movies using NHibernate in a non-transient protected
* `/ResilientNHibernate[?q=${SEARCH_TERM}[&page=${PAGE}]]` - Search for movies using NHibernate in a transient protected way using the [NHibernate.SqlAzure](https://github.com/robdmoore/NHibernate.SqlAzure) library
* `/TransientEntityFramework[?q=${SEARCH_TERM}[&page=${PAGE}]]` - Search for movies using EntityFramework in a non-transient protected way
* `/ResilientEntityFramework[?q=${SEARCH_TERM}[&page=${PAGE}]]` - Search for movies using EntityFramework in a transient protected way using the [ReliableDbProvider](https://github.com/robdmoore/ReliableDbProvider) library

How do I set-up and run the demo?
---------------------------------

1. Set up 4 Azure Web Sites on the free-tier to auto-deploy from a fork of this repository (or manually deploy this code to each one). **Note: Any other Azure Web Sites you have in the free tier of the same subscription will likely be taken out of action for up to an hour when you run this** - if possible put it in a standalone subscription.
2. Create a SQL Azure server and database and take note of the credentials you set up for the admin user. **It's strongly encouraged that you create a separate database because this will stop any other applications using this database from working for a period of time.**
3. Create an Azure Storage account and note the primary account key
4. Go to the Configure tab for each of your Azure Web Sites in the [Management Portal](https://manage.windowsazure.com) and add a connection string replacement for:
	* `Database` - `Server=tcp:${YOUR_SERVER}.database.windows.net,1433;Database=${YOUR_DATABASE};User ID=${YOUR_USER}@${YOUR_SERVER};Password=${YOUR_PASSWORD};Trusted_Connection=False;Encrypt=True;Connection Timeout=30;`
	* `AzureStorage` - `DefaultEndpointsProtocol=https;AccountName=${YOUR_ACCOUNT_NAME};AccountKey=${YOUR_PRIMARY_KEY};`
5. Visit the websites to make sure that they work (note: the first request will take a while because the database will be built) - perform a transient and non transient search via the menu items at the top and then view the homepage to see if they show up.
6. Change the domain names in `SQLAzureDemo.HttpFlooder\Program.cs` from `mscloudperthdemo${X}.azurewebsites.net` to `yourazurewebsitesdomain${X}.azurewebsites.net`.
7. (Optional) Deploy anther Azure Web Site with the same code and same values for `Database` and `AzureStorage` connection strings in a different Azure subscription so that you can view the dashboard when the main sites are getting hammered
7. Run the `SQLAzureDemo.HttpFlooder` project

**Note:** This test is likely to use all of the resources allotted to your free websites in which case they will be blocked for a while.

What should I expect to see when running the demo?
--------------------------------------------------

The Http Flooder will fire off 300 HTTP requests to the websites simultaneously. Each HTTP request will be a randomly generated search term and page.

By combining the sheer number of simultaneous HTTP requests, across 4 websites to get enough load to the SQL Server simultaneously and given that most queries perform a meaty pagination SQL query some of the throttling and timeout transient errors will start occurring for some of the HTTP requests.

The Http Flooder will display the HTTP status code of each request when it finishes and you will know when the transient errors start to occur when these change from `OK` to `Internal Server Error`. At this point in time you will see on the dashboard that the number of HTTP requests processed on the transient endpoint are increasing (but most of them are errors), whereas the number of requests on the resilient endpoint don't change at all (this is because those threads are retrying according to the configured retry policy). Once all of the transient requests have finished (around 150) then the requests to the resilient endpoint will increase (and mostly be successful). You can also note that the average processing time will be much larger for the resilient ones since they had to wait for the retries to be successful.

In order to see the data collected for the runs you can look at:

* `LogEventEntity` table in your storage account to see the logs collected from the application - results from running migrations - this will help diagnose any problems in running the initial database migrations. The partition key is the ticks when the log was made prepended by a "0" (like Windows Azure Diagnostics).
* `ControllerOperation` table in your storage account to see every HTTP request that was processed by the application along with how long it took to process and whether it was an error. The partition key is the ticks when the log was made prepended by a "0" (like Windows Azure Diagnostics).
* `TransientRetry` table in your storage account to see all the retries that the `NHibernate.SqlAzure` library makes, including the SQL error code of the exception being retried, the number retry and the current delay from the retry strategy it is using. The partition key is the ticks when the log was made prepended by a "0" (like Windows Azure Diagnostics).
* `/errors` from the website (or the `Exceptions` table in the SQL database) to see the [StackExchange.Exceptional](https://github.com/NickCraver/StackExchange.Exceptional) log of all uncaught exceptions in the application - this will be for all of the requests that had a transient error that wasn't caught.

How do I run the demo website locally?
--------------------------------------

1. Download the Azure SDK and run the Azure Storage Emulator
2. Create a database called `SQLAzureDemo` on your local SQL Express database (or change the connection string under `Database` in `web.config` to point to a different database)
3. Run the `SQLAzureDemo` web project

