using System.Windows;

namespace PocketPal.Settings;

public partial class SettingsWindow : Window
{
    private readonly AppSettings _settings;
    private readonly SettingsManager _manager = new();


    public SettingsWindow()
    {
        InitializeComponent();

        _settings = _manager.Load();

        StartupBox.IsChecked = _settings.StartWithWindows;
        ClickThroughBox.IsChecked = _settings.ClickThrough;
        StaticBox.IsChecked = _settings.StaticMode;

        CharacterBox.SelectedValue = _settings.SelectedCharacter;
    }


    private void Save_Click(object sender, RoutedEventArgs e)
    {
        _settings.StartWithWindows =
            StartupBox.IsChecked == true;

        _settings.ClickThrough =
            ClickThroughBox.IsChecked == true;

        _settings.StaticMode =
            StaticBox.IsChecked == true;


        if (CharacterBox.SelectedItem is System.Windows.Controls.ComboBoxItem item)
        {
            _settings.SelectedCharacter =
                item.Content.ToString()!;
        }


        _manager.Save(_settings);

        Close();
    }
}
