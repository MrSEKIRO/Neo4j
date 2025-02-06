using Carter;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Hosting;
using Neo4j.Driver;
using Neo4j.Models;
using System.Xml.Linq;

namespace Neo4j.Endpoints
{
	public class Endpoints : CarterModule
	{
		public Endpoints() : base("api")
		{
		}

		public override void AddRoutes(IEndpointRouteBuilder app)
		{
			app.MapGet("/get-users", async (HttpContext context, IDriver driver) =>
			{
				var session = driver.AsyncSession();
				var users = new List<User>();

				try
				{
					var result = await session.ExecuteReadAsync(async tx =>
					{
						var cursor = await tx.RunAsync("MATCH (u:User) RETURN u");
						return await cursor.ToListAsync();
					});

					foreach(var record in result)
					{
						var user = record["u"].As<INode>();
						users.Add(new User
						{
							ElementId = user.ElementId,
							Id = user["id"].As<int>(),
							FirstName = user["FirstName"].As<string>(),
							LastName = user["LastName"].As<string>(),
							Email = user["Email"].As<string>(),
							Password = user["Password"].As<string>(),
							DateOfBirth = user["DateOfBirth"].As<DateTime>(),
							Bio = user["Bio"].As<string>()
						});
					}
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex);
				}
				finally
				{
					await session.CloseAsync();
				}

				await context.Response.WriteAsJsonAsync(users);
			});

			app.MapGet("/get-posts-liked-by-user", async (HttpContext context, string userName, IDriver driver) =>
			{
				var session = driver.AsyncSession();
				var posts = new List<Post>();

				try
				{
					var result = await session.ExecuteReadAsync(async tx =>
					{
						var cursor = await tx.RunAsync(@"
							Match (u:User) - [likes:Likes] -> (post:Post)
							Where u.FirstName = $userName
							Return post,u",
							new { userName });

						return await cursor.ToListAsync();
					});

					// get user and post
					foreach(var record in result)
					{
						var post = record["post"].As<INode>();
						var user = record["u"].As<INode>();

						posts.Add(new Post
						{
							ElementId = post.ElementId,
							Id = post["id"].As<int>(),
							Content = post["Content"].As<string>(),
							PhotoUrl = post["PhotoUrl"].As<string>(),
							Timestamp = post["Timestamp"].As<DateTime>()
						});
					}
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex);
				}
				finally
				{
					await session.CloseAsync();
				}

				await context.Response.WriteAsJsonAsync(posts);
			});

			app.MapGet("/get-friends-who-liked-posts-by-user", async (HttpContext context, string userName, IDriver driver) =>
			{
				var session = driver.AsyncSession();
				var friends = new List<User>();

				try
				{
					var result = await session.ExecuteReadAsync(async tx =>
					{
						var cursor = await tx.RunAsync(@"
							Match (u:User {FirstName : $userName}), (u)-[:Author]->(uPosts:Post), (friends:User)-[:Likes]->(uPosts)
							Return friends",
							new { userName });

						return await cursor.ToListAsync();
					});

					// get user and post
					foreach(var record in result)
					{
						var friend = record["friends"].As<INode>();

						friends.Add(new User
						{
							ElementId = friend.ElementId,
							Id = friend["id"].As<int>(),
							FirstName = friend["FirstName"].As<string>(),
							LastName = friend["LastName"].As<string>(),
							Email = friend["Email"].As<string>(),
							Password = friend["Password"].As<string>(),
							DateOfBirth = friend["DateOfBirth"].As<DateTime>(),
							Bio = friend["Bio"].As<string>()
						});
					}
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex);
				}
				finally
				{
					await session.CloseAsync();
				}

				await context.Response.WriteAsJsonAsync(friends);
			});

			app.MapGet("/get-friends-in-same-group", async (HttpContext context, string userName, IDriver driver) =>
			{
				var session = driver.AsyncSession();
				var friends = new List<User>();

				try
				{
					var result = await session.ExecuteReadAsync(async tx =>
					{
						var cursor = await tx.RunAsync(@"
							Match (u:User {FirstName : $userName}), (u) - [:Member_In]->(uGroups:Group), (friends:User) - [:Member_In]->(uGroups)
							Return friends",
							new { userName });

						return await cursor.ToListAsync();
					});

					// get user and post
					foreach(var record in result)
					{
						var friend = record["friends"].As<INode>();

						friends.Add(new User
						{
							ElementId = friend.ElementId,
							Id = friend["id"].As<int>(),
							FirstName = friend["FirstName"].As<string>(),
							LastName = friend["LastName"].As<string>(),
							Email = friend["Email"].As<string>(),
							Password = friend["Password"].As<string>(),
							DateOfBirth = friend["DateOfBirth"].As<DateTime>(),
							Bio = friend["Bio"].As<string>()
						});
					}
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex);
				}
				finally
				{
					await session.CloseAsync();
				}

				await context.Response.WriteAsJsonAsync(friends);
			});

			app.MapGet("/get-group-members-count", async (HttpContext context, string userName, IDriver driver) =>
			{
				var session = driver.AsyncSession();
				var groupMembers = new List<GroupMembers>();

				try
				{
					var result = await session.ExecuteReadAsync(async tx =>
					{
						var cursor = await tx.RunAsync(@"
							Match (u:User {FirstName : $userName})- [:Member_In]->(uGroups:Group), (member: User) - [:Member_In]->(uGroups)
							Return uGroups.Name, count(member) as count",
							new { userName });

						return await cursor.ToListAsync();
					});

					// get user and post
					foreach(var record in result)
					{
						groupMembers.Add(new GroupMembers
						{
							GroupName = record["uGroups.Name"].As<string>(),
							Count = record["count"].As<int>()
						});
					}
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex);
				}
				finally
				{
					await session.CloseAsync();
				}

				await context.Response.WriteAsJsonAsync(groupMembers);
			});

			app.MapGet("/get-friends-of-user", async (HttpContext context, string userName, IDriver driver) =>
			{
				var session = driver.AsyncSession();
				var friends = new List<User>();

				try
				{
					var result = await session.ExecuteReadAsync(async tx =>
					{
						var cursor = await tx.RunAsync(@"
							Match (user1: User), (user2: User), (user1) - [:Friends_With]->(user2)
							where user1.FirstName = $userName
							Return user2",
							new { userName });

						return await cursor.ToListAsync();
					});

					// get user and post
					foreach(var record in result)
					{
						var friend = record["user2"].As<INode>();

						friends.Add(new User
						{
							ElementId = friend.ElementId,
							Id = friend["id"].As<int>(),
							FirstName = friend["FirstName"].As<string>(),
							LastName = friend["LastName"].As<string>(),
							Email = friend["Email"].As<string>(),
							Password = friend["Password"].As<string>(),
							DateOfBirth = friend["DateOfBirth"].As<DateTime>(),
							Bio = friend["Bio"].As<string>()
						});
					}
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex);
				}
				finally
				{
					await session.CloseAsync();
				}

				await context.Response.WriteAsJsonAsync(friends);
			});

			app.MapGet("/get-friends-posts", async (HttpContext context, string userName, IDriver driver) =>
			{
				var session = driver.AsyncSession();
				var friendsPosts = new List<FriendsPosts>();

				try
				{
					var result = await session.ExecuteReadAsync(async tx =>
					{
						var cursor = await tx.RunAsync(@"
							Match (alice:User {FirstName: $userName}), (alice)-[:Friends_With]->(friend:User), (friend)-[:Author]->(post:Post)
							Return alice, friend, post",
							new { userName });

						return await cursor.ToListAsync();
					});

					// get user and post
					foreach(var record in result)
					{
						var alice = record["alice"].As<INode>();
						var friend = record["friend"].As<INode>();
						var post = record["post"].As<INode>();

						friendsPosts.Add(new FriendsPosts
						{
							Alice = new User
							{
								ElementId = alice.ElementId,
								Id = alice["id"].As<int>(),
								FirstName = alice["FirstName"].As<string>(),
								LastName = alice["LastName"].As<string>(),
								Email = alice["Email"].As<string>(),
								Password = alice["Password"].As<string>(),
								DateOfBirth = alice["DateOfBirth"].As<DateTime>(),
								Bio = alice["Bio"].As<string>()
							},
							Friend = new User
							{
								ElementId = friend.ElementId,
								Id = friend["id"].As<int>(),
								FirstName = friend["FirstName"].As<string>(),
								LastName = friend["LastName"].As<string>(),
								Email = friend["Email"].As<string>(),
								Password = friend["Password"].As<string>(),
								DateOfBirth = friend["DateOfBirth"].As<DateTime>(),
								Bio = friend["Bio"].As<string>()
							},
							Post = new Post
							{
								ElementId = post.ElementId,
								Id = post["id"].As<int>(),
								Content = post["Content"].As<string>(),
								PhotoUrl = post["PhotoUrl"].As<string>(),
								Timestamp = post["Timestamp"].As<DateTime>()
							}
						});
					}
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex);
				}
				finally
				{
					await session.CloseAsync();
				}
			});

			app.MapGet("/get-posts-liked-by-friends-of-user", async (HttpContext context, string userName, IDriver driver) =>
			{
				var session = driver.AsyncSession();
				var posts = new List<PostWithLikes>();

				try
				{
					var result = await session.ExecuteReadAsync(async tx =>
					{
						var cursor = await tx.RunAsync(@"
							Match (alice: User { FirstName: $userName}), (alice) - [:Friends_With]->(friend: User), (friend) - [:Author]->(post: Post), (: User) - [like: Likes]->(post: Post)
							Return post, count(like) as likes",
							new { userName });

						return await cursor.ToListAsync();
					});

					// get user and post
					foreach(var record in result)
					{
						var post = record["post"].As<INode>();
						var likes = record["likes"].As<int>();

						posts.Add(new PostWithLikes
						{
							ElementId = post.ElementId,
							Id = post["id"].As<int>(),
							Content = post["Content"].As<string>(),
							PhotoUrl = post["PhotoUrl"].As<string>(),
							Timestamp = post["Timestamp"].As<DateTime>(),
							Likes = likes,
						});
					}
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex);
				}
				finally
				{
					await session.CloseAsync();
				}

				await context.Response.WriteAsJsonAsync(posts);
			});

			app.MapGet("/get-post-by-time", async (HttpContext context, string userName, IDriver driver) =>
			{
				var session = driver.AsyncSession();
				var posts = new List<Post>();

				var dateTime = DateTime.Parse("2023-10-01T00:00:00");

				try
				{
					var result = await session.ExecuteReadAsync(async tx =>
					{
						var cursor = await tx.RunAsync(@"
							Match(alice: User { FirstName: $userName}), (alice) - [:Author]->(post: Post)
							Where datetime(post.Timestamp) > datetime($dateTime)
							Return post",
							new { userName, dateTime });

						return await cursor.ToListAsync();
					});

					foreach(var record in result)
					{
						var post = record["post"].As<INode>();

						posts.Add(new Post
						{
							ElementId = post.ElementId,
							Id = post["id"].As<int>(),
							Content = post["Content"].As<string>(),
							PhotoUrl = post["PhotoUrl"].As<string>(),
							Timestamp = post["Timestamp"].As<DateTime>(),
						});
					}
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex);
				}
				finally
				{
					await session.CloseAsync();
				}

				await context.Response.WriteAsJsonAsync(posts);
			});

			app.MapPost("/create-post", async (HttpContext context, Post post, IDriver driver) =>
			{
				var session = driver.AsyncSession();

				try
				{
					await session.ExecuteWriteAsync(async tx =>
					{
						await tx.RunAsync(@"
							Create (p:Post {
								id: $id,
								Content: $content,
								PhotoUrl: $photoUrl,
								Timestamp: $timestamp
							})",
							new
							{
								id = post.Id,
								content = post.Content,
								photoUrl = post.PhotoUrl,
								timestamp = post.Timestamp
							});
					});
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex);
				}
				finally
				{
					await session.CloseAsync();
				}

				await context.Response.WriteAsync("Post created successfully");
			});

			app.MapPost("/like-post", async (HttpContext context, Post post, string userName, IDriver driver) =>
			{
				var session = driver.AsyncSession();

				try
				{
					await session.ExecuteWriteAsync(async tx =>
					{
						await tx.RunAsync(@"
							Match (u:User {FirstName: $userName}), (p:Post {id: $postId})
							Create (u) - [:Likes]->(p)",
							new { userName, postId = post.Id });
					});
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex);
				}
				finally
				{
					await session.CloseAsync();
				}

				await context.Response.WriteAsync("Post liked successfully");
			});

			app.MapGet("/get-post-labels", async (HttpContext context, IDriver driver) =>
			{
				var session = driver.AsyncSession();
				var labels = new List<string>();

				try
				{
					var result = await session.ExecuteReadAsync(async tx =>
					{
						var cursor = await tx.RunAsync(@"
							Match (p:Post {id: 10})
							Return labels(p) as labels");

						return await cursor.ToListAsync();
					});

					foreach(var record in result)
					{
						labels = record["labels"].As<List<string>>();
					}
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex);
				}
				finally
				{
					await session.CloseAsync();
				}

				await context.Response.WriteAsJsonAsync(labels);
			});
		}
	}

	public class PostWithLikes : Post
	{
		public string ElementId { get; set; }
		public int Id { get; set; }
		public string Content { get; set; }
		public string PhotoUrl { get; set; }
		public DateTime Timestamp { get; set; }
		public int Likes { get; set; }
	}

	public class FriendsPosts
	{
		public User Alice { get; set; }
		public User Friend { get; set; }
		public Post Post { get; set; }
	}

	public class GroupMembers
	{
		public string GroupName { get; set; }
		public int Count { get; set; }
	}
}
