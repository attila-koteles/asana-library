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
        const int WorkspaceId = 0;
        const int UserToAssign = 0;
        const int TagId = 0;

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
            var taskId = HelperCreateTestTask();
            Assert.True(taskId > 1_000_000);
        }

        [Fact]
        public void TestAddTagToTaskAsync()
        {
            var taskId = HelperCreateTestTask();
            asanaService.AddTagToTaskAsync(taskId, TagId).Wait();
        }

        [Fact]
        public void TestAddCommentToTaskAsync()
        {
            var taskId = HelperCreateTestTask();
            var commentId = asanaService.AddCommentToTaskAsync(taskId, "This is a test comment").Result;
            Assert.True(commentId > 1_000_000);
        }

        private long HelperCreateTestTask()
        {
            return asanaService.CreateTaskAsync(WorkspaceId, UserToAssign, "Unit test task", "Some notes").Result;
        }

    }
}
