﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using RestSharp;

using Infinity.Models;
using Infinity.Util;

namespace Infinity.Clients
{
    /// <summary>
    /// Client to access information about Git repositories managed in a
    /// TFS Project Collection.
    /// </summary>
    public class GitClient
    {
        internal GitClient(ITfsClientExecutor executor)
        {
            Executor = executor;
        }

        private ITfsClientExecutor Executor { get; set; }

        #region Repositories

        /// <summary>
        /// Get a list of all Git repositories managed in a TFS Project Collection.
        /// </summary>
        /// <returns>A list of repositories in the Project Collection</returns>
        public async Task<IEnumerable<Repository>> GetRepositories()
        {
            RepositoryList list = await Executor.Execute<RepositoryList>(new RestRequest("/_apis/git/repositories"));
            return list.Value;
        }

        /// <summary>
        /// Get a list of all Git repositories managed in a TFS Team Project.
        /// </summary>
        /// <param name="projectId">The ID of the Team Project to query</param>
        /// <returns>A list of repositories in the Project Collection</returns>
        public async Task<IEnumerable<Repository>> GetRepositories(Guid projectId)
        {
            Assert.NotNull(projectId, "projectId");

            RestRequest request = new RestRequest("/_apis/git/{ProjectId}/repositories");
            request.AddUrlSegment("ProjectId", projectId.ToString());

            RepositoryList list = await Executor.Execute<RepositoryList>(request);
            return list.Value;
        }

        /// <summary>
        /// Get the information about a Git repository by its ID.
        /// </summary>
        /// <param name="id">The ID of the Git repository</param>
        /// <returns>The Git repository</returns>
        public async Task<Repository> GetRepository(Guid id)
        {
            Assert.NotNull(id, "id");

            var request = new RestRequest("/_apis/git/repositories/{RepositoryId}");
            request.AddUrlSegment("RepositoryId", id.ToString());

            return await Executor.Execute<Repository>(request);
        }

        /// <summary>
        /// Get the information about a Git repository by its name.
        /// </summary>
        /// <param name="projectId">The ID of the Team Project that contains this repository</param>
        /// <param name="name">The name of the Git repository</param>
        /// <returns>The Git repository</returns>
        public async Task<Repository> GetRepository(Guid projectId, string name)
        {
            Assert.NotNull(projectId, "projectId");
            Assert.NotNull(name, "name");

            var request = new RestRequest("/_apis/git/{ProjectId}/repositories/{Name}");
            request.AddUrlSegment("ProjectId", projectId.ToString());
            request.AddUrlSegment("Name", name);

            return await Executor.Execute<Repository>(request);
        }

        /// <summary>
        /// Create a new Git repository inside a Team Project.
        /// </summary>
        /// <param name="projectId">The ID of the Team Project that will contain this repository</param>
        /// <param name="name">The name of the repository to create</param>
        /// <returns>The Git repository created</returns>
        /// <exception cref="Infinity.Exceptions.TfsConflictException">If a repository by the same name already exists</exception>
        public async Task<Repository> CreateRepository(Guid projectId, string name)
        {
            Assert.NotNull(projectId, "projectId");
            Assert.NotNull(name, "name");

            var request = new RestRequest("/_apis/git/repositories", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { name = name, project = new { id = projectId.ToString() } });
            return await Executor.Execute<Repository>(request);
        }

        /// <summary>
        /// Rename a Git repository.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository to rename</param>
        /// <param name="newName">The new name of the repository</param>
        /// <returns>The Git repository after update</returns>
        public async Task<Repository> RenameRepository(Guid repositoryId, string newName)
        {
            Assert.NotNull(repositoryId, "repositoryId");
            Assert.NotNull(newName, "newName");

            var request = new RestRequest("/_apis/git/repositories/{RepositoryId}", Method.PATCH);
            request.AddUrlSegment("RepositoryId", repositoryId.ToString());
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { name = newName });
            return await Executor.Execute<Repository>(request);
        }

        /// <summary>
        /// Delete the given Git repository.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository to delete</param>
        public async Task DeleteRepository(Guid repositoryId)
        {
            Assert.NotNull(repositoryId, "repositoryId");

            var request = new RestRequest("/_apis/git/repositories/{RepositoryId}", Method.DELETE);
            request.AddUrlSegment("RepositoryId", repositoryId.ToString());
            await Executor.Execute(request);
        }

        #endregion

        #region References

        /// <summary>
        /// Get the references (branches, tags, notes) in the given repository.
        /// </summary>
        /// <param name="repositoryId">The ID of the Git repository</param>
        /// <param name="filter">The string prefix to match, excepting the <code>refs/</code> prefix.  For example, <code>heads/</code> will return branches.</param>
        /// <returns>The list of references.</returns>
        public async Task<IEnumerable<Reference>> GetReferences(Guid repositoryId, string filter = null)
        {
            Assert.NotNull(repositoryId, "repositoryId");

            if (filter != null)
            {
                Assert.IsTrue(filter.StartsWith("refs/"), "filter.StartsWith(refs/)");
                filter = filter.Substring(4);
            }

            var request = new RestRequest("/_apis/git/repositories/{RepositoryId}/refs{Filter}", Method.GET);
            request.AddUrlSegment("RepositoryId", repositoryId.ToString());
            request.AddUrlSegment("Filter", filter != null ? filter : "");

            ReferenceList references = await Executor.Execute<ReferenceList>(request);
            return (references != null) ? references.Value : new List<Reference>();
        }

        #endregion

        private class RepositoryList
        {
            public int Count { get; set; }
            public List<Repository> Value { get; set; }
        }

        private class ReferenceList
        {
            public int Count { get; set; }
            public List<Reference> Value { get; set; }
        }
    }
}