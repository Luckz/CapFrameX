﻿using CapFrameX.Contracts.Configuration;
using CapFrameX.Data;
using CapFrameX.Extensions.NetStandard;
using CapFrameX.Statistics.NetStandard;
using CapFrameX.Statistics.NetStandard.Contracts;
using CapFrameX.Statistics.PlotBuilder;
using CapFrameX.Statistics.PlotBuilder.Contracts;
using OxyPlot;
using OxyPlot.Axes;
using Prism.Commands;
using Prism.Events;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace CapFrameX.ViewModel.DataContext
{
    public class FrametimeGraphDataContext : GraphDataContextBase
    {
        public ICommand CopyFrametimeValuesCommand { get; }

        public ICommand CopyFrametimePointsCommand { get; }

        public ICommand SavePlotAsSVG { get; }

        public ICommand SavePlotAsPNG { get; }

        public PlotModel FrametimeModel => PlotModel;

        private readonly FrametimePlotBuilder _frametimePlotBuilder;

        public FrametimeGraphDataContext(IRecordDataServer recordDataServer, 
                                         IAppConfiguration appConfiguration, 
                                         IStatisticProvider frametimesStatisticProvider, 
                                         IEventAggregator eventAggregator) :
            base(appConfiguration, recordDataServer, frametimesStatisticProvider, eventAggregator)
        {
            CopyFrametimeValuesCommand = new DelegateCommand(OnCopyFrametimeValues);
            CopyFrametimePointsCommand = new DelegateCommand(OnCopyFrametimePoints);
            SavePlotAsSVG = new DelegateCommand(() => OnSavePlotAsImage("frametimes", "svg"));
            SavePlotAsPNG = new DelegateCommand(() => OnSavePlotAsImage("frametimes", "png"));
            _frametimePlotBuilder = new FrametimePlotBuilder(appConfiguration, frametimesStatisticProvider);
        }

        public void BuildPlotmodel(IPlotSettings plotSettings, Action<PlotModel> onFinishAction = null)
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                _frametimePlotBuilder.BuildPlotmodel(RecordDataServer.CurrentSession, plotSettings, RecordDataServer.CurrentTime, RecordDataServer.CurrentTime + RecordDataServer.WindowLength, RecordDataServer.RemoveOutlierMethod, onFinishAction);
                var plotModel = _frametimePlotBuilder.PlotModel;
                plotModel.TextColor = AppConfiguration.UseDarkMode ? OxyColors.White : OxyColors.Black;
                plotModel.PlotAreaBorderColor = AppConfiguration.UseDarkMode ? OxyColor.FromArgb(100, 204, 204, 204) : OxyColor.FromArgb(50, 30, 30, 30);

                var xAxis = plotModel.GetAxisOrDefault(EPlotAxis.XAXIS.GetDescription(), null);
                if (xAxis != null)
                    xAxis.MajorGridlineColor = AppConfiguration.UseDarkMode ? OxyColor.FromArgb(40, 204, 204, 204) : OxyColor.FromArgb(20, 30, 30, 30);

                var yAxis = plotModel.GetAxisOrDefault(EPlotAxis.YAXISFRAMETIMES.GetDescription(), null);
                if (yAxis != null)
                    yAxis.MajorGridlineColor = AppConfiguration.UseDarkMode ? OxyColor.FromArgb(40, 204, 204, 204) : OxyColor.FromArgb(20, 30, 30, 30);

                PlotModel = plotModel;
            });
        }

        public void Reset()
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                _frametimePlotBuilder.Reset();
                var plotModel = _frametimePlotBuilder.PlotModel;
                plotModel.TextColor = AppConfiguration.UseDarkMode ? OxyColors.White : OxyColors.Black;
                plotModel.PlotAreaBorderColor = AppConfiguration.UseDarkMode ? OxyColor.FromArgb(64, 204, 204, 204) : OxyColor.FromArgb(30, 30, 30, 30);
                PlotModel = plotModel;
            });
        }

        public void UpdateAxis(EPlotAxis axis, Action<Axis> action)
        {
            _frametimePlotBuilder.UpdateAxis(axis, action);
        }

        private void OnCopyFrametimeValues()
        {
            if (RecordSession == null)
                return;

            var frametimes = RecordDataServer.GetFrametimeTimeWindow();
            StringBuilder builder = new StringBuilder();

            foreach (var frametime in frametimes)
            {
                builder.Append(Math.Round(frametime, 2).ToString(CultureInfo.InvariantCulture) + Environment.NewLine);
            }

            Clipboard.SetDataObject(builder.ToString(), false);
        }

        private void OnCopyFrametimePoints()
        {
            if (RecordSession == null)
                return;

            var frametimePoints = RecordDataServer.GetFrametimePointTimeWindow();
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < frametimePoints.Count; i++)
            {
                builder.Append(Math.Round(frametimePoints[i].X, 2).ToString(CultureInfo.InvariantCulture) + "\t" +
                    Math.Round(frametimePoints[i].Y, 2).ToString(CultureInfo.InvariantCulture) + Environment.NewLine);
            }

            Clipboard.SetDataObject(builder.ToString(), false);
        }
    }
}
