namespace Gu.Roslyn.Asserts.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    /// <summary>A minimalistic git client implementation based on launching an external process.</summary>
    internal class GitClient
    {
        private readonly string gitPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="GitClient"/> class.
        /// Creates a GitClient given an absolute path to the git executable.
        /// </summary>
        /// <param name="path">Path to the git executable.</param>
        internal GitClient(string path)
        {
            this.gitPath = path ?? throw new ArgumentNullException(nameof(path));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GitClient"/> class.
        /// Creates a GitClient by finding it on a best-effort basis.
        /// </summary>
        internal GitClient()
            : this("git")
        {
        }

        /// <summary>
        /// For specifying the kind of <see cref="Clone"/>.
        /// </summary>
        [Flags]
        internal enum CloneFlags
        {
            None = 0,
            Recursive = 1 << 0,
            Shallow = 1 << 1,
            Bare = 1 << 2,
        }

        /// <summary>
        /// Clone a repository.
        /// </summary>
        /// <param name="uri">Address of a repository.</param>
        /// <param name="cloneTargetPath">Path to target directory where to clone the repository. Directory must be empty.</param>
        /// <param name="flags">Flags.</param>
        /// <param name="branch">Check out to the specific branch.</param>
        /// <param name="config">Configuration values of the cloned repository.</param>
        internal void Clone(
            Uri uri,
            string cloneTargetPath,
            CloneFlags flags = CloneFlags.None,
            string? branch = null,
            IReadOnlyDictionary<string, string>? config = null)
        {
            var parameters = new List<string>
                             {
                                 "clone",
                             };
            if (flags.HasFlag(CloneFlags.Recursive))
            {
                parameters.Add("--recursive");
            }

            if (flags.HasFlag(CloneFlags.Shallow))
            {
                parameters.Add("--depth");
                parameters.Add("1");
                parameters.Add("--shallow-submodules");
            }

            if (flags.HasFlag(CloneFlags.Bare))
            {
                parameters.Add("--bare");
            }

            if (branch != null)
            {
                parameters.Add("-b");
                parameters.Add(branch);
            }

            if (config != null)
            {
                foreach (var kvp in config)
                {
                    parameters.Add("-c");
                    parameters.Add($"{kvp.Key}={kvp.Value}");
                }
            }

            parameters.Add(uri.AbsoluteUri);
            parameters.Add(cloneTargetPath);
            var psi = new ProcessStartInfo(this.gitPath, CommandLineFromArgv(parameters))
            {
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };
            using var p = Process.Start(psi) ??
                          throw new InvalidOperationException($"Process.Start({this.gitPath}) returned null.");
            p.WaitForExit();
            var output = p.StandardError.ReadToEnd();
            if (p.ExitCode != 0)
            {
                throw new InvalidOperationException(output);
            }
        }

        /// <summary>
        /// Does the inverse transformation as specified in
        /// https://msdn.microsoft.com/en-us/library/ms880421.
        /// </summary>
        /// <param name="args">Argument list.</param>
        /// <returns>A single commandline string usable with ProcessStartInfo.Arguments.</returns>
        private static string CommandLineFromArgv(IEnumerable<string> args)
        {
            return string.Join(" ", args.Select(Escape));

            // adapted from https://blogs.msdn.microsoft.com/twistylittlepassagesallalike/2011/04/23/everyone-quotes-command-line-arguments-the-wrong-way/
            static string Escape(string argument)
            {
                var commandLine = new StringBuilder();

                if (string.IsNullOrEmpty(argument) ||
                    argument.IndexOfAny(new[] { ' ', '\t', '\n', '\v', '\"' }) == -1)
                {
                    commandLine.Append(argument);
                }
                else
                {
                    commandLine.Append('\"');
                    for (var i = 0; ; ++i)
                    {
                        var numberOfBackslashes = 0;
                        while (i != argument.Length && argument[i] == '\\')
                        {
                            ++i;
                            ++numberOfBackslashes;
                        }

                        if (i == argument.Length)
                        {
                            commandLine.Append('\\', numberOfBackslashes * 2);
                            break;
                        }
                        else if (argument[i] == '\"')
                        {
                            commandLine.Append('\\', (numberOfBackslashes * 2) + 1);
                            commandLine.Append(argument[i]);
                        }
                        else
                        {
                            commandLine.Append('\\', numberOfBackslashes);
                            commandLine.Append(argument[i]);
                        }
                    }

                    commandLine.Append('"');
                }

                return commandLine.ToString();
            }
        }
    }
}
