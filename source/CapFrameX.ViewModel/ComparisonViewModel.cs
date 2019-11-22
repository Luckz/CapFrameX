﻿using CapFrameX.Contracts.Configuration;
using CapFrameX.Contracts.Data;
using CapFrameX.EventAggregation.Messages;
using CapFrameX.Extensions;
using CapFrameX.Data;
using CapFrameX.Statistics;
using GongSolutions.Wpf.DragDrop;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using OxyPlot;
using OxyPlot.Axes;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ComparisonCollection = System.Collections.ObjectModel
	.ObservableCollection<CapFrameX.ViewModel.ComparisonRecordInfoWrapper>;

namespace CapFrameX.ViewModel
{
	public partial class ComparisonViewModel : BindableBase, INavigationAware, IDropTarget
	{
		private static readonly int PART_LENGTH = 42;

		private readonly IStatisticProvider _frametimeStatisticProvider;
		private readonly IFrametimeAnalyzer _frametimeAnalyzer;
		private readonly IEventAggregator _eventAggregator;
		private readonly IAppConfiguration _appConfiguration;

		private PlotModel _comparisonModel;
		private SeriesCollection _comparisonRowChartSeriesCollection;
		private string[] _comparisonRowChartLabels;
		private SeriesCollection _comparisonLShapeCollection;
		private string _comparisonItemControlHeight = "300";
		private string _columnChartYAxisTitle = "FPS";
		private ComparisonColorManager _comparisonColorManager = new ComparisonColorManager();
		private bool _useEventMessages;
		private string _remainingRecordingTime;
		private double _cutLeftSliderMaximum;
		private double _cutRightSliderMaximum;
		private double _firstSeconds;
		private double _lastSeconds;
		private bool _isContextLegendActive = true;
		private double _maxRecordingTime;
		private bool _doUpdateCharts = true;
		private double _barChartHeight;
		private bool _hasComparisonItems;
		private TabItem _selectedChartItem;
		private bool _isSortModeAscending = false;
		private Func<double, string> _comparisonColumnChartFormatter;
		private bool _colorPickerVisibility;
		private EMetric _selectSecondaryMetric = EMetric.P1;
		private EComparisonContext _selectedComparisonContext = EComparisonContext.DateTime;
		private string _currentGameName;
		private bool _hasUniqueGameNames;
		private bool _useComparisonGrouping;
		private bool _isCuttingModeActive;

		public Array MetricItems => Enum.GetValues(typeof(EMetric))
										.Cast<EMetric>()
										.Where(metric => metric != EMetric.Average)
										.ToArray();

		public Array ComparisonContextItems => Enum.GetValues(typeof(EComparisonContext))
												   .Cast<EComparisonContext>()
												   .ToArray();

		public ISubject<Unit> ResetLShapeChart = new Subject<Unit>();

		public ComparisonColorManager ComparisonColorManager
			=> _comparisonColorManager;

		public EMetric SelectSecondaryMetric
		{
			get { return _selectSecondaryMetric; }
			set
			{
				_appConfiguration.SecondaryMetric = 
					value.ConvertToString();
				_selectSecondaryMetric = value;
				RaisePropertyChanged();
				OnSecondaryMetricChanged();
			}
		}

		public EComparisonContext SelectedComparisonContext
		{
			get { return _selectedComparisonContext; }
			set
			{
				_appConfiguration.ComparisonContext = 
					value.ConvertToString();
				_selectedComparisonContext = value;
				RaisePropertyChanged();
				OnComparisonContextChanged();
			}
		}

		public Func<double, string> ComparisonColumnChartFormatter
		{
			get { return _comparisonColumnChartFormatter; }
			set
			{
				_comparisonColumnChartFormatter = value;
				RaisePropertyChanged();
			}
		}

		public PlotModel ComparisonModel
		{
			get { return _comparisonModel; }
			set
			{
				_comparisonModel = value;
				RaisePropertyChanged();
			}
		}

		public SeriesCollection ComparisonRowChartSeriesCollection
		{
			get { return _comparisonRowChartSeriesCollection; }
			set
			{
				_comparisonRowChartSeriesCollection = value;
				RaisePropertyChanged();
			}
		}

		public SeriesCollection ComparisonLShapeCollection
		{
			get { return _comparisonLShapeCollection; }
			set
			{
				_comparisonLShapeCollection = value;
				RaisePropertyChanged();
			}
		}

		public string[] ComparisonRowChartLabels
		{
			get { return _comparisonRowChartLabels; }
			set
			{
				_comparisonRowChartLabels = value;
				RaisePropertyChanged();
			}
		}

		public string ComparisonItemControlHeight
		{
			get { return _comparisonItemControlHeight; }
			set
			{
				_comparisonItemControlHeight = value;
				RaisePropertyChanged();
			}
		}

		public string RemainingRecordingTime
		{
			get { return _remainingRecordingTime; }
			set
			{
				_remainingRecordingTime = value;
				RaisePropertyChanged();
			}
		}

		public double CutLeftSliderMaximum
		{
			get { return _cutLeftSliderMaximum; }
			set
			{
				_cutLeftSliderMaximum = value;
				RaisePropertyChanged();
			}
		}

		public double CutRightSliderMaximum
		{
			get { return _cutRightSliderMaximum; }
			set
			{
				_cutRightSliderMaximum = value;
				RaisePropertyChanged();
			}
		}

		public string ColumnChartYAxisTitle
		{
			get { return _columnChartYAxisTitle; }
			set
			{
				_columnChartYAxisTitle = value;
				RaisePropertyChanged();
			}
		}

		public double BarChartHeight
		{
			get { return _barChartHeight; }
			set
			{
				_barChartHeight = value;
				RaisePropertyChanged();
			}
		}

		public bool HasComparisonItems
		{
			get { return _hasComparisonItems; }
			set
			{
				_hasComparisonItems = value;
				RaisePropertyChanged();
			}
		}

		public bool ColorPickerVisibility
		{
			get { return _colorPickerVisibility; }
			set
			{
				_colorPickerVisibility = value;
				RaisePropertyChanged();
			}
		}

		public double FirstSeconds
		{
			get { return _firstSeconds; }
			set
			{
				_firstSeconds = value;
				RaisePropertyChanged();
				UpdateCharts();
				RemainingRecordingTime = Math.Round(_maxRecordingTime - LastSeconds - _firstSeconds, 2)
					.ToString(CultureInfo.InvariantCulture) + " s";
			}
		}

		public double LastSeconds
		{
			get { return _lastSeconds; }
			set
			{
				_lastSeconds = value;
				RaisePropertyChanged();
				UpdateCharts();
				RemainingRecordingTime = Math.Round(_maxRecordingTime - _lastSeconds - FirstSeconds, 2)
					.ToString(CultureInfo.InvariantCulture) + " s";
			}
		}

		public bool IsContextLegendActive
		{
			get { return _isContextLegendActive; }
			set
			{
				_isContextLegendActive = value;
				RaisePropertyChanged();
				OnShowContextLegendChanged();
			}
		}

		public TabItem SelectedChartItem
		{
			get { return _selectedChartItem; }
			set
			{
				_selectedChartItem = value;
				RaisePropertyChanged();
				RaisePropertyChanged(nameof(IsBarChartTabActive));
				OnChartItemChanged();
				UpdateCharts();
			}
		}

		public bool IsSortModeAscending
		{
			get { return _isSortModeAscending; }
			set
			{
				_isSortModeAscending = value;
				RaisePropertyChanged();
				OnSortModeChanged();
			}
		}

		public string CurrentGameName
		{
			get { return _currentGameName; }
			set
			{
				_currentGameName = value;
				RaisePropertyChanged();
			}
		}

		public bool HasUniqueGameNames
		{
			get { return _hasUniqueGameNames; }
			set
			{
				_hasUniqueGameNames = value;
				RaisePropertyChanged();
			}
		}

		public bool UseComparisonGrouping
		{
			get { return _useComparisonGrouping; }
			set
			{
				_useComparisonGrouping = value;
				RaisePropertyChanged();
				OnComparisonGroupingChanged();
			}
		}

		public bool IsCuttingModeActive
		{
			get { return _isCuttingModeActive; }
			set
			{
				_isCuttingModeActive = value;
				RaisePropertyChanged();
				OnCuttingModeChanged();
			}
		}

		public bool IsBarChartTabActive
		{
			get { return SelectedChartItem?.Header.ToString() == "Bar charts"; }
		}

		public ICommand RemoveAllComparisonsCommand { get; }

		public ComparisonCollection ComparisonRecords { get; private set; }
			= new ObservableCollection<ComparisonRecordInfoWrapper>();

		public double BarChartMaxRowHeight { get; private set; } = 20;

		public ComparisonViewModel(IStatisticProvider frametimeStatisticProvider,
			IFrametimeAnalyzer frametimeAnalyzer,
			IEventAggregator eventAggregator,
			IAppConfiguration appConfiguration)
		{
			_frametimeStatisticProvider = frametimeStatisticProvider;
			_frametimeAnalyzer = frametimeAnalyzer;
			_eventAggregator = eventAggregator;
			_appConfiguration = appConfiguration;

			RemoveAllComparisonsCommand = new DelegateCommand(OnRemoveAllComparisons);
			ComparisonLShapeCollection = new SeriesCollection();

			ComparisonColumnChartFormatter = value => value.ToString(string.Format("F{0}",
			_appConfiguration.FpsValuesRoundingDigits), CultureInfo.InvariantCulture);
			SelectedComparisonContext = _appConfiguration.ComparisonContext.ConverToEnum<EComparisonContext>();
			SelectSecondaryMetric = _appConfiguration.SecondaryMetric.ConverToEnum<EMetric>();

			SetRowSeries();
			InitializePlotModel();
			SubscribeToSelectRecord();
		}

		private void InitializePlotModel()
		{
			ComparisonModel = new PlotModel
			{
				PlotMargins = new OxyThickness(40, 10, 0, 40),
				PlotAreaBorderColor = OxyColor.FromArgb(64, 204, 204, 204),
				LegendPosition = LegendPosition.TopCenter,
				LegendOrientation = LegendOrientation.Horizontal
			};

			//Axes
			//X
			ComparisonModel.Axes.Add(new LinearAxis()
			{
				Key = "xAxis",
				Position = OxyPlot.Axes.AxisPosition.Bottom,
				Title = "Recording time [s]",
				MajorGridlineStyle = LineStyle.Solid,
				MajorGridlineThickness = 1,
				MajorGridlineColor = OxyColor.FromArgb(64, 204, 204, 204),
				MinorTickSize = 0,
				MajorTickSize = 0
			});

			//Y
			ComparisonModel.Axes.Add(new LinearAxis()
			{
				Key = "yAxis",
				Position = OxyPlot.Axes.AxisPosition.Left,
				Title = "Frametime [ms]",
				MajorGridlineStyle = LineStyle.Solid,
				MajorGridlineThickness = 1,
				MajorGridlineColor = OxyColor.FromArgb(64, 204, 204, 204),
				MinorTickSize = 0,
				MajorTickSize = 0
			});
		}

		private void SubscribeToSelectRecord()
		{
			_eventAggregator.GetEvent<PubSubEvent<ViewMessages.SelectSession>>()
							.Subscribe(msg =>
							{
								if (_useEventMessages)
								{
									AddComparisonItem(msg.RecordInfo);
								}
							});
		}

		private void OnChartItemChanged()
			=> ColorPickerVisibility = SelectedChartItem.Header.ToString() != "Bar charts";

		private void OnSortModeChanged()
			=> SortComparisonItems();

		private void OnComparisonGroupingChanged()
			=> SortComparisonItems();

		private void SetRowSeries()
		{
			ComparisonRowChartSeriesCollection = new SeriesCollection()
			{
				new RowSeries
				{
					Title = "Average",
					Values = new ChartValues<double>(),
					Fill = new SolidColorBrush(Color.FromRgb(34, 151, 243)),
					HighlightFill = new SolidColorBrush(Color.FromRgb(122, 192, 247)),
					DataLabels = true,
					MaxRowHeigth = BarChartMaxRowHeight,
					UseRelativeMode = true
				}
			};

			if (SelectSecondaryMetric != EMetric.None)
			{
				// second metric
				ComparisonRowChartSeriesCollection.Add(
				new RowSeries
				{
					Title = SelectSecondaryMetric.GetDescription(),
					Values = new ChartValues<double>(),
					Fill = new SolidColorBrush(Color.FromRgb(241, 125, 32)),
					HighlightFill = new SolidColorBrush(Color.FromRgb(245, 164, 98)),
					DataLabels = true,
					MaxRowHeigth = BarChartMaxRowHeight,
					UseRelativeMode = true
				});
			}
		}

		private void OnSecondaryMetricChanged()
		{
			if (SelectSecondaryMetric == EMetric.None)
			{
				ComparisonRowChartSeriesCollection.RemoveAt(1);
				// Cannot adjust height here, otherwise the labels will not fit
				//BarChartHeight = 40 + (BarChartMaxRowHeight + 12) * ComparisonRecords.Count;
			}
			else
			{
				double GeMetricValue(IList<double> sequence, EMetric metric) =>
					_frametimeStatisticProvider.GetFpsMetricValue(sequence, metric);

				if (ComparisonRowChartSeriesCollection.Count < 2)
				{
					ComparisonRowChartSeriesCollection.Add(
						new RowSeries
						{
							Values = new ChartValues<double>(),
							// 241, 125, 32 (orange)
							Fill = new SolidColorBrush(Color.FromRgb(241, 125, 32)),
							HighlightFill = new SolidColorBrush(Color.FromRgb(245, 164, 98)),
							DataLabels = true,
							MaxRowHeigth = BarChartMaxRowHeight,
							UseRelativeMode = true
						});
				}

				ComparisonRowChartSeriesCollection[1].Values.Clear();

				for (int i = 0; i < ComparisonRecords.Count; i++)
				{
					var currentWrappedComparisonInfo = ComparisonRecords[i];

					double startTime = FirstSeconds;
					double endTime = _maxRecordingTime - LastSeconds;
					var frametimeTimeWindow = currentWrappedComparisonInfo.WrappedRecordInfo.Session
						.GetFrametimeTimeWindow(startTime, endTime, ERemoveOutlierMethod.None);

					var secondaryMetric = GeMetricValue(frametimeTimeWindow, SelectSecondaryMetric);
					(ComparisonRowChartSeriesCollection[1] as RowSeries).Title = SelectSecondaryMetric.GetDescription();
					ComparisonRowChartSeriesCollection[1].Values.Insert(0, secondaryMetric);
				}
			}
		}

		private void OnComparisonContextChanged()
		{
			switch (SelectedComparisonContext)
			{
				case EComparisonContext.DateTime:
					OnDateTimeContext();
					break;
				case EComparisonContext.CPU:
					OnCpuContext();
					break;
				case EComparisonContext.GPU:
					OnGpuContex();
					break;
				case EComparisonContext.SystemRam:
					OnSystemRamContex();
					break;
				case EComparisonContext.Custom:
					OnCustomContex();
					break;
				default:
					OnDateTimeContext();
					break;
			}
		}

		private void OnCuttingModeChanged()
		{
			UpdateCuttingParameter();
			UpdateCharts();
		}

		private void UpdateCuttingParameter()
		{
			if (ComparisonRecords == null || !ComparisonRecords.Any())
				return;

			double minRecordingTime = double.MaxValue;
			_maxRecordingTime = double.MinValue;

			foreach (var record in ComparisonRecords)
			{
				if (record.WrappedRecordInfo.Session.FrameStart.Last() > _maxRecordingTime)
					_maxRecordingTime = record.WrappedRecordInfo.Session.FrameStart.Last();

				if (record.WrappedRecordInfo.Session.FrameStart.Last() < minRecordingTime)
					minRecordingTime = record.WrappedRecordInfo.Session.FrameStart.Last();
			}

			_doUpdateCharts = false;
			FirstSeconds = 0;
			LastSeconds = 0;
			_doUpdateCharts = true;

			CutLeftSliderMaximum = minRecordingTime / 2 - 0.5;
			CutRightSliderMaximum = minRecordingTime / 2 + _maxRecordingTime - minRecordingTime - 0.5;
			RemainingRecordingTime = ComparisonRecords.Any() ?
				Math.Round(_maxRecordingTime, 2).ToString(CultureInfo.InvariantCulture) + " s" : "0.0 s"; ;
		}

		private void UpdateAxesMinMax(bool invalidatePlot)
		{
			if (ComparisonRecords == null || !ComparisonRecords.Any())
				return;

			var xAxis = ComparisonModel.GetAxisOrDefault("xAxis", null);
			var yAxis = ComparisonModel.GetAxisOrDefault("yAxis", null);

			if (xAxis == null || yAxis == null)
				return;

			double xMin = 0;
			double xMax = 0;
			double yMin = 0;
			double yMax = 0;

			double startTime = FirstSeconds;
			double endTime = _maxRecordingTime - LastSeconds;

			var sessionParallelQuery = ComparisonRecords.Select(record => record.WrappedRecordInfo.Session).AsParallel();

			xMin = sessionParallelQuery.Min(session =>
			{
				return session.GetFrametimePointsTimeWindow(startTime, endTime).First().X;
			});

			xMax = sessionParallelQuery.Max(session =>
			{
				return session.GetFrametimePointsTimeWindow(startTime, endTime).Last().X;
			});

			yMin = sessionParallelQuery.Min(session =>
			{
				return session.GetFrametimePointsTimeWindow(startTime, endTime).Min(pnt => pnt.Y); ;
			});

			yMax = sessionParallelQuery.Max(session =>
			{
				return session.GetFrametimePointsTimeWindow(startTime, endTime).Max(pnt => pnt.Y);
			});

			xAxis.Minimum = xMin;
			xAxis.Maximum = xMax;

			yAxis.Minimum = yMin - (yMax - yMin) / 6;
			yAxis.Maximum = yMax + (yMax - yMin) / 6;

			if (invalidatePlot)
				ComparisonModel.InvalidatePlot(true);
		}

		private void UpdateBarChartHeight()
			=> BarChartHeight = 60 + (2 * BarChartMaxRowHeight + 12) * ComparisonRecords.Count;

		private void OnRemoveAllComparisons()
			=> RemoveAllComparisonItems(true, true);

		private void ResetBarChartSeriesTitles()
		{
			for (int i = 0; i < ComparisonModel.Series.Count; i++)
			{
				ComparisonModel.Series[i].Title = string.Empty;
			}

			ComparisonRowChartLabels = Array.Empty<string>();
		}

		private ComparisonRecordInfo GetComparisonRecordInfoFromFileRecordInfo(IFileRecordInfo fileRecordInfo)
		{
			string infoText = string.Empty;
			var session = RecordManager.LoadData(fileRecordInfo.FullPath);

			if (session != null)
			{
				var newLine = Environment.NewLine;
				infoText += $"{fileRecordInfo.CreationDate} { fileRecordInfo.CreationTime}" + newLine +
							$"{session.FrameTimes.Count} frames in {Math.Round(session.LastFrameTime, 2).ToString(CultureInfo.InvariantCulture)}s";
			}

			return new ComparisonRecordInfo
			{
				Game = fileRecordInfo.GameName,
				InfoText = infoText,
				DateTime = fileRecordInfo.FileInfo.LastWriteTime.ToString(),
				Session = session,
				FileRecordInfo = fileRecordInfo
			};
		}

		private void UpdateCharts()
		{
			if (!_doUpdateCharts)
				return;

			ResetBarChartSeriesTitles();
			ComparisonModel.Series.Clear();
			ComparisonLShapeCollection.Clear();

			if (SelectedChartItem.Header.ToString() == "Bar charts")
				SetColumnChart();
			else
			{
				SetFrametimeChart();
				SetLShapeChart();
			}
		}

		private void AddToColumnCharts(ComparisonRecordInfoWrapper wrappedComparisonInfo)
		{
			// Update metrics
			SetMetrics(wrappedComparisonInfo);

			// First metric
			ComparisonRowChartSeriesCollection[0].Values.Insert(0, wrappedComparisonInfo.WrappedRecordInfo.FirstMetric);

			// Second metric
			if (ComparisonRowChartSeriesCollection.Count > 1)
				ComparisonRowChartSeriesCollection[1].Values.Insert(0, wrappedComparisonInfo.WrappedRecordInfo.SecondMetric);

			switch (SelectedComparisonContext)
			{
				case EComparisonContext.DateTime:
					SetLabelDateTimeContext();
					break;
				case EComparisonContext.CPU:
					SetLabelCpuContext();
					break;
				case EComparisonContext.GPU:
					SetLabelGpuContext();
					break;
				case EComparisonContext.Custom:
					SetLabelCustomContext();
					break;
				default:
					SetLabelDateTimeContext();
					break;
			}
		}

		private void AddToFrameTimeChart(ComparisonRecordInfoWrapper wrappedComparisonInfo)
		{
			double startTime = FirstSeconds;
			double endTime = _maxRecordingTime - LastSeconds;
			var session = wrappedComparisonInfo.WrappedRecordInfo.Session;
			var frametimePoints = session.GetFrametimePointsTimeWindow(startTime, endTime)
										 .Select(pnt => new Point(pnt.X, pnt.Y));

			var chartTitle = string.Empty;

			switch (SelectedComparisonContext)
			{
				case EComparisonContext.DateTime:
					chartTitle = $"{wrappedComparisonInfo.WrappedRecordInfo.FileRecordInfo.CreationDate} " +
						$"{ wrappedComparisonInfo.WrappedRecordInfo.FileRecordInfo.CreationTime}";
					break;
				case EComparisonContext.CPU:
					chartTitle = wrappedComparisonInfo.WrappedRecordInfo.FileRecordInfo.ProcessorName;
					break;
				case EComparisonContext.GPU:
					chartTitle = wrappedComparisonInfo.WrappedRecordInfo.FileRecordInfo.GraphicCardName;
					break;
				case EComparisonContext.SystemRam:
					chartTitle = wrappedComparisonInfo.WrappedRecordInfo.FileRecordInfo.SystemRamInfo;
					break;
				case EComparisonContext.Custom:
					chartTitle = wrappedComparisonInfo.WrappedRecordInfo.FileRecordInfo.Comment;
					break;
				default:
					chartTitle = $"{wrappedComparisonInfo.WrappedRecordInfo.FileRecordInfo.CreationDate} " +
						$"{ wrappedComparisonInfo.WrappedRecordInfo.FileRecordInfo.CreationTime}";
					break;
			}

			var color = wrappedComparisonInfo.FrametimeGraphColor.Value;
			var frametimeSeries = new OxyPlot.Series.LineSeries
			{
				Title = chartTitle,
				StrokeThickness = 1,
				LegendStrokeThickness = 4,
				Color = wrappedComparisonInfo.IsHideModeSelected ?
					OxyColors.Transparent : OxyColor.FromRgb(color.R, color.G, color.B)
			};

			frametimeSeries.Points.AddRange(frametimePoints.Select(pnt => new DataPoint(pnt.X, pnt.Y)));
			ComparisonModel.Series.Add(frametimeSeries);
		}

		private void AddToLShapeChart(ComparisonRecordInfoWrapper wrappedComparisonInfo)
		{
			double startTime = FirstSeconds;
			double endTime = _maxRecordingTime - LastSeconds;
			var frametimeTimeWindow = wrappedComparisonInfo.WrappedRecordInfo.Session.GetFrametimeTimeWindow(startTime, endTime, ERemoveOutlierMethod.None);

			var lShapeQuantiles = _frametimeAnalyzer.GetLShapeQuantiles();
			double action(double q) => _frametimeStatisticProvider.GetPQuantileSequence(frametimeTimeWindow, q / 100);
			var quantiles = lShapeQuantiles.Select(q => new ObservablePoint(q, action(q)));
			var quantileValues = new ChartValues<ObservablePoint>();
			quantileValues.AddRange(quantiles);

			ComparisonLShapeCollection.Add(
			new LineSeries
			{
				Values = quantileValues,
				Stroke = wrappedComparisonInfo.IsHideModeSelected ? Brushes.Transparent : wrappedComparisonInfo.Color,
				Fill = Brushes.Transparent,
				StrokeThickness = 1,
				LineSmoothness = 1,
				PointGeometrySize = 5,
				PointGeometry = DefaultGeometries.Square,
				LabelPoint = chartPoint => string.Format(CultureInfo.InvariantCulture, "{0:0.##}", chartPoint.Y, "ms")
			});
		}

		private void SetColumnChart()
		{
			foreach (var item in ComparisonRowChartSeriesCollection)
			{
				item.Values.Clear();
			}

			for (int i = 0; i < ComparisonRecords.Count; i++)
			{
				AddToColumnCharts(ComparisonRecords[i]);
			}
		}

		private void SetFrametimeChart()
		{
			for (int i = 0; i < ComparisonRecords.Count; i++)
			{
				AddToFrameTimeChart(ComparisonRecords[i]);
			}

			UpdateAxesMinMax(true);
		}

		private void SetLShapeChart()
		{
			for (int i = 0; i < ComparisonRecords.Count; i++)
			{
				AddToLShapeChart(ComparisonRecords[i]);
			}

			ResetLShapeChart.OnNext(default(Unit));
		}

		private ComparisonRecordInfoWrapper GetWrappedRecordInfo(ComparisonRecordInfo comparisonRecordInfo)
		{
			var wrappedComparisonRecordInfo = new ComparisonRecordInfoWrapper(comparisonRecordInfo, this);

			var color = _comparisonColorManager.GetNextFreeColor();
			wrappedComparisonRecordInfo.Color = color;
			wrappedComparisonRecordInfo.FrametimeGraphColor = color.Color;

			return wrappedComparisonRecordInfo;
		}

		void IDropTarget.Drop(IDropInfo dropInfo)
		{
			if (dropInfo != null)
			{
				if (dropInfo.VisualTarget is FrameworkElement frameworkElement)
				{
					if (frameworkElement.Name == "ComparisonRecordItemControl" ||
						frameworkElement.Name == "ComparisonImage")
					{
						if (dropInfo.Data is IFileRecordInfo recordInfo)
						{
							AddComparisonItem(recordInfo);
						}

						if (dropInfo.Data is ComparisonRecordInfoWrapper wrappedRecordInfo)
						{
							// manage sorting
							int currentIndex = ComparisonRecords.IndexOf(wrappedRecordInfo);

							if (dropInfo.InsertIndex < ComparisonRecords.Count)
							{
								ComparisonRecords.Move(currentIndex, dropInfo.InsertIndex);

								foreach (var rowSeries in ComparisonRowChartSeriesCollection)
								{
									var chartValueList = (rowSeries.Values as IList<double>).Reverse().ToList();
									chartValueList.Move(currentIndex, dropInfo.InsertIndex);
									chartValueList.Reverse();
									rowSeries.Values.Clear();
									rowSeries.Values.AddRange(chartValueList.Select(chartValue => chartValue as object));
								}

								var labelList = ComparisonRowChartLabels.Reverse().ToList();
								labelList.Move(currentIndex, dropInfo.InsertIndex);
								labelList.Reverse();
								ComparisonRowChartLabels = labelList.ToArray();
							}
						}
					}
				}
			}
		}

		void IDropTarget.DragOver(IDropInfo dropInfo)
		{
			if (dropInfo != null)
			{
				dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
				dropInfo.Effects = DragDropEffects.Move;
			}
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
	}
}
