using ConfDbRipper.Model;
using Microsoft.Data.SqlClient;

namespace ConfDbRipper
{
    internal static class SqlQueryService
    {
        internal static IEnumerable<User> GetUsers(SqlConnection connection)
        {
            var users = new List<User>();

            try
            {                
                var query = "SELECT [user_key] ,[username] ,[lower_username] FROM [dbo].[user_mapping]";

                using (var command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var userId = reader.GetString(0);
                            var upn = reader.GetString(1);

                            if (upn.EndsWith("@microsoft.com"))
                            {
                                users.Add(new User { ConfluenceId = userId, Upn = upn });
                            }
                        }
                    }
                }                
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            return users;
        }

        internal static IEnumerable<Space> GetSpaces(SqlConnection connection)
        {
            var spaces = new List<Space>();

            try
            {
                var query = "SELECT [SPACENAME] ,[SPACEID] ,[CREATOR] FROM [dbo].[SPACES]";

                using (var command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var title = reader.GetString(0);
                            var id = reader.GetDecimal(1); 
                            var authorId = reader.GetString(2);

                            spaces.Add(new Space { AuthorId = authorId, Id = id.ToString(), Title = title }); ;
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            return spaces;
        }

        /// <summary>
        /// Gets all content for the specified user
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        internal static IEnumerable<Content> GetContent(SqlConnection connection, User user)
        {
            var content = new List<Content>();

            try
            {
                var query = $"SELECT [dbo].CONTENT.[CONTENTID], [TITLE], [BODY], [CREATOR], [CONTENTTYPE], [CREATIONDATE], [LASTMODDATE] FROM [dbo].CONTENT INNER JOIN [dbo].BODYCONTENT ON( [dbo].CONTENT.CONTENTID = [dbo].BODYCONTENT.CONTENTID) WHERE CONTENTTYPE = 'PAGE' AND CREATOR = '{user.ConfluenceId}'";

                using (var command = new SqlCommand(query, connection))
                {                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var contentId = reader.GetDecimal(0);
                            var title = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                            var body = reader.GetString(2);
                            var creator = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                            
                            var created = reader.GetDateTime(5);
                            var modified = reader.GetDateTime(6);

                            if (!string.IsNullOrWhiteSpace(creator))
                            {
                                content.Add(new Content { 
                                    Id = contentId.ToString(), 
                                    Title = title, 
                                    Body = body, 
                                    AuthorId = creator.ToString(),
                                    Created = created,
                                    Modified = modified
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            return content;
        }

        internal static IEnumerable<Content> GetContent(SqlConnection connection, Space space)
        {
            var content = new List<Content>();

            try
            {
                var query = $"SELECT [dbo].CONTENT.[CONTENTID], [TITLE], [BODY], [CREATOR], [CONTENTTYPE], [CREATIONDATE], [LASTMODDATE], [SPACEID] FROM [dbo].CONTENT INNER JOIN [dbo].BODYCONTENT ON( [dbo].CONTENT.CONTENTID = [dbo].BODYCONTENT.CONTENTID) WHERE CONTENTTYPE = 'PAGE' AND SPACEID = '{space.Id}'";

                using (var command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var contentId = reader.GetDecimal(0);
                            var title = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                            var body = reader.GetString(2);
                            var creator = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);

                            var created = reader.GetDateTime(5);
                            var modified = reader.GetDateTime(6);

                            if (!string.IsNullOrWhiteSpace(creator))
                            {
                                content.Add(new Content
                                {
                                    Id = contentId.ToString(),
                                    Title = title,
                                    Body = body,
                                    AuthorId = creator.ToString(),
                                    Created = created,
                                    Modified = modified
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            return content;
        }
    }
}
