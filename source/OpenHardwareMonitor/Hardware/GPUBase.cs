﻿using CapFrameX.Contracts.RTSS;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;

namespace OpenHardwareMonitor.Hardware
{
    internal abstract class GPUBase : Hardware
    {
        protected const float SCALE = 1024 * 1024 * 1024;

        protected Sensor memoryUsageDedicated;
        protected Sensor memoryUsageShared;
        protected Sensor processMemoryUsageDedicated;
        protected Sensor processMemoryUsageShared;

        protected readonly PerformanceCounter dedicatedVramUsagePerformCounter;
        protected readonly PerformanceCounter sharedVramUsagePerformCounter;
        protected PerformanceCounter dedicatedVramUsageProcessPerformCounter;
        protected PerformanceCounter sharedVramUsageProcessPerformCounter;

        public GPUBase(string name, Identifier identifier, ISettings settings, IRTSSService rTSSService) : base(name, identifier, settings)
        {
            try
            {
                if (PerformanceCounterCategory.Exists("GPU Adapter Memory"))
                {
                    var category = new PerformanceCounterCategory("GPU Adapter Memory");
                    var instances = category.GetInstanceNames().Where(inst => inst != string.Empty).ToArray();

                    if (instances.Any())
                    {
                        float maxValue = 0;
                        int maxIndex = 0;
                        for (int i = 0; i < instances.Length; i++)
                        {
                            try
                            {
                                var currentPerfCounter = new PerformanceCounter("GPU Adapter Memory", "Dedicated Usage", instances[i]);

                                if (currentPerfCounter.NextValue() > maxValue)
                                {
                                    maxValue = currentPerfCounter.NextValue();
                                    maxIndex = i;
                                }

                            }
                            catch (Exception ex)
                            {
                                Log.Logger.Error(ex, $"Error while creating performance counter with instance {i}.");
                            }
                        }

                        dedicatedVramUsagePerformCounter = new PerformanceCounter("GPU Adapter Memory", "Dedicated Usage", instances[maxIndex]);
                        sharedVramUsagePerformCounter = new PerformanceCounter("GPU Adapter Memory", "Shared Usage", instances[maxIndex]);
                    }
                    else
                    {
                        Log.Logger.Error("Error while creating GPU memory performance counter. No instances found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error while creating GPU memory performance counter.");
            }

            try
            {
                if (PerformanceCounterCategory.Exists("GPU Process Memory"))
                {
                    var category = new PerformanceCounterCategory("GPU Process Memory");

                    _ = rTSSService
                    .ProcessIdStream
                    .DistinctUntilChanged()
                    .Subscribe(id =>
                    {
                        if (id == 0)
                        {
                            dedicatedVramUsageProcessPerformCounter = null;
                            sharedVramUsageProcessPerformCounter = null;
                            return;
                        }

                        string idString = $"pid_{id}_luid";

                        var instances = category.GetInstanceNames();
                        if (instances != null && instances.Any())
                        {
                            var pid = instances.FirstOrDefault(instance => instance.Contains(idString));

                            if (pid != null)
                            {
                                dedicatedVramUsageProcessPerformCounter = new PerformanceCounter("GPU Process Memory", "Dedicated Usage", pid);
                                sharedVramUsageProcessPerformCounter = new PerformanceCounter("GPU Process Memory", "Shared Usage", pid);
                            }
                            else
                            {
                                dedicatedVramUsageProcessPerformCounter = null;
                                sharedVramUsageProcessPerformCounter = null;
                            }
                        }
                        else
                        {
                            dedicatedVramUsageProcessPerformCounter = null;
                            sharedVramUsageProcessPerformCounter = null;
                            Log.Logger.Error("Error while creating GPU process memory performance counter. No instances found.");
                        }

                    });
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error while creating GPU process memory performance counter.");
            }
        }
    }
}