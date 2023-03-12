using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Nowy.Database.Client.Services;
using Nowy.Database.Contract.Models;

// ReSharper disable ArrangeMethodOrOperatorBody
// ReSharper disable MemberCanBePrivate.Global

namespace Nowy.UI.Server.Services;

public sealed record ModelTypeSettings(string DatabaseName);

public abstract class BaseDatabaseService
{
    protected readonly ILogger _logger;
    protected readonly INowyDatabase _nowy_database;

    protected static readonly object _lock = new();
    private readonly Dictionary<(Type model_type, string uuid), BaseModel> _models = new();
    private ImmutableDictionary<Type, ModelTypeSettings> _model_types = ImmutableDictionary<Type, ModelTypeSettings>.Empty;

    public event Action? ModelCollectionChanged;

    private static readonly BlockingCollection<Func<Task>> _tasks = new();

    public UnixTimestamp LatestInteractionTimestamp { get; set; } = UnixTimestamp.Epoch;


    protected BaseDatabaseService(ILogger logger, INowyDatabase nowy_database)
    {
        this._logger = logger;
        this._nowy_database = nowy_database;
    }

    public void Run()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                Func<Task> task = _tasks.Take();

                try
                {
                    await task.Invoke();
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, $"Exception in database task");
                }
            }
        }).Forget();

        this._triggerLoop();

        this.Load();
    }

    private void _triggerLoop()
    {
        _tasks.Add(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            await this._loopAsync();

            this._triggerLoop();
        });
    }

    public void RegisterModel<TModel>(string database_name) where TModel : BaseModel
    {
        lock (_lock)
        {
            this._model_types = this._model_types.SetItem(typeof(TModel), new ModelTypeSettings(DatabaseName: database_name));
        }
    }

    public IReadOnlyDictionary<(Type model_type, string uuid), BaseModel> GetAllModels()
    {
        lock (_lock)
        {
            return this._models.ToDictionarySafe(o => o.Key, o => o.Value);
        }
    }

    public void Add(BaseModel item)
    {
        lock (_lock)
        {
            Type item_type = item.GetType();
            this._models[( item_type, item.uuid ?? string.Empty )] = item;
        }
    }

    public void Delete(BaseModel item)
    {
        lock (_lock)
        {
            this._models.Remove(( item.GetType(), item.uuid ?? string.Empty ));
        }
    }

    public void AddRange(IEnumerable<BaseModel> collection)
    {
        this.AddRange(collection, out int count_updated);
    }

    public void AddRange(IEnumerable<BaseModel> collection, out int count_updated)
    {
        count_updated = 0;

        lock (_lock)
        {
            foreach (BaseModel item in collection)
            {
                Type item_type = item.GetType();
                (Type item_type, string) models_key = ( item_type, item.uuid ?? string.Empty );
                this._models.TryGetValue(models_key, out BaseModel? existing_item);
                if (existing_item is null || item.timestamp_database_update > existing_item.timestamp_database_update)
                {
                    this._models[models_key] = item;
                    count_updated++;
                }
            }
        }
    }

    public BaseModel[] Fetch(Predicate<BaseModel> predicate)
    {
        BaseModel[] ret;
        lock (_lock)
        {
            ret = this._models.Values.Where(o => predicate(o)).ToArray();
        }

        return ret;
    }

    public BaseModel[] Fetch(Func<IEnumerable<BaseModel>, IEnumerable<BaseModel>>? query_transform = null)
    {
        BaseModel[] ret;
        lock (_lock)
        {
            IEnumerable<BaseModel> query = this._models.Values;
            if (query_transform is { })
                query = query_transform(query);
            ret = query.ToArray();
        }

        return ret;
    }

    public TModel[] FetchBy<TModel>(Predicate<TModel> predicate, Func<IEnumerable<TModel>, IEnumerable<TModel>>? query_transform = null) where TModel : BaseModel
    {
        TModel[] ret;
        lock (_lock)
        {
            IEnumerable<TModel> query = this._models.Values.OfType<TModel>().Where(o => predicate(o));
            if (query_transform is { })
                query = query_transform(query);
            ret = query.ToArray();
        }

        return ret;
    }

    public TModel[] Fetch<TModel>(Func<IEnumerable<TModel>, IEnumerable<TModel>>? query_transform = null) where TModel : BaseModel
    {
        TModel[] ret;
        lock (_lock)
        {
            IEnumerable<TModel> query = this._models.Values.OfType<TModel>();
            if (query_transform is { })
                query = query_transform(query);
            ret = query.ToArray();
        }

        return ret;
    }

    public TModel? FetchFirstOrDefault<TModel>(Predicate<TModel> predicate, Func<IEnumerable<TModel>, IEnumerable<TModel>>? query_transform = null) where TModel : BaseModel
    {
        TModel? ret;
        lock (_lock)
        {
            IEnumerable<TModel> query = this._models.Values.OfType<TModel>().Where(o => predicate(o));
            if (query_transform is { })
                query = query_transform(query);
            ret = query.FirstOrDefault();
        }

        return ret;
    }

    public TModel? FetchById<TModel>(string? uuid) where TModel : BaseModel
    {
        TModel? ret;
        lock (_lock)
        {
            if (this._models.TryGetValue(( typeof(TModel), uuid ?? string.Empty ), out BaseModel? out_m))
            {
                ret = (TModel)out_m;
            }
            else
            {
                ret = null;
            }
        }

        return ret;
    }

    public BaseModel? FetchById(string? uuid, Type model_type)
    {
        BaseModel? ret;
        lock (_lock)
        {
            if (this._models.TryGetValue(( model_type, uuid ?? string.Empty ), out BaseModel? out_m))
            {
                ret = (BaseModel)out_m;
            }
            else
            {
                ret = null;
            }
        }

        return ret;
    }


    public void Load()
    {
        _tasks.Add(async () =>
        {
            await this._loadFromStorageAsync();
            await this._loadStaticDataAsync();
        });
    }

    protected virtual Task _loadFromStorageAsync()
    {
        return Task.CompletedTask;
    }

    protected virtual Task _loadStaticDataAsync()
    {
        return Task.CompletedTask;
    }

    public void Save()
    {
        _tasks.Add(async () =>
        {
            bool is_save_necessary;
            lock (_lock)
            {
                is_save_necessary = this._models.Values.Any(o => o.ShouldSave);
            }

            if (is_save_necessary)
            {
                await this._saveToStorageAsync();
            }
        });
    }

    protected virtual Task _saveToStorageAsync()
    {
        return Task.CompletedTask;
    }

    protected virtual Task _loopAsync()
    {
        return Task.CompletedTask;
    }

    private string _getStorageFilePath(Type model_type, string bucket)
    {
        if (!string.IsNullOrEmpty(bucket))
            return Path.Combine("files", $"models.{model_type.Name}.bucket-{bucket}.json");
        else
            return Path.Combine("files", $"models.{model_type.Name}.json");
    }

    protected async Task _syncWithNowyDatabaseAsync()
    {
        foreach (( Type model_type, ModelTypeSettings model_type_settings ) in this._model_types)
        {
            await ( ( typeof(BaseDatabaseService).GetMethod(nameof(this._syncWithNowyDatabaseWithTypeAsync), BindingFlags.Instance | BindingFlags.NonPublic)!
                .MakeGenericMethod(model_type)
                .Invoke(this, new object?[] { model_type_settings, }) as Task ) ?? Task.CompletedTask );
        }
    }

    protected async Task _syncWithNowyDatabaseWithTypeAsync<TModel>(ModelTypeSettings model_type_settings) where TModel : BaseModel
    {
        Type model_type = typeof(TModel);

        INowyCollection<TModel> collection = this._nowy_database.GetCollection<TModel>(database_name:model_type_settings.DatabaseName);

        TModel[] models_to_save;
        do
        {
            lock (_lock)
            {
                models_to_save = this._models
                    .Values
                    .Where(o => o.ShouldSave)
                    .OfType<TModel>()
                    .ToArray();
            }

            long timestamp_millis_now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            this._logger.LogInformation($"Save {models_to_save.Length} models of type {model_type.Name}.");

            foreach (TModel model in models_to_save)
            {
                if (model.timestamp_database_insert == 0)
                {
                    model.timestamp_database_insert = timestamp_millis_now;
                    model.timestamp_database_update = timestamp_millis_now;
                    model.ShouldSave = false;

                    try
                    {
                        await collection.InsertAsync(model.uuid, model);
                    }
                    catch (NowyDatabaseException ex) // when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                    {
                        this._logger.LogWarning($"Skipped Nowy Write error: {ex.Message}");
                    }
                }
                else
                {
                    model.timestamp_database_update = timestamp_millis_now;
                    model.ShouldSave = false;

                    await collection.UpdateAsync(model.uuid, model);
                }
            }
        } while (models_to_save.Length != 0);

        IReadOnlyList<TModel> models_from_mongo = await collection.GetAllAsync();

        if (models_from_mongo.Count != 0)
        {
            this.AddRange(models_from_mongo, out int count_updated);

            this._logger.LogInformation($"Load {count_updated} models of type {model_type.Name}.");
        }
    }

    /*
    protected async Task _loadFromStorageStorageFiles()
    {
        Directory.CreateDirectory("files");
        foreach (Type model_type in this._model_types)
        {
            foreach (string bucket in BaseModelExtensions.GetStorageBuckets())
            {
                string file_path = this._getStorageFilePath(model_type, bucket: bucket);
                string file_path_temp = $"{file_path}.tmp.{StringHelper.Random.NextLong(1, 100):000}";

                Log.Information($"Read: {file_path}");

                List<dynamic>? data = ( await File.ReadAllTextAsync(file_path) )?.FromJson<List<dynamic>>(JsonPropertyName, type_name_handling: false);

                // Log.Information(data?.Take(5).ToJson(error_handling: true, type_name_handling: false, preserve_object_references: true));
                // Log.Information(data?.Take(5).Select(o => o.GetType()).ToJson(error_handling: true, type_name_handling: false, preserve_object_references: true));

                if (data is { })
                {
                    this.AddRange(data.OfType<BaseModel>());
                }
            }
        }
    }

    protected async Task _saveToStorageStorageFiles()
    {
        IReadOnlyDictionary<(Type model_type, string uuid), BaseModel> all_models = this.GetAllModels();

        foreach (Type model_type in this._model_types)
        {
            IEnumerable<BaseModel> data_enumerable = all_models
                .Where(o => o.Key.model_type == model_type)
                .Select(o => o.Value);

            if (data_enumerable.Any(o => o.ShouldSave))
            {
                BaseModel[] data = data_enumerable.ToArray();

                foreach (BaseModel model in data)
                {
                    model.ShouldSave = false;
                }

                foreach (IGrouping<string, (string bucket, BaseModel o)> group in data.Select(o => ( bucket: BaseModelExtensions.GetStorageBucket(o.uuid), o ))
                             .GroupBy(o => o.bucket))
                {
                    string file_path = this._getStorageFilePath(model_type, bucket: group.Key);
                    string file_path_temp = $"{file_path}.tmp.{StringHelper.Random.NextLong(1, 100):000}";

                    Log.Information($"Write: {file_path}");

                    await using (Stream stream = File.Open(file_path_temp, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                    {
                        group.ToArray().ToJson(stream, JsonPropertyName, type_name_handling: false, preserve_object_references: true);
                    }

                    File.Move(file_path_temp, file_path, true);
                }
            }
        }
    }
    */
}
