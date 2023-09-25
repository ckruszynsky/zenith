﻿using System.ComponentModel;

namespace Zenith.Core.Domain.Entities
{
    public enum ActivityType
    {
        [Description("User has been created")]
        UserCreated,
        [Description("User has been retrieved")]
        UserRetrieved,
        [Description("User has been updated")]
        UserUpdated,
        [Description("User has logged in")]
        Login,
        [Description("User has created an article")]
        ArticleCreate,
        [Description("User has deleted an article")]
        ArticleDelete,
        [Description("User has favorited an article")]
        UserFavorite,
        [Description("User has followed another user")]
        UserFollow
    }
}
