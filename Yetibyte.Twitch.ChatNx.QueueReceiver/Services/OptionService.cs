using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media;
using Yetibyte.Twitch.ChatNx.QueueReceiver.Models;

namespace Yetibyte.Twitch.ChatNx.QueueReceiver.Services
{
    public class OptionService
    { 
        public string OptionsFilePath { get; init; } = "settings.dat";

        public OptionService()
        {
        }

        public ApplicationOptions LoadApplicationOptions()
        {
            OptionsData optionsData = LoadOptionsData();
            ApplicationOptions applicationOptions = RestoreOptions(optionsData);

            return applicationOptions;
        }

        public OptionsData LoadOptionsData()
        {
            OptionsData defaultOptions = CreateOptionsData(ApplicationOptions.CreateDefault());

            if (!File.Exists(OptionsFilePath)) {

                SaveOptionsData(defaultOptions);
                return defaultOptions;
            }


            try
            {
                using FileStream fileStream = new FileStream(OptionsFilePath, FileMode.Open);

                OptionsData optionsData = OptionsData.Load(fileStream);

                return optionsData;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"{nameof(OptionService)}.{nameof(LoadOptionsData)} - Error: {ex.Message}");
                return defaultOptions;
            }

        }

        public bool SaveOptions(ApplicationOptions applicationOptions)
        {
            if (applicationOptions is null)
            {
                throw new ArgumentNullException(nameof(applicationOptions));
            }

            OptionsData optionsData = CreateOptionsData(applicationOptions);

            return SaveOptionsData(optionsData);
        }

        public bool SaveOptionsData(OptionsData optionsData)
        {
            try
            {
                using FileStream fileStream = new FileStream(OptionsFilePath, FileMode.OpenOrCreate);

                optionsData.Save(fileStream);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"{nameof(OptionService)}.{nameof(SaveOptionsData)} - Error: {ex.Message}");
                return false;
            }

            return true;
        }

        public OptionsData CreateOptionsData(ApplicationOptions applicationOptions)
        {
            if (applicationOptions is null)
            {
                throw new ArgumentNullException(nameof(applicationOptions));
            }

            OptionsData optionsData = new OptionsData
            {
                BackgroundColor = (applicationOptions.BackgroundColor?.Color).GetValueOrDefault().ToString(),
                CurrentItemBorderColor = (applicationOptions.CurrentItemBorderColor?.Color).GetValueOrDefault().ToString(),
                DividerColor = (applicationOptions.DividerColor?.Color).GetValueOrDefault().ToString(),
                HeaderTextColor = (applicationOptions.HeaderTextColor?.Color).GetValueOrDefault().ToString(),
                HistoryItemTextColor = (applicationOptions.HistoryItemTextColor?.Color).GetValueOrDefault().ToString(),
                QueueItemTextColor = (applicationOptions.QueueItemTextColor?.Color).GetValueOrDefault().ToString(),
                HeaderTextSize = applicationOptions.HeaderTextSize,
                HistoryItemTextSize = applicationOptions.HistoryItemTextSize,
                Port = applicationOptions.Port,
                QueueItemTextSize = applicationOptions.QueueItemTextSize,
                TimeStampScalePercentage = applicationOptions.TimeStampScalePercentage
            };

            return optionsData;
        }

        public ApplicationOptions RestoreOptions(OptionsData optionsData)
        {
            ApplicationOptions applicationOptions;
            ApplicationOptions defaultAppOptions = ApplicationOptions.CreateDefault();

            try
            {
                SolidColorBrush backgroundColor =  ColorConverter.ConvertFromString(optionsData.BackgroundColor) is  Color bgCol ? new SolidColorBrush(bgCol) : defaultAppOptions.BackgroundColor;
                SolidColorBrush dividerColor = ColorConverter.ConvertFromString(optionsData.DividerColor) is Color dividerCol ? new SolidColorBrush(dividerCol) : defaultAppOptions.DividerColor;
                SolidColorBrush headerTextColor = ColorConverter.ConvertFromString(optionsData.HeaderTextColor) is Color headerTextCol ? new SolidColorBrush(headerTextCol) : defaultAppOptions.HeaderTextColor;
                SolidColorBrush currentItemBorderColor = ColorConverter.ConvertFromString(optionsData.CurrentItemBorderColor) is Color currItemBorderCOl ? new SolidColorBrush(currItemBorderCOl) : defaultAppOptions.CurrentItemBorderColor;
                SolidColorBrush historyItemTextColor = ColorConverter.ConvertFromString(optionsData.HistoryItemTextColor) is Color historyItemCol ? new SolidColorBrush(historyItemCol) : defaultAppOptions.HistoryItemTextColor;
                SolidColorBrush queueItemTextColor = ColorConverter.ConvertFromString(optionsData.QueueItemTextColor) is Color queueItemCol ? new SolidColorBrush(queueItemCol) : defaultAppOptions.QueueItemTextColor;

                applicationOptions = new ApplicationOptions
                {
                    BackgroundColor = backgroundColor,
                    CurrentItemBorderColor = currentItemBorderColor,
                    DividerColor = dividerColor,
                    HeaderTextColor = headerTextColor,
                    HeaderTextSize = optionsData.HeaderTextSize,
                    HistoryItemTextColor = historyItemTextColor,
                    HistoryItemTextSize = optionsData.HistoryItemTextSize,
                    Port = optionsData.Port,
                    QueueItemTextColor = queueItemTextColor,
                    QueueItemTextSize = optionsData.QueueItemTextSize,
                    TimeStampScalePercentage = optionsData.TimeStampScalePercentage
                };

            }
            catch (FormatException ex)
            {
                System.Diagnostics.Debug.WriteLine($"{nameof(OptionService)}.{nameof(RestoreOptions)} - Error: {ex.Message}");
                applicationOptions = ApplicationOptions.CreateDefault();
            }

            return applicationOptions;
        }

    }
}
