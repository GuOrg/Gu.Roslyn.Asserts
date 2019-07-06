namespace AstView
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts;

    public class ViewModel : INotifyPropertyChanged
    {
        private string source;
        private string generation;
        private Exception exception;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Source
        {
            get => this.source;
            set
            {
                if (value == this.source)
                {
                    return;
                }

                this.source = value;
                this.OnPropertyChanged();
                this.UpdateGeneration();
            }
        }

        public string Generation
        {
            get => this.generation;
            private set
            {
                if (value == this.generation)
                {
                    return;
                }

                this.generation = value;
                this.OnPropertyChanged();
            }
        }

        public Exception Exception
        {
            get => this.exception;
            private set
            {
                if (ReferenceEquals(value, this.exception))
                {
                    return;
                }

                this.exception = value;
                this.OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void UpdateGeneration()
        {
            this.Exception = null;
            this.Generation = null;
            if (string.IsNullOrWhiteSpace(this.source))
            {
                this.Generation = string.Empty;
            }
            else
            {
                try
                {
                    await Task.Run(() => this.Generation = SyntaxFactoryWriter.Serialize(this.source));
                }
                catch (Exception e)
                {
                    this.Exception = e;
                }
            }
        }
    }
}
