﻿using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using CapFrameX.Contracts.Configuration;
using CapFrameX.EventAggregation.Messages;
using CapFrameX.Extensions;
using CapFrameX.OcatInterface;
using CapFrameX.Statistics;
using GongSolutions.Wpf.DragDrop;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace CapFrameX.ViewModel
{
	public class ReportViewModel : BindableBase, INavigationAware, IDropTarget
	{
		private readonly IStatisticProvider _frametimeStatisticProvider;
		private readonly IEventAggregator _eventAggregator;
		private readonly IAppConfiguration _appConfiguration;

		private bool _useEventMessages;

		public ObservableCollection<ReportInfo> ReportInfoCollecion { get; }
			= new ObservableCollection<ReportInfo>();

		public ICommand CopyTableDataCommand { get; }

		public ReportViewModel(IStatisticProvider frametimeStatisticProvider,
							  IEventAggregator eventAggregator,
							  IAppConfiguration appConfiguration)
		{
			_frametimeStatisticProvider = frametimeStatisticProvider;
			_eventAggregator = eventAggregator;
			_appConfiguration = appConfiguration;

			CopyTableDataCommand = new DelegateCommand(OnCopyTableData);

			SubscribeToSelectRecord();
		}

		private void OnCopyTableData()
		{
			if (!ReportInfoCollecion.Any())
				return;

			StringBuilder builder = new StringBuilder();

			// Header
			var displayNameGame = ReflectionExtensions.GetPropertyDisplayName<ReportInfo>(x => x.Game);
			var displayNameDate = ReflectionExtensions.GetPropertyDisplayName<ReportInfo>(x => x.Date);
			var displayNameTime = ReflectionExtensions.GetPropertyDisplayName<ReportInfo>(x => x.Time);
			var displayNameNumberOfSamples = ReflectionExtensions.GetPropertyDisplayName<ReportInfo>(x => x.NumberOfSamples);
			var displayNameRecordTime = ReflectionExtensions.GetPropertyDisplayName<ReportInfo>(x => x.RecordTime);
			var displayNameCpu = ReflectionExtensions.GetPropertyDisplayName<ReportInfo>(x => x.Cpu);
			var displayNameGraphicCard = ReflectionExtensions.GetPropertyDisplayName<ReportInfo>(x => x.GraphicCard);
			var displayNameMaxFps = ReflectionExtensions.GetPropertyDisplayName<ReportInfo>(x => x.MaxFps);
			var displayNameAverageFps = ReflectionExtensions.GetPropertyDisplayName<ReportInfo>(x => x.AverageFps);
			var displayNameOnePercentQuantileFps = ReflectionExtensions.GetPropertyDisplayName<ReportInfo>(x => x.OnePercentQuantileFps);
			var displayNameZeroDotOnePercentQuantileFps = ReflectionExtensions.GetPropertyDisplayName<ReportInfo>(x => x.ZeroDotOnePercentQuantileFps);
			var displayNameMinFps = ReflectionExtensions.GetPropertyDisplayName<ReportInfo>(x => x.MinFps);
			var displayNameAdaptiveSTDFps = ReflectionExtensions.GetPropertyDisplayName<ReportInfo>(x => x.AdaptiveSTDFps);
			var displayNameCustomComment = ReflectionExtensions.GetPropertyDisplayName<ReportInfo>(x => x.CustomComment);

			builder.Append(displayNameGame + "\t" +
						   displayNameDate + "\t" +
						   displayNameTime + "\t" +
						   displayNameNumberOfSamples + "\t" +
						   displayNameRecordTime + "\t" +
						   displayNameCpu + "\t" +
						   displayNameGraphicCard + "\t" +
						   displayNameMaxFps + "\t" +
						   displayNameAverageFps + "\t" +
						   displayNameOnePercentQuantileFps + "\t" +
						   displayNameZeroDotOnePercentQuantileFps + "\t" +
						   displayNameMinFps + "\t" +
						   displayNameAdaptiveSTDFps + "\t" +
						   displayNameCustomComment +
						   Environment.NewLine);

			foreach (var reportInfo in ReportInfoCollecion)
			{
				builder.Append(reportInfo.Game + "\t" +
							   reportInfo.Date + "\t" +
							   reportInfo.Time + "\t" +
							   reportInfo.NumberOfSamples + "\t" +
							   reportInfo.RecordTime + "\t" +
							   reportInfo.Cpu + "\t" +
							   reportInfo.GraphicCard + "\t" +
							   reportInfo.MaxFps + "\t" +
							   reportInfo.AverageFps + "\t" +
							   reportInfo.OnePercentQuantileFps + "\t" +
							   reportInfo.ZeroDotOnePercentQuantileFps + "\t" +
							   reportInfo.MinFps + "\t" +
							   reportInfo.AdaptiveSTDFps + "\t" +
							   reportInfo.CustomComment +
							   Environment.NewLine);
			}

			Clipboard.SetDataObject(builder.ToString(), false);
		}

		private void SubscribeToSelectRecord()
		{
			_eventAggregator.GetEvent<PubSubEvent<ViewMessages.SelectSession>>()
							.Subscribe(msg =>
							{
								if (_useEventMessages)
								{
									ReportInfo reportInfo = GetReportInfoFromRecordInfo(msg.RecordInfo);
									AddReportRecord(reportInfo);
								}
							});
		}

		private ReportInfo GetReportInfoFromRecordInfo(OcatRecordInfo recordInfo)
		{
			var session = RecordManager.LoadData(recordInfo.FullPath);
			var roundingDigits = _appConfiguration.FpsValuesRoundingDigits;

			var fps = session.FrameTimes.Select(ft => 1000 / ft).ToList();
			var p99_quantile = Math.Round(_frametimeStatisticProvider.GetPQuantileSequence(fps, 0.99), roundingDigits);
			var p95_quantile = Math.Round(_frametimeStatisticProvider.GetPQuantileSequence(fps, 0.95), roundingDigits);
			var max = Math.Round(fps.Max(), roundingDigits);
			var average = Math.Round(session.FrameTimes.Count * 1000 / session.FrameTimes.Sum(), roundingDigits);
			var p1_quantile = Math.Round(_frametimeStatisticProvider.GetPQuantileSequence(fps, 0.01), roundingDigits);
			var p0dot1_quantile = Math.Round(_frametimeStatisticProvider.GetPQuantileSequence(fps, 0.001), roundingDigits);
			var min = Math.Round(fps.Min(), roundingDigits);
			var adaptiveStandardDeviation = Math.Round(_frametimeStatisticProvider
				.GetAdaptiveStandardDeviation(fps, _appConfiguration.MovingAverageWindowSize), roundingDigits);

			var reportInfo = new ReportInfo()
			{
				Game = recordInfo.GameName,
				Date = recordInfo.CreationDate,
				Time = recordInfo.CreationTime,
				NumberOfSamples = session.FrameTimes.Count,
				RecordTime = Math.Round(session.LastFrameTime, 2).ToString(CultureInfo.InvariantCulture),
				Cpu = session.ProcessorName == null ? "-" : session.ProcessorName.Trim(new Char[] { ' ', '"' }),
				GraphicCard = session.GraphicCardName == null ? "-" : session.GraphicCardName.Trim(new Char[] { ' ', '"' }),
				MaxFps = max,
				NinetyNinePercentQuantileFps = p99_quantile,
				NinetyFivePercentQuantileFps = p95_quantile,
				AverageFps = average,
				OnePercentQuantileFps = p1_quantile,
				ZeroDotOnePercentQuantileFps = p0dot1_quantile,
				MinFps = min,
				AdaptiveSTDFps = adaptiveStandardDeviation,
				CustomComment = session.Comment
			};

			return reportInfo;
		}

		private void AddReportRecord(ReportInfo reportInfo)
		{
			ReportInfoCollecion.Add(reportInfo);
		}

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return true;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
			_useEventMessages = false;
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			_useEventMessages = true;
		}

		void IDropTarget.DragOver(IDropInfo dropInfo)
		{
			if (dropInfo != null)
			{
				dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
				dropInfo.Effects = DragDropEffects.Move;
			}
		}

		void IDropTarget.Drop(IDropInfo dropInfo)
		{
			if (dropInfo != null)
			{
				if (dropInfo.VisualTarget is FrameworkElement frameworkElement)
				{
					if (frameworkElement.Name == "ReportDataGrid")
					{
						if (dropInfo.Data is OcatRecordInfo recordInfo)
						{
							ReportInfo reportInfo = GetReportInfoFromRecordInfo(recordInfo);
							AddReportRecord(reportInfo);
						}
					}
					else if (frameworkElement.Name == "RemoveRecordItemControl")
					{
						if (dropInfo.Data is ReportInfo reportInfo)
						{
							ReportInfoCollecion.Remove(reportInfo);
						}
					}
				}
			}
		}
	}
}