Azure SQL Database Transient Demo
=================================

This code gives a demonstration of the impact that transient errors have when using SQL Azure.

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

What should I expect to see?
----------------------------

todo

