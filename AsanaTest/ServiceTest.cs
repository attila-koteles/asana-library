using Asana;
using Asana.Mapping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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
        const string ProjectId = "FILL-THIS-OUT";

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
        public async Task TestTypeAheadSearch()
        {
            TypeAheadResult[] results;

            results = await asanaService.TypeAheadSearch(WorkspaceId, "user", "Attila Köteles");
            Assert.Single(results);
            Assert.Equal("Attila Köteles", results[0].Name);
            Assert.Equal("255545193380", results[0].Gid);

            results = await asanaService.TypeAheadSearch(WorkspaceId, "tag", "Canal Digital");
            Assert.Single(results);
            Assert.Equal("canal digital", results[0].Name);
            Assert.Equal("11335945644051", results[0].Gid);

            results = await asanaService.TypeAheadSearch(WorkspaceId, "tag", "Siketfajd");
            Assert.Empty(results);
        }

        [Fact]
        public void TestCreateComplexTaskAsync()
        {
            var request = new
            {
                data = new
                {
                    workspace = WorkspaceId,
                    assignee = UserToAssign,
                    name = "Complex task test",
                    notes = "These are task notes",
                    due_on = "2020-07-26",
                    projects = new[] { "11238705598188" },
                    tags = new[] { "8368374567647" }
                }
            };

            var taskGid = asanaService.CreateTaskAsync(request).Result;
            Assert.NotEmpty(taskGid);
        }

        [Fact]
        public void TestAddAttachmentAsync()
        {
            var taskGid = HelperCreateTestTask();
            var bytes = File.ReadAllBytes(@"tplogo.jpg");
            asanaService.AddAttachmentToTaskAsync(taskGid, bytes, "tplogo_uploaded.jpg", "image/jpeg").Wait();
        }

        [Fact]
        public void TestAddProjectToTaskAsync()
        {
            var taskGid = HelperCreateTestTask();
            asanaService.AddProjectToTask(taskGid, ProjectId).Wait();
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
