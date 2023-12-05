using System.Diagnostics;
using System.Net.Http.Json;
using API;
using Newtonsoft.Json;
using Serilog;

[TestClass]
public class UserApiTests
{
    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
    }
    private readonly string ApiBaseUrl = "http://localhost:5000";

    [TestMethod]
    public async Task GetUsers_ShouldReturnSuccessAndNonZeroCount()
    {
        using (var httpClient = new HttpClient())
        {
            var getUsersUrl = $"{ApiBaseUrl}/api/User";
            var response = await httpClient.GetAsync(getUsersUrl);

            Assert.IsTrue(response.IsSuccessStatusCode, $"Failed to get users. Status code: {response.StatusCode}");

            var responseContent = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<User[]>(responseContent);

            Assert.IsNotNull(users);
            Assert.IsTrue(users.Length > 0);

        }
    }

    [TestMethod]
    public async Task GetRandomUserById_ShouldReturnSuccessAndNonNullFields()
    {
        using (var httpClient = new HttpClient())
        {
            var userId = "1";

            var getUserByIdUrl = $"{ApiBaseUrl}/api/User/{userId}";
            var response = await httpClient.GetAsync(getUserByIdUrl);

            Assert.IsTrue(response.IsSuccessStatusCode, $"Failed to get user with ID {userId}. Status code: {response.StatusCode}");

            var responseContent = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(responseContent);

            Assert.IsNotNull(user);
            Assert.IsNotNull(user.Id);
            Assert.IsNotNull(user.Name);
            Assert.IsNotNull(user.Email);
        }
    }

    [TestMethod]
    public async Task DeleteUserById_ShouldReturnSuccessAnd404AfterDelete()
    {
        try
        {
            using (var httpClient = new HttpClient())
            {
                var userIdToDelete = "2";

                var deleteUserUrl = $"{ApiBaseUrl}/api/User/{userIdToDelete}";
                var deleteResponse = await httpClient.DeleteAsync(deleteUserUrl);

                Debug.WriteLine($"DELETE request to {deleteUserUrl} returned status code: {deleteResponse.StatusCode}");

                Assert.IsTrue(deleteResponse.IsSuccessStatusCode, $"Failed to delete user with ID {userIdToDelete}. Status code: {deleteResponse.StatusCode}");
                await Task.Delay(5000); 

                var getUserByIdUrl = $"{ApiBaseUrl}/api/User/{userIdToDelete}";
                var responseAfterDelete = await httpClient.GetAsync(getUserByIdUrl);

                Debug.WriteLine($"GET request to {getUserByIdUrl} returned status code: {responseAfterDelete.StatusCode}");

                Assert.AreEqual(System.Net.HttpStatusCode.OK, responseAfterDelete.StatusCode, $"User with ID {userIdToDelete} still exists after deletion.");

                if (!responseAfterDelete.IsSuccessStatusCode)
                {
                    var errorContent = await responseAfterDelete.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Error content: {errorContent}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception: {ex.ToString()}");
            throw; 
        }
    }

    [TestMethod]
    public async Task UpdateUserById_ShouldReturnSuccessAndUpdatedUser()
    {
        using (var httpClient = new HttpClient())
        {
            var userIdToUpdate = "1"; 

            var updatedUser = new User
            {
                Id = userIdToUpdate,
                Name = "Updated User",
                Email = "updated.user@example.com"
            };

            try
            {
                var updateUserResponse = await httpClient.PutAsJsonAsync($"{ApiBaseUrl}/api/User/create/{userIdToUpdate}", updatedUser);

                updateUserResponse.EnsureSuccessStatusCode();
                var updatedUserString = await updateUserResponse.Content.ReadAsStringAsync();
                var returnedUser = JsonConvert.DeserializeObject<User>(updatedUserString);

                Assert.IsNotNull(returnedUser);
                Assert.AreEqual(updatedUser.Id, returnedUser.Id);
                Assert.AreEqual(updatedUser.Name, returnedUser.Name);
                Assert.AreEqual(updatedUser.Email, returnedUser.Email);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HttpRequestException: {ex.Message}");
                Console.WriteLine($"InnerException: {ex.InnerException?.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }
    }
}

