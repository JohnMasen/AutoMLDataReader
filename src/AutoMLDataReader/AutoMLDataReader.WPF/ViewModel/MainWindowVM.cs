using AutoMLDataReader.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace AutoMLDataReader.WPF.ViewModel
{
    public class MainWindowVM:INotifyPropertyChanged
    {
        public string SwaggerUrl { get; set; }
        public ObservableCollection<string> SwaggerResultItems { get; set; } = new ObservableCollection<string>();

        public DataTable ResultTable { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public async void RefreshSwagger()
        {
            if (string.IsNullOrEmpty(SwaggerUrl))
            {
                return;
            }
            var result=await EndpointClient.ParseSwaggerAsync(SwaggerUrl);
            SwaggerResultItems.Clear();
            foreach (var item in result)
            {
                SwaggerResultItems.Add(item);
            }
        }

        public async void Load()
        {
            DataLoader loader = new DataLoader();
            ResultTable = await loader.LoadData(@"D:\Temp\dwd_sellin_invoice_daily_for_poc\Predict_KAP2_RegionSKU_Month_Raw.csv", "http://b28960d0-58a2-41ad-93c8-fa756b0e3307.chinaeast2.azurecontainer.console.azure.cn/score", "JhHUoZP5twdDrQoepqRRL3BLkdORw7cy");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResultTable)));
        }

    }
}
