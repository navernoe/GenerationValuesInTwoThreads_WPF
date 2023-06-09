﻿using System.Collections.Concurrent;
using System.Collections.Specialized;
using WpfApp.Domain;

namespace WpfApp.Logic.GeneratedValueHandlers;

/// <summary>
/// Обработчик сгенерированных значений.
/// </summary>
public abstract class BaseGeneratedValueHandler
{
    protected ConcurrentBag<IGeneratedProperties> GeneratedValuesContainer;

    protected BaseGeneratedValueHandler(ConcurrentBag<IGeneratedProperties> generatedValuesContainer)
    {
        GeneratedValuesContainer = generatedValuesContainer;
    }

    public int? ThreadId { get; private set; }

    public async Task GeneratedValues_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems is not null)
        {
            ThreadId = Thread.CurrentThread.ManagedThreadId;

            foreach (var newItem in e.NewItems)
            {
                var generatedValue = (IGeneratedProperties) newItem;
                GeneratedValuesContainer.Add(generatedValue);

                await UpdateDbAccordingGeneratedValue(generatedValue);
                await AdditionalHandle(generatedValue);
            }
        }
    }

    protected abstract Task AdditionalHandle(IGeneratedProperties generatedValue);

    protected abstract Task UpdateDbAccordingGeneratedValue(IGeneratedProperties newGeneratedValue);
}