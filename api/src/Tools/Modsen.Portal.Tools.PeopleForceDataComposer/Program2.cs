//using System.Collections.Concurrent;
//using System.Text.Encodings.Web;
//using System.Text.Json;
//using System.Text.Unicode;

//namespace Modsen.Portal.Tools.PeopleForceDataComposer
//{
//    internal class Program2
//    {
//        private const int MaxDegreeOfParallelism = 4;

//        private const string RootDirectory = "C:\\pf-backup\\";
//        private const string ComposedEmployeesFileName = "employees_full.json";
//        private const string ComposedCandidatesFileName = "candidates_full.json";
//        private const string ComposedVacanciesFileName = "vacancies_full.json";

//        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
//        {
//            WriteIndented = true,
//            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
//        };

//        private static readonly string[] EmployeeArrayFields = [
//            "leave_balances",
//            "leave_types",
//            "positions",
//            "employment_statuses"
//            ];

//        private static readonly string[] CandidateArrayFields = [
//            "notes",
//            "educations",
//            "experiences",
//            ];

//        public static async Task Main(string[] args)
//        {
//            var cancellationToken = CancellationToken.None;

//            try
//            {
//                await ComposeObjects("employees", ComposeEmployee, ComposedEmployeesFileName, cancellationToken);
//                await ComposeObjects("candidates", ComposeCandidate, ComposedCandidatesFileName, cancellationToken);
//                await ComposeObjects("vacancies", ComposeVacancy, ComposedVacanciesFileName, cancellationToken);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex);
//            }

//            Console.ReadKey();
//        }
//        private static async Task ComposeObjects(
//            string objectsName,
//            Func<JsonElement, CancellationToken, Task<string?>> composeFunction,
//            string composedFileName,
//            CancellationToken cancellationToken)
//        {
//            var candidatesJson = await File.ReadAllTextAsync(Path.Combine(RootDirectory, objectsName + ".json"), cancellationToken);
//            using var candidatesDoc = JsonDocument.Parse(candidatesJson);
//            var candidatesArray = candidatesDoc.RootElement.EnumerateArray().ToArray();

//            var updatedCandidates = new ConcurrentBag<JsonElement>();
//            using var semaphoreSlim = new SemaphoreSlim(MaxDegreeOfParallelism);

//            var tasks = candidatesArray.Select(
//                async candidate =>
//                {
//                    await semaphoreSlim.WaitAsync();
//                    try
//                    {
//                        var candidateString = await composeFunction.Invoke(candidate, cancellationToken);
//                        if (candidateString != null)
//                        {
//                            updatedCandidates.Add(JsonSerializer.Deserialize<JsonElement>(candidateString));
//                        }
//                    }
//                    finally
//                    {
//                        semaphoreSlim.Release();
//                    }
//                }).ToList();

//            await Task.WhenAll(tasks);

//            var resultJson = JsonSerializer.Serialize(updatedCandidates, _jsonSerializerOptions);
//            var resultFilePath = Path.Combine(RootDirectory, composedFileName);
//            await File.WriteAllTextAsync(resultFilePath, resultJson, cancellationToken);

//            Console.WriteLine($"{updatedCandidates.Count}/{candidatesArray.Length} {objectsName} are composed and saved to file: {resultFilePath}");
//        }

//        private static async Task<string?> ComposeEmployee(
//            JsonElement employee,
//            CancellationToken cancellationToken)
//        {
//            var updatedEmployee = new Dictionary<string, object>();

//            if (!employee.TryGetProperty("id", out var idString) ||
//                !idString.TryGetInt32(out var employeeId))
//            {
//                //Console.WriteLine($"Failed parse employee id");
//                return null;
//            }

//            var jsonDocuments = new List<JsonDocument>();

//            try
//            {
//                foreach (var property in employee.EnumerateObject())
//                {
//                    updatedEmployee[property.Name] = property.Value.Clone();
//                }

//                // get fields from "api/public/v2/employees/{employee_id}"
//                var employeeFilePath = Path.Combine(RootDirectory, $"employee_{employeeId}.json");

//                if (File.Exists(employeeFilePath))
//                {
//                    var employeeJson = await File.ReadAllTextAsync(employeeFilePath, cancellationToken);
//                    var employeeJsonDocument = JsonDocument.Parse(employeeJson);
//                    jsonDocuments.Add(employeeJsonDocument);
//                    var employeeJsonElement = employeeJsonDocument.RootElement;

//                    if (employeeJsonElement.TryGetProperty("data", out var data))
//                    {
//                        if (data.TryGetProperty("direct_reports", out var directReports))
//                        {
//                            updatedEmployee["direct_reports"] = directReports;
//                            //Console.WriteLine($"Employee {employeeId}: direct_reports field is created");
//                        }

//                        if (data.TryGetProperty("custom_fields", out var customFields))
//                        {
//                            updatedEmployee["custom_fields"] = customFields;
//                            //Console.WriteLine($"Employee {employeeId}: custom_fields field is created");
//                        }
//                    }
//                }

//                // get fields from other endpoints
//                foreach (var fieldName in EmployeeArrayFields)
//                {
//                    var filePath = Path.Combine(RootDirectory, $"employee_{employeeId}_{fieldName}.json");

//                    if (File.Exists(filePath))
//                    {
//                        var json = await File.ReadAllTextAsync(filePath, cancellationToken);
//                        var jsonDocument = JsonDocument.Parse(json);
//                        jsonDocuments.Add(jsonDocument);
//                        var jsonArray = jsonDocument.RootElement.EnumerateArray();

//                        var newJsonElements = new List<JsonElement>();
//                        foreach (var jsonElement in jsonArray)
//                        {
//                            newJsonElements.Add(jsonElement.Clone());
//                            //Console.WriteLine($"Employee {employeeId}: {fieldName} field is created");
//                        }

//                        updatedEmployee[fieldName] = newJsonElements;
//                    }
//                }

//                //Console.WriteLine($"Employee {employeeId} was updated");
//                return JsonSerializer.Serialize(updatedEmployee);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Failed compose employee with id: {employeeId}: {ex.Message}");
//            }
//            finally
//            {
//                foreach (var document in jsonDocuments)
//                {
//                    document?.Dispose();
//                }
//            }

//            return null;
//        }

//        private static async Task<string?> ComposeCandidate(
//            JsonElement candidate,
//            CancellationToken cancellationToken)
//        {
//            var updatedCandidate = new Dictionary<string, object>();

//            if (!candidate.TryGetProperty("id", out var idString) ||
//                !idString.TryGetInt32(out var candidateId))
//            {
//                //Console.WriteLine($"Failed parse candidate id");
//                return null;
//            }

//            var jsonDocuments = new List<JsonDocument>();

//            try
//            {
//                foreach (var property in candidate.EnumerateObject())
//                {
//                    updatedCandidate[property.Name] = property.Value.Clone();
//                }

//                var detailsFilePath = Path.Combine(RootDirectory, $"candidate_details_{candidateId}.json");

//                if (File.Exists(detailsFilePath))
//                {
//                    var candidateJson = await File.ReadAllTextAsync(detailsFilePath, cancellationToken);
//                    var candidateJsonDocument = JsonDocument.Parse(candidateJson);
//                    jsonDocuments.Add(candidateJsonDocument);
//                    var candidateJsonElement = candidateJsonDocument.RootElement;

//                    if (candidateJsonElement.TryGetProperty("data", out var data))
//                    {
//                        if (data.TryGetProperty("custom_fields", out var customFields))
//                        {
//                            updatedCandidate["custom_fields"] = customFields;
//                            //Console.WriteLine($"Candidate {candidateId}: custom_fields field is created");
//                        }
//                        if (data.TryGetProperty("resume_url", out var resumeUrl))
//                        {
//                            updatedCandidate["resume_url"] = resumeUrl;
//                            //Console.WriteLine($"Candidate {candidateId}: resume_url field is created");
//                        }
//                    }
//                }

//                foreach (var fieldName in CandidateArrayFields)
//                {
//                    var filePath = Path.Combine(RootDirectory, $"candidate_{candidateId}_{fieldName}.json");

//                    if (File.Exists(filePath))
//                    {
//                        var json = await File.ReadAllTextAsync(filePath, cancellationToken);
//                        var jsonDocument = JsonDocument.Parse(json);
//                        jsonDocuments.Add(jsonDocument);
//                        var jsonArray = jsonDocument.RootElement.EnumerateArray();

//                        var newJsonElements = new List<JsonElement>();
//                        foreach (var jsonElement in jsonArray)
//                        {
//                            newJsonElements.Add(jsonElement.Clone());
//                            //Console.WriteLine($"Employee {candidateId}: {fieldName} field is created");
//                        }

//                        updatedCandidate[fieldName] = newJsonElements;
//                    }
//                }

//                //Console.WriteLine($"Employee {candidateId} was updated");
//                return JsonSerializer.Serialize(updatedCandidate);
//            }
//            catch (Exception ex)
//            {
//                //Console.WriteLine($"Failed compose candidate with id: {candidateId}: {ex.Message}");
//            }
//            finally
//            {
//                foreach (var document in jsonDocuments)
//                {
//                    document?.Dispose();
//                }
//            }

//            return null;
//        }

//        private static async Task<string?> ComposeVacancy(
//            JsonElement vacancy,
//            CancellationToken cancellationToken)
//        {
//            var updatedVacancy = new Dictionary<string, object>();

//            if (!vacancy.TryGetProperty("id", out var idString) ||
//                !idString.TryGetInt32(out var vacancyId))
//            {
//                //Console.WriteLine($"Failed parse vacancy id");
//                return null;
//            }

//            var jsonDocuments = new List<JsonDocument>();

//            try
//            {
//                foreach (var property in vacancy.EnumerateObject())
//                {
//                    updatedVacancy[property.Name] = property.Value.Clone();
//                }

//                var pipelineFilePath = Path.Combine(RootDirectory, $"vacancy_{vacancyId}_pipeline.json");

//                if (File.Exists(pipelineFilePath))
//                {
//                    var pipelineJson = await File.ReadAllTextAsync(pipelineFilePath, cancellationToken);
//                    var pipelineJsonDocument = JsonDocument.Parse(pipelineJson);
//                    jsonDocuments.Add(pipelineJsonDocument);
//                    var pipelineJsonElement = pipelineJsonDocument.RootElement;

//                    updatedVacancy["pipeline"] = pipelineJsonElement;
//                    //Console.WriteLine($"Vacancy {vacancyId}: pipeline field is created");
//                }


//                var filePath = Path.Combine(RootDirectory, $"vacancy_{vacancyId}_applications.json");

//                if (File.Exists(filePath))
//                {
//                    var json = await File.ReadAllTextAsync(filePath, cancellationToken);
//                    var jsonDocument = JsonDocument.Parse(json);
//                    jsonDocuments.Add(jsonDocument);
//                    var jsonArray = jsonDocument.RootElement.EnumerateArray();

//                    var newJsonElements = new List<JsonElement>();
//                    foreach (var jsonElement in jsonArray)
//                    {
//                        newJsonElements.Add(jsonElement.Clone());
//                        //Console.WriteLine($"Employee {candidateId}: applications field is created");
//                    }

//                    updatedVacancy["applications"] = newJsonElements;
//                }

//                //Console.WriteLine($"Employee {candidateId} was updated");
//                return JsonSerializer.Serialize(updatedVacancy);
//            }
//            catch (Exception ex)
//            {
//                //Console.WriteLine($"Failed compose candidate with id: {candidateId}: {ex.Message}");
//            }
//            finally
//            {
//                foreach (var document in jsonDocuments)
//                {
//                    document?.Dispose();
//                }
//            }

//            return null;
//        }
//    }
//}
