using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMLDataReader.WPF
{
    public class DelegateProcess<T> : IProgress<T>
    {
        private Action<T> a;
        public DelegateProcess(Action<T> report)
        {
            a = report;
        }
        public void Report(T value)
        {
            a(value);
        }
    }
}
