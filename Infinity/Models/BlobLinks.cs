﻿using System.Collections.Generic;

namespace Infinity.Models
{
    /// <summary>
    /// Links to other Git blob resources.
    /// </summary>
    public class BlobLinks
    {
        /// <summary>
        /// Link to the Git blob's REST API endpoint.
        /// </summary>
        public LinkUrl Self { get; private set; }

        /// <summary>
        /// Link to the repository's REST API endpoint.
        /// </summary>
        public LinkUrl Repository { get; private set; }
    }
}