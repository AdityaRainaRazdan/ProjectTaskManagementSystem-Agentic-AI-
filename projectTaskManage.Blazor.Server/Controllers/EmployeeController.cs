using DevExpress.ExpressApp;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

[ApiController]
[Route("api/crud")]
public class CrudController : ControllerBase
{
    private readonly IObjectSpaceFactory objectSpaceFactory;

    public CrudController(IObjectSpaceFactory objectSpaceFactory)
    {
        this.objectSpaceFactory = objectSpaceFactory;
    }

    // GET ALL
    [HttpGet("list/{entityName}")]
    public IActionResult List(string entityName)
    {
        var entityType = FindEntityType(entityName);
        if (entityType == null)
            return BadRequest("Entity not found");

        using var os = objectSpaceFactory.CreateObjectSpace(entityType);

        var method = typeof(IObjectSpace)
            .GetMethod("GetObjectsQuery")
            .MakeGenericMethod(entityType);

        var query = method.Invoke(os, null) as IQueryable;

        var list = query.Cast<object>().ToList();

        return Ok(list);
    }


    // CREATE
    [HttpPost("create")]
    public IActionResult Create([FromBody] CrudRequest request)
    {
        var entityType = FindEntityType(request.Entity);
        if (entityType == null)
            return BadRequest("Entity not found");

        using var os = objectSpaceFactory.CreateObjectSpace(entityType);

        var obj = os.CreateObject(entityType);

        foreach (var prop in request.Data)
        {
            var property = entityType.GetProperty(prop.Key);
            if (property != null && property.CanWrite)
            {
                property.SetValue(
                    obj,
                    Convert.ChangeType(prop.Value, property.PropertyType)
                );
            }
        }

        os.CommitChanges();
        return Ok(obj);
    }

    // DELETE
    [HttpDelete("delete/{entity}/{id}")]
    public IActionResult Delete(string entity, Guid id)
    {
        var entityType = FindEntityType(entity);
        if (entityType == null)
            return BadRequest("Entity not found");

        using var os = objectSpaceFactory.CreateObjectSpace(entityType);

        var obj = os.GetObjectByKey(entityType, id);
        if (obj == null)
            return NotFound();

        os.Delete(obj);
        os.CommitChanges();

        return Ok("Deleted");
    }

    // UPDATE
    [HttpPut("update/{entity}/{id}")]
    public IActionResult Update(string entity, Guid id, [FromBody] Dictionary<string, object> data)
    {
        var entityType = FindEntityType(entity);
        if (entityType == null)
            return BadRequest("Entity not found");

        using var os = objectSpaceFactory.CreateObjectSpace(entityType);

        var obj = os.GetObjectByKey(entityType, id);
        if (obj == null)
            return NotFound();

        foreach (var prop in data)
        {
            var property = entityType.GetProperty(prop.Key);
            if (property != null && property.CanWrite)
            {
                property.SetValue(
                    obj,
                    Convert.ChangeType(prop.Value, property.PropertyType)
                );
            }
        }

        os.CommitChanges();
        return Ok(obj);
    }

    // ENTITY FINDER
    private Type FindEntityType(string name)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t =>
                t.IsClass &&
                t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}

public class CrudRequest
{
    public string Entity { get; set; }
    public Dictionary<string, object> Data { get; set; }
}
