using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nowy.Database.Contract.Models;
using Nowy.UI.Server.Services;

namespace Nowy.UI.Server.Controllers;

public abstract class BaseRestController<TItem> : ControllerBase where TItem : BaseModel
{
    protected readonly ILogger _logger;
    protected readonly BaseDatabaseService _database;

    protected BaseRestController(ILogger logger, BaseDatabaseService database)
    {
        this._logger = logger;
        this._database = database;
    }


    protected abstract Task<TItem?> _getModelByKeyAsync(string key);

    protected abstract Task<TItem> _createModelAsync();

    [HttpGet(Order = 99)]
    public async Task<IEnumerable<TItem>> Get()
    {
        this._database.LatestInteractionTimestamp = UnixTimestamp.Now;

        TItem[] ret = this._database.Fetch<TItem>();
        this._logger.LogInformation($"Get (count = {ret.Length})");

        return ret;
    }

    [HttpGet("{key}", Order = 98)]
    public async Task<ActionResult<TItem>> GetItem(string key)
    {
        this._database.LatestInteractionTimestamp = UnixTimestamp.Now;

        this._logger.LogInformation("GetItem");

        TItem? ret = await this._getModelByKeyAsync(key);

        if (ret is null)
        {
            return this.NotFound();
        }

        return ret;
    }

    [HttpPost(Order = 99)]
    public async Task<IActionResult> PostModel(TItem input)
    {
        this._database.LatestInteractionTimestamp = UnixTimestamp.Now;

        this._logger.LogInformation("PostModel");

        TItem? ret = await this._createModelAsync();

        this._copyProperties(ret, input);

        ret.ShouldSave = true;
        this._database.Add(ret);
        this._database.Save();

        return this.NoContent();
    }

    [HttpPut("{key}", Order = 99)]
    public async Task<IActionResult> PutModel(string key, TItem input)
    {
        this._database.LatestInteractionTimestamp = UnixTimestamp.Now;

        this._logger.LogInformation("PutModel");

        TItem? ret = await this._getModelByKeyAsync(key);

        if (ret is null)
        {
            return this.NotFound();
        }

        this._copyProperties(ret, input);

        ret.ShouldSave = true;
        this._database.Add(ret);
        this._database.Save();

        return this.NoContent();
    }

    private void _copyProperties(TItem ret, TItem input)
    {
        IEnumerable<PropertyInfo> props = ReflectionExtensions.GetPublicInstanceProperties(ret.GetType());
        foreach (PropertyInfo prop in props)
        {
            if (prop.CanWrite && prop.CanRead)
            {
                this._logger.LogInformation($"copy prop {prop.Name} ({prop.PropertyType.Name}) = {prop.GetValue(input)}");
                prop.SetValue(ret, prop.GetValue(input));
            }
        }
    }
}
