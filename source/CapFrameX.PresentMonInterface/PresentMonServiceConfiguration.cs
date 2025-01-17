﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace CapFrameX.PresentMonInterface
{
    public class PresentMonServiceConfiguration
    {
        public string ProcessName { get; set; }

        public bool RedirectOutputStream { get; set; }

        /// <summary>
        /// verbose or simple
        /// </summary>
        public string OutputLevelofDetail { get; set; } = "track_debug";

        public bool CaptureAllProcesses { get; set; } = false;

        public string OutputFilename { get; set; }

        public List<string> ExcludeProcesses { get; set; }

        public string ConfigParameterToArguments()
        {
            var arguments = string.Empty;
            if (CaptureAllProcesses)
            {
                arguments += "-restart_as_admin";
                arguments += " ";
                arguments += "-stop_existing_session";
                arguments += " ";
                arguments += "-captureall";
                arguments += " ";
                arguments += "-multi_csv";
                arguments += " ";
                arguments += "-output_file";
                arguments += " ";
				arguments += "-qpc_time";
                arguments += " ";
                arguments += OutputFilename;

                if (!string.IsNullOrWhiteSpace(OutputLevelofDetail))
                {
                    arguments += " ";
                    arguments += "-" + OutputLevelofDetail;
                }

                if (ExcludeProcesses != null && ExcludeProcesses.Any())
                {
                    arguments += " ";
                    foreach (var process in ExcludeProcesses.Where(proc => !proc.Contains(" ")))
                    {
                        arguments += "-exclude";
                        arguments += " ";
                        arguments += process;
                    }
                }
            }
            else
            {
                if (RedirectOutputStream)
                {
                    arguments += "-restart_as_admin";
                    arguments += " ";
                    arguments += "-stop_existing_session";
					arguments += " ";
					arguments += "-output_stdout";
					arguments += " ";
					arguments += "-qpc_time";

                    if (!string.IsNullOrWhiteSpace(OutputLevelofDetail))
                    {
                        arguments += " ";
                        arguments += "-" + OutputLevelofDetail;
                    }

					if (ExcludeProcesses != null && ExcludeProcesses.Any())
					{
						foreach (var process in ExcludeProcesses.Where(proc => !proc.Contains(" ")))
						{
							arguments += " ";
							arguments += "-exclude";
							arguments += " ";
							arguments += process + ".exe";
						}
					}
				}
                else
                {
					if (string.IsNullOrWhiteSpace(ProcessName))
					{
						throw new ArgumentException("Process name must be set!");
					}

                    arguments += "-restart_as_admin";
                    arguments += " ";
                    arguments += "-stop_existing_session";
                    arguments += " ";
                    arguments += "-process_name";
                    arguments += " ";
                    arguments += ProcessName;
                    arguments += " ";
                    arguments += "-output_file";
                    arguments += " ";
                    arguments += OutputFilename;
					arguments += " ";
					arguments += "-qpc_time";

                    if (!string.IsNullOrWhiteSpace(OutputLevelofDetail))
                    {
                        arguments += " ";
                        arguments += "-" + OutputLevelofDetail;
                    }
                }
            }

            return arguments;
        }
    }
}
