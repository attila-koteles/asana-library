using Asana;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AsanaTest
{
    public class ServiceTest
    {
        // You need to fill these variables with your values!
        const string PersonalAccessToken = "FILL-THIS-OUT";
        const string WorkspaceId = "FILL-THIS-OUT";
        const string UserToAssign = "FILL-THIS-OUT";
        const string TagId = "FILL-THIS-OUT";

        private static Service asanaService;

        public ServiceTest()
        {
            asanaService = new Service(PersonalAccessToken);
        }

        [Fact]
        public void TestGetUsersAsync()
        {
            var users = asanaService.GetUsersAsync().Result;
            Assert.True(users.Count > 0);
        }

        [Fact]
        public void TestCreateTaskAsync()
        {
            var taskGid = HelperCreateTestTask();
            Assert.NotEmpty(taskGid);
        }

        [Fact]
        public void TestAddTagToTaskAsync()
        {
            var taskGid = HelperCreateTestTask();
            asanaService.AddTagToTaskAsync(taskGid, TagId).Wait();
        }

        [Fact]
        public void TestAddCommentToTaskAsync()
        {
            var taskGid = HelperCreateTestTask();
            var commentGid = asanaService.AddCommentToTaskAsync(taskGid, "This is a test comment").Result;
            Assert.NotEmpty(commentGid);
        }

        private string HelperCreateTestTask()
        {
            return asanaService.CreateTaskAsync(WorkspaceId, UserToAssign, "Unit test task", "Some notes").Result;
        }

    }
}
