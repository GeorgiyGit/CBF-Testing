var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CBF_Testing_Server>("cbf-testing-server");

builder.Build().Run();
