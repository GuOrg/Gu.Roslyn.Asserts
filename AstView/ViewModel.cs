﻿namespace AstView;

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Gu.Roslyn.Asserts;

public class ViewModel : INotifyPropertyChanged
{
    private string? source;
    private string? generation;
    private Exception? exception;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string? Source
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

    public string? Generation
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

    public Exception? Exception
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

    public bool TrySelect(int pos, out int start, out int length)
    {
        start = 0;
        length = 0;
        return this.generation is { } text &&
               TryFindStart(text, out start) &&
               TryFindLength(text, start, out length);

        bool TryFindStart(string code, out int result)
        {
            result = code.LastIndexOf("SyntaxFactory.", pos + 15, StringComparison.Ordinal);
            return result >= 0;
        }

        static bool TryFindLength(string code, int from, out int result)
        {
            var level = 0;
            for (var i = from; i < code.Length; i++)
            {
                switch (code[i])
                {
                    case '(':
                        level++;
                        break;
                    case ')':
                        level--;
                        if (level == 0)
                        {
                            result = i - from + 1;
                            return true;
                        }

                        break;
                }
            }

            result = 0;
            return false;
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

#pragma warning disable VSTHRD100 // Avoid async void methods
    private async void UpdateGeneration()
#pragma warning restore VSTHRD100 // Avoid async void methods
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
                await Task.Run(() => this.Generation = SyntaxFactoryWriter.Serialize(this.source)).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                this.Exception = e;
            }
        }
    }
}
