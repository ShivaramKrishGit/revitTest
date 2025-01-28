using System;
using System.Windows;
using RevitMeshOptimizer.Core;

namespace RevitMeshOptimizer.UI
{
    public partial class OptimizationWindow : Window
    {
        private readonly MeshSimplifier _meshSimplifier;
        private readonly Action<double, double> _onApply;

        public OptimizationWindow(int originalTriangleCount, Action<double, double> onApply)
        {
            InitializeComponent();
            _onApply = onApply;
            _meshSimplifier = new MeshSimplifier();

            // Initialize statistics
            OriginalCountText.Text = originalTriangleCount.ToString();
            UpdateOptimizationStats();

            // Wire up event handlers
            ReductionSlider.ValueChanged += (s, e) => UpdateOptimizationStats();
            QualitySlider.ValueChanged += (s, e) => UpdateOptimizationStats();
        }

        private void UpdateOptimizationStats()
        {
            double targetReduction = ReductionSlider.Value / 100.0;
            int originalCount = int.Parse(OriginalCountText.Text);
            int estimatedCount = (int)(originalCount * (1 - targetReduction));

            OptimizedCountText.Text = estimatedCount.ToString();
            ReductionAchievedText.Text = $"{ReductionSlider.Value:F1}%";
        }

        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // TODO: Implement preview functionality
                MessageBox.Show("Preview functionality coming soon!", "Preview", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during preview: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double targetReduction = ReductionSlider.Value / 100.0;
                double qualityThreshold = QualitySlider.Value;

                _onApply?.Invoke(targetReduction, qualityThreshold);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying optimization: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}