﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.Cli.Args;
using Volo.Abp.Cli.ProjectModification;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.Cli.Commands
{
    public class UpdateCommand : IConsoleCommand, ITransientDependency
    {
        public ILogger<UpdateCommand> Logger { get; set; }

        private readonly VoloPackagesVersionUpdater _packagesVersionUpdater;

        public UpdateCommand(VoloPackagesVersionUpdater packagesVersionUpdater)
        {
            _packagesVersionUpdater = packagesVersionUpdater;

            Logger = NullLogger<UpdateCommand>.Instance;
        }

        public async Task ExecuteAsync(CommandLineArgs commandLineArgs)
        {
            var includePreviews = commandLineArgs.Options.GetOrNull(Options.IncludePreviews.Short, Options.IncludePreviews.Long) != null;

            var solution = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.sln").FirstOrDefault();

            if (solution != null)
            {
                var solutionName = Path.GetFileName(solution).RemovePostFix(".sln");

                _packagesVersionUpdater.UpdateSolution(solution, includePreviews);

                Logger.LogInformation($"Volo packages are updated in {solutionName} solution.");
                return;
            }

            var project = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.csproj").FirstOrDefault();

            if (project != null)
            {
                var projectName = Path.GetFileName(project).RemovePostFix(".csproj");

                _packagesVersionUpdater.UpdateProject(project, includePreviews);

                Logger.LogInformation($"Volo packages are updated in {projectName} project.");
                return;
            }

            Logger.LogError("No solution or project found in this directory.");
        }

        public static class Options
        {
            public static class IncludePreviews
            {
                public const string Short = "p";
                public const string Long = "include-previews";
            }
        }
    }
}
