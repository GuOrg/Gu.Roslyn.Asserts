using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Gu.Roslyn.Asserts.Tests, PublicKey=002400000480000094000000060200000024000052534131000400000100010091bce5bdf04b3bc012bdd4e8ca9ef4088bcddf2971a85ed4db4c6f4d5f5c3233f79a688a548e29545c580b503d16fe9717ca01b0e12ef70253691f84790eb470178112c7c87253efb169f0954b571bedeb1d6be756c64cc7a9d0baacb1ec97021ebe51622e998706decec5494b499738d8c492a8c808faad9d05f96d8e02c98d")]

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
            string branch = null,
            IReadOnlyDictionary<string, string> config = null)
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
            using (var p = Process.Start(psi))
            {
                p.WaitForExit();
                var output = p.StandardError.ReadToEnd();
                if (p.ExitCode != 0)
                {
                    throw new InvalidOperationException(output);
                }
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
                bool force = false;

                if (!force && argument == string.Empty && argument.IndexOfAny(new[] { ' ', '\t', '\n', '\v', '\"' }) == -1)
                {
                    commandLine.Append(argument);
                }
                else
                {
                    commandLine.Append('\"');
                    for (int i = 0; ; ++i)
                    {
                        int numberOfBackslashes = 0;
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
