﻿using System;

namespace Infinity.Models
{
    /// <summary>
    /// A reviewer on a pull request.
    /// </summary>
    public class PullRequestReviewer : TeamMember
    {
        /// <summary>
        /// The value of the vote: 1 for thumbs up, 0 for neutral (or not
        /// voted); -1 for thumbs down
        /// </summary>
        public int Vote { get; private set; }

        /// <summary>
        /// The URL of the reviewer.
        /// </summary>
        public Uri ReviewerUrl { get; private set; }
    }
}
