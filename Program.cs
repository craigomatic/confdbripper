

using ConfDbRipper;
using Microsoft.Data.SqlClient;

//TODO: put your SQL Server connection string here (you might have to put your IP address onto the allow list of your SQL server also)
var connectionString = "";

var outputDir = "C:\\ConfluenceDump";
System.IO.Directory.CreateDirectory(outputDir);

using (SqlConnection connection = new SqlConnection(connectionString))
{
    connection.Open();

    var users = SqlQueryService.GetUsers(connection);
    var spaces = SqlQueryService.GetSpaces(connection);
    
    foreach (var space in spaces)
    {
        var content = SqlQueryService.GetContent(connection, space);
        var dir = Path.Combine(outputDir, space.Title);

        //create a folder for each space and dump the html to that folder
        System.IO.Directory.CreateDirectory(dir);

        //content may have multiple revisions, we'll just take the most recent
        var groupedContent = content.GroupBy(c => $"{c.AuthorId}{c.Title}");

        foreach (var contentGrouping in groupedContent)
        {            
            var contentItem = contentGrouping.OrderByDescending(c => c.Modified).First();
            var title = contentItem.Title;
            
            while(title.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                var badIdx = title.IndexOfAny(Path.GetInvalidFileNameChars());
                title = title.Remove(badIdx);
            }

            System.IO.File.WriteAllText(Path.Combine(dir, $"{title}_{contentItem.Modified.ToFileTime()}.html"), contentItem.Body);
        }
    }
}