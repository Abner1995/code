﻿using System.Runtime.CompilerServices;

namespace NeutronUI;

public abstract class NeutronBindableObject : BindableObject
{
    public void SetProperty<T>(ref T field, T value, Action<T> doAfter = null, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        OnPropertyChanged(propertyName);
        doAfter?.Invoke(value);
    }
}
