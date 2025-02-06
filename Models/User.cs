using Microsoft.AspNetCore.Mvc.ViewEngines;
using static System.Net.Mime.MediaTypeNames;

namespace Neo4j.Models
{

//	<elementId>	4:6ccf11f6-28db-4d74-9b08-405d5bd79234:17
//<id>	17
//Bio Cyclist
//DateOfBirth	1994-12-12
//Email ryan@example.com
//FirstName   Ryan
//LastName    Allen
//Password    password18
//id  18
	public class User
	{
        public string ElementId { get; set; }
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public DateTime DateOfBirth { get; set; }
        public string Bio { get; set; }
    }

	//	Group
	//<elementId>	4:6ccf11f6-28db-4d74-9b08-405d5bd79234:66
	//<id>	66
	//Description A community for book lovers and readers.
	//Name    Book Club
	//id  6

	public class Group
	{
		public string ElementId { get; set; }
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}

//	Post
//<elementId>	4:6ccf11f6-28db-4d74-9b08-405d5bd79234:92
//<id>	92
//Content New tech gadget review.
//PhotoUrl https://robohash.org/post16
//Timestamp	2023-10-11T13:00:00
//id	16

	public class Post
	{
		public string ElementId { get; set; }
		public int Id { get; set; }
		public string Content { get; set; }
		public string PhotoUrl { get; set; }
		public DateTime Timestamp { get; set; }
	}


	//	Event
	//<elementId>	4:6ccf11f6-28db-4d74-9b08-405d5bd79234:74
	//<id>	74
	//Date	2023-10-15
	//Description A festival showcasing local cuisines.
	//Name    Food Festival 2023
	//id  4

	public class Event
	{
		public string ElementId { get; set; }
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime Date { get; set; }
	}

	//	<elementId>	4:6ccf11f6-28db-4d74-9b08-405d5bd79234:204
	//<id>	204
	//Content What do you recommend next?
	//Timestamp   2023-10-09T16:50:00
	//id	48

	public class RegularComment
	{
		public string ElementId { get; set; }
		public int Id { get; set; }
		public string Content { get; set; }
		public DateTime Timestamp { get; set; }
	}

	//	<elementId>	4:6ccf11f6-28db-4d74-9b08-405d5bd79234:180
	//<id>	180
	//Content Definitely worth a try.
	//Timestamp	2023-10-07T09:30:00
	//id	24

	public class QuotedComment
	{
		public string ElementId { get; set; }
		public int Id { get; set; }
		public string Content { get; set; }
		public DateTime Timestamp { get; set; }
	}

}
