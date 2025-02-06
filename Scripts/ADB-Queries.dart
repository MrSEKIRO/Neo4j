Match (alice:User) - [likes:Likes] -> (post:Post)
Where alice.FirstName = "Alice"
Return post,alice

Match (bob:User {FirstName : "Bob"}), (bob:User)-[:Author]->(bobPost:Post), (firends:User)-[:Likes]->(bobPost:Post)
Return firends

Match (alice:User {FirstName:"Alice"}), (alice)-[:Member_In]->(aliceGroups:Group), (sameUserGroup:User)-[:Member_In]->(aliceGroups)
Return sameUserGroup

// Number of Alice Groups with their names
Match (alice:User {FirstName: "Alice"})-[:Member_In]->(aliceGroups:Group), (member:User)-[:Member_In]->(aliceGroups)
Return aliceGroups.Name, count(member)

Match (user1:User), (user2:User), (user1)-[:Friends_With]->(user2)
where user1.FirstName = "Alice"
Return user1, user2

Match (alice:User {FirstName: "Alice"}), (alice)-[:Friends_With]->(friend:User), (friend)-[:Author]->(post:Post)
Return alice, friend, post

// some more real complex queries
Match (alice:User {FirstName: "Alice"}), (alice)-[:Friends_With]->(friend:User), (friend)-[:Author]->(post:Post), (friend)-[:Likes]->(post:Post)
Return alice, friend, post

// Number of Alice's friends who liked a post
Match (alice:User {FirstName: "Alice"}), (alice)-[:Friends_With]->(friend:User), (friend)-[:Likes]->(post:Post)
Return count(friend)

// Is Any of Alice's friends liked a post
Match (alice:User {FirstName: "Alice"}), (alice)-[:Friends_With]->(friend:User), (friend)-[:Likes]->(post:Post)
Return count(friend) > 0

// Most liked posts for alice authored posts
Match (alice:User {FirstName: "Alice"}), (alice)-[:Author]->(post:Post), (:User)-[like:Likes]->(post:Post)
Return post, count(like) as likes

// return all posts of alice after 2019
Match (alice:User {FirstName: "Alice"}), (alice)-[:Author]->(post:Post)
Where datetime(post.Timestamp) > datetime('2023-11-01T00:00:00')
Return post

////////////////////////////////////////

////////////////////////////////////////

// Find All Posts by a Specific User
MATCH (u:User {FirstName: 'Alice'})-[:Author]->(p:Post)
RETURN p.Content, p.Timestamp

// Find All Groups a User Belongs To
MATCH (u:User {FirstName: 'Alice'})-[:Member_In]->(g:Group)
RETURN g.Name, g.Description

// Find All Comments on a Post
MATCH (p:Post {id: 1})<-[:Comments_On]-(c:RegularComment)
RETURN c.Content, c.Timestamp