using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TaskManagement.API.Requests;
using TaskManagement.Application.Queries;
using TaskManagement.Domain.Entities;

namespace TaskManagment.Tests;

/// <summary>
/// <c>TasksControllerTests</c> contiene pruebas de integración para los
/// endpoints del TasksController. Las pruebas se basan en las pruebas manuales
/// definidas en TaskManagement.http.
/// </summary>
[TestFixture]
public class TasksControllerTests
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _httpClient = null!;
    private readonly string _bearerToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImZtYWNoYWRvIiwic3ViIjoiZm1hY2hhZG8iLCJqdGkiOiIzNDlkMGE0ZSIsIm5hbWVpZCI6IjExMTExMTExLTExMTEtMTExMS0xMTExLTExMTExMTExMTExMSIsIm5hbWUiOiJkZXYtdXNlciIsImF1ZCI6WyJodHRwczovL2xvY2FsaG9zdDo1MDAxIiwiaHR0cDovL2xvY2FsaG9zdDo1MDAwIl0sIm5iZiI6MTc3OTgyNTgwMCwiZXhwIjoxNzg3Nzc0NjAwLCJpYXQiOjE3Nzk4MjU4MDAsImlzcyI6ImRvdG5ldC11c2VyLWp3dHMifQ._nxUZlzDMQkpYzvJ6Tsa5wka8XQ6H_bD-Twg04bY7qM";

    [SetUp]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                // Disable HTTPS redirect for testing
                builder.UseSetting("https_port", "");
            });
        _httpClient = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient.Dispose();
        _factory.Dispose();
    }

    #region Create Task Tests

    /// <summary>
    /// Prueba crear una tarea exitosamente. Valida el endpoint "Create Task" en
    /// forma análoga al archivo .http. En este escenario el usuario crea una
    /// nueva tarea con datos válidos. El resultado esperado es que la tarea se
    /// crea y retorna 201 Created con el ID de la tarea.
    /// </summary>
    [Test]
    public async Task CreateTask_WithValidData_ReturnsCreatedStatusAndTaskId()
    {
        // Arrange
        var createRequest = new CreateTaskRequest(
            title: "Finish Clean Architecture demo",
            description: "Wire up in-memory SQLite and verify endpoint tests",
            priority: TaskPriority.High,
            dueDate: new DateTime(2027, 5, 20, 18, 0, 0, DateTimeKind.Utc)
        );

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/tasks")
        {
            Content = JsonContent.Create(createRequest),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken) }
        };

        // Act
        var response = await _httpClient.SendAsync(request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Expected 201 Created status");

        var taskId = await response.Content.ReadAsAsync<Guid>();
        Assert.That(taskId, Is.Not.EqualTo(Guid.Empty), "Expected a valid task ID");

        // Verify the task was persisted by retrieving it
        var getResponse = await _httpClient.GetAsync($"/api/tasks/{taskId}");
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Task should exist after creation");

        var retrievedTask = await getResponse.Content.ReadAsAsync<TaskDto>();
        Assert.That(retrievedTask.Title, Is.EqualTo(createRequest.Title), "Task title should match");
        Assert.That(retrievedTask.Description, Is.EqualTo(createRequest.Description), "Task description should match");
        Assert.That(retrievedTask.Priority, Is.EqualTo(createRequest.Priority.ToString()), "Task priority should match");
    }

    /// <summary>
    /// Prueba crear una tarea sin autenticación. Valida el requisito de
    /// autorización para la creación de tareas. En este escenario el usuario
    /// intenta crear una tarea sin estar autenticado, es decir, sin un token
    /// bearer. El resultado esperado es que retorne 401 Unauthorized.
    /// </summary>
    [Test]
    public async Task CreateTask_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var createRequest = new CreateTaskRequest(
            title: "Test Task",
            description: "Test Description",
            priority: TaskPriority.Medium,
            dueDate: DateTime.UtcNow.AddDays(7)
        );

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/tasks")
        {
            Content = JsonContent.Create(createRequest)
        };

        // Act
        var response = await _httpClient.SendAsync(request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized), "Expected 401 Unauthorized when no bearer token");
    }

    /// <summary>
    /// Prueba crear una tarea sin título. Valida la validación de campos
    /// requeridos. En este escenario el usuario crea una tarea sin proporcionar
    /// un título. El resultado esperado es que retorne 400 BadRequest con error
    /// de validación.
    /// </summary>
    [Test]
    public async Task CreateTask_WithMissingTitle_ReturnsBadRequest()
    {
        // Arrange
        var createRequest = new CreateTaskRequest(
            title: "",
            description: "Valid Description",
            priority: TaskPriority.High,
            dueDate: DateTime.UtcNow.AddDays(7)
        );

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/tasks")
        {
            Content = JsonContent.Create(createRequest),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken) }
        };

        // Act
        var response = await _httpClient.SendAsync(request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), "Expected 400 BadRequest for missing title");
    }

    /// <summary>
    /// Prueba crear una tarea con diferentes prioridades. Valida la creación de
    /// tareas con diferentes niveles de prioridad. En este escenario: Se crean
    /// tareas con prioridades Baja, Media y Alta. El resultado esperado es que
    /// todas las tareas se crean exitosamente con la prioridad correcta.
    /// </summary>
    [Test]
    [TestCase(TaskPriority.Low)]
    [TestCase(TaskPriority.Medium)]
    [TestCase(TaskPriority.High)]
    public async Task CreateTask_WithDifferentPriorities_CreatesTasksSuccessfully(TaskPriority priority)
    {
        // Arrange
        var createRequest = new CreateTaskRequest(
            title: $"Task with {priority} priority",
            description: "Test task for priority validation",
            priority: priority,
            dueDate: DateTime.UtcNow.AddDays(7)
        );

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/tasks")
        {
            Content = JsonContent.Create(createRequest),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken) }
        };

        // Act
        var response = await _httpClient.SendAsync(request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), $"Expected 201 Created for {priority} priority");

        var taskId = await response.Content.ReadAsAsync<Guid>();
        var getResponse = await _httpClient.GetAsync($"/api/tasks/{taskId}");
        var task = await getResponse.Content.ReadAsAsync<TaskDto>();

        Assert.That(task.Priority, Is.EqualTo(priority.ToString()), $"Task priority should be {priority}");
    }

    #endregion

    #region Get Task By Id Tests

    /// <summary>
    /// Prueba obtener una tarea por ID exitosamente. Valida el endpoint "Get
    /// Task By Id" en forma análoga al archivo .http. En este escenario el
    /// usuario recupera una tarea usando su ID. El resultado esperado es que
    /// retorne 200 OK con los detalles de la tarea.
    /// </summary>
    [Test]
    public async Task GetTaskById_WithValidId_ReturnsOkAndTaskDetails()
    {
        // Arrange - Create a task first
        var createRequest = new CreateTaskRequest(
            title: "Task to Retrieve",
            description: "This task will be retrieved",
            priority: TaskPriority.Medium,
            dueDate: new DateTime(2027, 6, 15, 10, 0, 0, DateTimeKind.Utc)
        );

        var createHttpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/tasks")
        {
            Content = JsonContent.Create(createRequest),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken) }
        };

        var createResponse = await _httpClient.SendAsync(createHttpRequest);
        var taskId = await createResponse.Content.ReadAsAsync<Guid>();

        // Act
        var getResponse = await _httpClient.GetAsync($"/api/tasks/{taskId}");

        // Assert
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Expected 200 OK when task exists");

        var task = await getResponse.Content.ReadAsAsync<TaskDto>();
        Assert.That(task.Id, Is.EqualTo(taskId.ToString()).IgnoreCase, "Task ID should match");
        Assert.That(task.Title, Is.EqualTo(createRequest.Title), "Task title should match");
        Assert.That(task.Status, Is.EqualTo("Todo"), "New task should have Todo status");
        Assert.That(task.CreatedBy, Is.Not.Empty, "Task should have a creator");
    }

    /// <summary>
    /// Prueba obtener una tarea con ID inexistente. Valida el manejo de errores
    /// para tareas que no existen. En este escenario el usuario intenta
    /// recuperar una tarea con un ID que no existe. El resultado esperado es
    /// que retorne 404 NotFound.
    /// </summary>
    [Test]
    public async Task GetTaskById_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _httpClient.GetAsync($"/api/tasks/{nonExistentId}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "Expected 404 NotFound for non-existent task");
    }

    /// <summary>
    /// Prueba obtener tarea con GUID inválido. Valida la validación de entrada
    /// para el ID de la tarea. En este escenario el usuario proporciona un
    /// formato GUID inválido. El resultado esperado es que retorne 400
    /// BadRequest o error de enrutamiento.
    /// </summary>
    [Test]
    public async Task GetTaskById_WithInvalidGuid_ReturnsBadRequestOrNotFound()
    {
        // Arrange
        var invalidId = "not-a-guid";

        // Act
        var response = await _httpClient.GetAsync($"/api/tasks/{invalidId}");

        // Assert
        Assert.That(
            response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.NotFound,
            "Expected 400 BadRequest or 404 NotFound for invalid GUID format"
        );
    }

    #endregion

    #region Complete Task Tests

    /// <summary>
    /// Prueba completar una tarea exitosamente. Valida el endpoint "Complete
    /// Task" en forma análoga al archivo .http. En este escenario el usuario
    /// marca una tarea como completada. El resultado esperado es que retorne
    /// 204 NoContent y el estado de la tarea se convierte en Completada.
    /// </summary>
    [Test]
    public async Task CompleteTask_WithValidId_ReturnsNoContentAndUpdatesStatus()
    {
        // Arrange - Create a task first
        var createRequest = new CreateTaskRequest(
            title: "Task to Complete",
            description: "This task will be completed",
            priority: TaskPriority.High,
            dueDate: DateTime.UtcNow.AddDays(3)
        );

        var createHttpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/tasks")
        {
            Content = JsonContent.Create(createRequest),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken) }
        };

        var createResponse = await _httpClient.SendAsync(createHttpRequest);
        var taskId = await createResponse.Content.ReadAsAsync<Guid>();

        // Act - Complete the task
        var completeRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/tasks/{taskId}/complete")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken) }
        };

        var completeResponse = await _httpClient.SendAsync(completeRequest);

        // Assert
        Assert.That(completeResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent), "Expected 204 NoContent when task completed");

        // Verify the task status was updated
        var getResponse = await _httpClient.GetAsync($"/api/tasks/{taskId}");
        var completedTask = await getResponse.Content.ReadAsAsync<TaskDto>();

        Assert.That(completedTask.Status, Is.EqualTo("Completed"), "Task status should be Completed");
        Assert.That(completedTask.Id, Is.EqualTo(taskId.ToString()).IgnoreCase, "Task ID should remain unchanged");
    }

    /// <summary>
    /// Prueba completar tarea sin autenticación. Valida el requisito de
    /// autorización para la finalización de tareas. En este escenario el
    /// usuario intenta completar una tarea sin autenticación. El resultado
    /// esperado es que retorne 401 Unauthorized.
    /// </summary>
    [Test]
    public async Task CompleteTask_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        // Act
        var response = await _httpClient.PostAsync($"/api/tasks/{taskId}/complete", null);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized), "Expected 401 Unauthorized when no bearer token");
    }

    /// <summary>
    /// Prueba completar tarea inexistente. Valida el manejo de errores al
    /// completar una tarea que no existe. En este escenario el usuario intenta
    /// completar una tarea que no existe. El resultado esperado es que retorne
    /// 400 BadRequest con un mensaje de error.
    /// </summary>
    [Test]
    public async Task CompleteTask_WithNonExistentId_ReturnsBadRequest()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/tasks/{nonExistentId}/complete")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken) }
        };

        // Act
        var response = await _httpClient.SendAsync(request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), "Expected 400 BadRequest for non-existent task");
    }

    /// <summary>
    /// Prueba completar tarea ya completada. Valida la regla de negocio: No se
    /// puede completar una tarea ya completada. En este escenario el usuario
    /// intenta completar una tarea que ya está completada. El resultado
    /// esperado es que retorne 400 BadRequest con mensaje de error "ya
    /// completada".
    /// </summary>
    [Test]
    public async Task CompleteTask_WhenAlreadyCompleted_ReturnsBadRequest()
    {
        // Arrange - Create and complete a task
        var createRequest = new CreateTaskRequest(
            title: "Already Completed Task",
            description: "This task is already completed",
            priority: TaskPriority.Low,
            dueDate: DateTime.UtcNow.AddDays(1)
        );

        var createHttpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/tasks")
        {
            Content = JsonContent.Create(createRequest),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken) }
        };

        var createResponse = await _httpClient.SendAsync(createHttpRequest);
        var taskId = await createResponse.Content.ReadAsAsync<Guid>();

        // Complete it once
        var completeRequest1 = new HttpRequestMessage(HttpMethod.Post, $"/api/tasks/{taskId}/complete")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken) }
        };
        await _httpClient.SendAsync(completeRequest1);

        // Act - Try to complete it again
        var completeRequest2 = new HttpRequestMessage(HttpMethod.Post, $"/api/tasks/{taskId}/complete")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken) }
        };

        var response = await _httpClient.SendAsync(completeRequest2);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), "Expected 400 BadRequest when completing already completed task");

        var jsonContent = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(jsonContent);
        var detail = jsonDoc.RootElement.GetProperty("detail").GetString();
        Assert.That(detail, Does.Contain("already completed"), "Error message should mention task is already completed");
    }

    #endregion

    #region Workflow Tests

    /// <summary>
    /// Prueba flujo de trabajo completo: Crear → Obtener → Completar → Verificar
    /// Valida el flujo de trabajo completo como se muestra en el archivo .http.
    /// En este escenario:
    /// <ol>
    ///   <li>Se crea una nueva tarea</li>
    ///   <li>Se recupera la tarea creada</li>
    ///   <li>Se completa la tarea</li>
    ///   <li>Se verifica que el estado fue actualizado</li>
    /// </ol>
    /// El resultado esperado es: Todas las operaciones tienen éxito y los datos son consistentes.
    /// </summary>
    [Test]
    public async Task CompleteWorkflow_CreateGetAndCompleteTask_SucceedsWithConsistentData()
    {
        // Arrange
        var createRequest = new CreateTaskRequest(
            title: "Finish Clean Architecture demo",
            description: "Wire up in-memory SQLite and verify endpoint tests",
            priority: TaskPriority.High,
            dueDate: new DateTime(2027, 5, 20, 18, 0, 0, DateTimeKind.Utc)
        );

        // Act 1 - Create Task
        var createHttpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/tasks")
        {
            Content = JsonContent.Create(createRequest),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken) }
        };

        var createResponse = await _httpClient.SendAsync(createHttpRequest);
        Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Task creation should succeed");

        var taskId = await createResponse.Content.ReadAsAsync<Guid>();

        // Act 2 - Get Task By Id
        var getResponse = await _httpClient.GetAsync($"/api/tasks/{taskId}");
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Task retrieval should succeed");

        var task = await getResponse.Content.ReadAsAsync<TaskDto>();
        Assert.That(task.Title, Is.EqualTo(createRequest.Title), "Retrieved task should have correct title");
        Assert.That(task.Status, Is.EqualTo("Todo"), "New task should have Todo status");

        // Act 3 - Complete Task
        var completeRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/tasks/{taskId}/complete")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken) }
        };

        var completeResponse = await _httpClient.SendAsync(completeRequest);
        Assert.That(completeResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent), "Task completion should succeed");

        // Act 4 - Verify Status Updated
        var verifyResponse = await _httpClient.GetAsync($"/api/tasks/{taskId}");
        var completedTask = await verifyResponse.Content.ReadAsAsync<TaskDto>();

        Assert.That(completedTask.Status, Is.EqualTo("Completed"), "Completed task should have Completed status");
        Assert.That(completedTask.Title, Is.EqualTo(task.Title), "Task title should remain unchanged");
        Assert.That(completedTask.Description, Is.EqualTo(task.Description), "Task description should remain unchanged");
    }

    #endregion

    #region Concurrent Operations Tests

    /// <summary>
    /// Prueba creación de múltiples tareas simultáneamente.
    /// Valida la creación de múltiples tareas de forma concurrente.
    /// En este escenario se crean varias tareas con datos diferentes en paralelo.
    /// El resultado esperado es que todas las tareas se crean independientemente con IDs únicos.
    /// </summary>
    [Test]
    public async Task CreateMultipleTasks_WithDifferentData_CreatesMultipleIndependentTasks()
    {
        // Arrange
        var taskData = new[]
        {
            new CreateTaskRequest("Task 1", "Description 1", TaskPriority.Low, null),
            new CreateTaskRequest("Task 2", "Description 2", TaskPriority.Medium, DateTime.UtcNow.AddDays(5)),
            new CreateTaskRequest("Task 3", "Description 3", TaskPriority.High, DateTime.UtcNow.AddDays(10))
        };

        // Act - Create all tasks concurrently
        var createTasks = taskData.Select(async taskRequest =>
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/tasks")
            {
                Content = JsonContent.Create(taskRequest),
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken) }
            };

            var response = await _httpClient.SendAsync(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "Each task creation should succeed");

            return await response.Content.ReadAsAsync<Guid>();
        }).ToList();

        var taskIds = new List<Guid>(await Task.WhenAll(createTasks));

        // Assert
        Assert.That(taskIds.Count, Is.EqualTo(3), "Should have created 3 tasks");
        Assert.That(taskIds.Distinct().Count(), Is.EqualTo(3), "All task IDs should be unique");

        // Verify each task exists with correct data
        for (int i = 0; i < taskIds.Count; i++)
        {
            var response = await _httpClient.GetAsync($"/api/tasks/{taskIds[i]}");
            var task = await response.Content.ReadAsAsync<TaskDto>();

            Assert.That(task.Title, Is.EqualTo(taskData[i].Title), $"Task {i} title should match");
            Assert.That(task.Description, Is.EqualTo(taskData[i].Description), $"Task {i} description should match");
        }
    }

    #endregion
}
