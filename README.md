Azure SQL Database Transient Demo
=================================

This code gives a demonstration of the impact that [transient errors](http://social.technet.microsoft.com/wiki/contents/articles/4235.retry-logic-for-transient-failures-in-windows-azure-sql-database.aspx) have when using Azure [SQL Database](http://www.windowsazure.com/en-us/services/data-management/).

What is the demo doing / how does it work?
------------------------------------------

The demo consists of a website that allows the user to search for movies within a fictional movie database of 129,600 records that have title and year of manufacture.

The database was [populated by](SQLAzureDemo/Database/Migrations):

1. Grabbing the top 10 results from the [IMDB API](http://imdbapi.org) for movies starting with 0-9 and a-z (returns 360 records)
2. Combinatorially combining the results together - concatenating the titles and adding the year of creation - to quickly generate a large number of records

The website provides a number of search pages over this database that perform a contains search over the title for a given search term and present a paginated table of results.

The website provides a number of HTTP endpoints (note: the [] denotes an optional component of the URL):

* `/` - Dashboard of the status of the demo in the last 5 minutes
* `/Transient[?q=${SEARCH_TERM}[&page=${PAGE}]]` - Search for movies in a non-transient protected way using NHibernate
* `/Resilient[?q=${SEARCH_TERM}[&page=${PAGE}]]` - Search for movies in a transient protected way using the [NHibernate.SqlAzure](https://github.com/robdmoore/NHibernate.SqlAzure) library

How do I set-up and run the demo?
---------------------------------

1. Set up 4 Azure Web Sites on the free-tier to auto-deploy from a fork of this repository (or manually deploy this code to each one). **Note: Any other Azure Web Sites you have in the free tier of the same subscription will likely be taken out of action for up to an hour when you run this** - if possible put it in a standalone subscription.
2. Create a SQL Azure server and database and take note of the credentials you set up for the admin user. **It's strongly encouraged that you create a separate database because this will stop any other applications using this database from working for a period of time.**
3. Create an Azure Storage account and note the primary account key
4. Go to the Configure tab for each of your Azure Web Sites in the [Management Portal](https://manage.windowsazure.com) and add a connection string replacement for:
	* `Database` - `Server=tcp:${YOUR_SERVER}.database.windows.net,1433;Database=${YOUR_DATABASE};User ID=${YOUR_USER}@${YOUR_SERVER};Password=${YOUR_PASSWORD};Trusted_Connection=False;Encrypt=True;Connection Timeout=30;`
	* `AzureStorage` - `DefaultEndpointsProtocol=https;AccountName=${YOUR_ACCOUNT_NAME};AccountKey=${YOUR_PRIMARY_KEY};`
5. Visit the websites to make sure that they work - perform a transient and non transient search via the menu items at the top and then view the homepage to see if they show up.
6. Change the domain names in `SQLAzureDemo.HttpFlooder\Program.cs` from `mscloudperthdemo${X}.azurewebsites.net` to `yourazurewebsitesdomain${X}.azurewebsites.net`.
7. Run the `SQLAzureDemo.HttpFlooder` project

What should I expect to see when running the demo?
--------------------------------------------------

The console output for the Http Flooder will show you... todo



How do I run the demo website locally?
--------------------------------------

1. Download the Azure SDK and run the Azure Storage Emulator
2. Create a database called `SQLAzureDemo` on your local SQL Express database (or change the connection string under `Database` in `web.config` to point to a different database)
3. Run the `SQLAzureDemo` web project

