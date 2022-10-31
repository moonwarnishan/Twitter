namespace TweetRetweetCommentLike.Services
{
    public class LikeCommentRetweetServices : ILikeCommentRetweetServices
    {
        private readonly IMongoCollection<CommentLikeRetweet> _likeCommentRetweet;
        private readonly INotificationServices _notificationServices;
        public LikeCommentRetweetServices(IOptions<DatabaseSetting.DatabaseSetting> db,INotificationServices notificationServices)
        {
            var client = new MongoClient(db.Value.connectionString);
            var database = client.GetDatabase(db.Value.databaseName);
            _notificationServices = notificationServices;
            _likeCommentRetweet = database.GetCollection<CommentLikeRetweet>(db.Value.likeCommentRetweetCollectionName);
        }
        
        //create or delete like
        public async Task CreateOrDeleteLike(string tweetId,string receiverUserName, string userName)
        {
            var collection =await  _likeCommentRetweet.Find(x => x.tweetId == tweetId).FirstOrDefaultAsync();
            
            
            if (collection == null)
            {
                var notification = new NotificationDto
                {
                    tweetId = tweetId,
                    receiverUserName = receiverUserName,
                    senderUserName = userName,
                    message = userName+" liked your tweet",
                };

                _likeCommentRetweet.InsertOne(new CommentLikeRetweet
                {
                    tweetId = tweetId,
                    likes = new List<string> { userName }
                });
                if (notification.receiverUserName != notification.senderUserName)
                {
                    await _notificationServices.CreateNotification(notification);
                }
                
            }
            else if (collection.likes.Contains(userName))
            {
                collection.likes.Remove(userName);
                await _likeCommentRetweet.ReplaceOneAsync(x => x.tweetId == tweetId, collection);
            }
            else
            {
                var notification = new NotificationDto
                {
                    tweetId = tweetId,
                    receiverUserName = receiverUserName,
                    senderUserName = userName,
                    message = userName + " liked your tweet",
                };
                if (notification.receiverUserName != notification.senderUserName)
                {
                    await _notificationServices.CreateNotification(notification);
                }
                collection.likes.Add(userName);
                await  _likeCommentRetweet.ReplaceOneAsync(x => x.tweetId == tweetId, collection);
            }
        }
        //retweet or delete retweet
        public async Task CreateOrDeleteReTweet(string tweetId, string receiverUserName, RetweetDto retweet)
        {
            var collection =await _likeCommentRetweet.Find(x => x.tweetId == tweetId).FirstOrDefaultAsync();
            if (collection == null)
            {
                var notification = new NotificationDto
                {
                    tweetId = tweetId,
                    receiverUserName = receiverUserName,
                    senderUserName = retweet.userName,
                    message = retweet.userName + " retweeted your tweet",
                };
                if (notification.receiverUserName != notification.senderUserName)
                {
                    await _notificationServices.CreateNotification(notification);
                }
                _likeCommentRetweet.InsertOne(new CommentLikeRetweet
                {
                    tweetId = tweetId,
                    retweets = new List<RetweetDto> { retweet }
                });
            }
            else if (collection.retweets.Any(x => x.userName == retweet.userName))
            {
                collection.retweets.Remove(collection.retweets.FirstOrDefault(x => x.userName == retweet.userName));
                await _likeCommentRetweet.ReplaceOneAsync(x => x.tweetId == tweetId, collection);
            }
            else
            {
                collection.retweets.Add(retweet);
                await _likeCommentRetweet.ReplaceOneAsync(x => x.tweetId == tweetId, collection);
                var notification = new NotificationDto
                {
                    tweetId = tweetId,
                    receiverUserName = receiverUserName,
                    senderUserName = retweet.userName,
                    message = retweet.userName + " retweeted your tweet",
                };
                if (notification.receiverUserName != notification.senderUserName)
                {
                    await _notificationServices.CreateNotification(notification);
                }
            }
           
        }

        //create comment
        public async Task CreateComment(string tweetId,string receiverUserName, CommentDto comment)
        {
            var collection = await _likeCommentRetweet.Find(x => x.tweetId == tweetId).FirstOrDefaultAsync();
            if (collection == null)
            {
                var notification = new NotificationDto
                {
                    tweetId = tweetId,
                    receiverUserName = receiverUserName,
                    senderUserName = comment.userName,
                    message = comment.userName + " commented your tweet",
                };
                if (notification.receiverUserName != notification.senderUserName)
                {
                    await _notificationServices.CreateNotification(notification);
                }
                _likeCommentRetweet.InsertOne(new CommentLikeRetweet
                {
                    tweetId = tweetId,
                    comments = new List<CommentDto> { comment }
                });
            }
            else
            {
                var notification = new NotificationDto
                {
                    tweetId = tweetId,
                    receiverUserName = receiverUserName,
                    senderUserName = comment.userName,
                    message = comment.userName + " commented your tweet",
                };
                if (notification.receiverUserName != notification.senderUserName)
                {
                    await _notificationServices.CreateNotification(notification);
                }
                collection.comments.Add(comment);
                await _likeCommentRetweet.ReplaceOneAsync(x => x.tweetId == tweetId, collection);

            }
            
        }
        //delete comment
        public async Task DeleteComment(string tweetId, string commentId , string userName)
        {
            var collection = await _likeCommentRetweet.Find(x => x.tweetId == tweetId).FirstOrDefaultAsync();
            var comment = collection.comments.Find(x => x.commentId == commentId && x.userName==userName );
            collection.comments.Remove(comment);
            await _likeCommentRetweet.ReplaceOneAsync(x => x.tweetId == tweetId, collection);
        }
        //get all 
        public async Task<CommentLikeRetweet> getAll(string tweetId)
        {
            return await _likeCommentRetweet.Find(x => x.tweetId == tweetId).FirstOrDefaultAsync();
        }

        public async Task Delete(string tweetId)
        {
            await _likeCommentRetweet.DeleteOneAsync(x=>x.tweetId==tweetId);
        }
    }
}
