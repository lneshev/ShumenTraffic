using Microsoft.AspNetCore.Mvc;
using MoravianStar.Dao;
using MoravianStar.WebAPI.Attributes;
using MoravianStar.WebAPI.Helpers;
using ShumenTraffic.Persistence.DbContexts;
using ShumenTraffic.Web.Core.DTOs;
using ShumenTraffic.Web.WebAPI.Infrastructure.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.WebAPI.Controllers
{
    /// <summary>
    /// The base WebAPI controller for the most common operations over an entity (like CRUD, count, exist, etc.), defined in the REST standard.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TId">The type of the Id of the entity.</typeparam>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    [ApiController]
    [Route(RoutingConstants.ApiController)]
    public abstract class EntityRestController<TEntity, TId, TModel, TFilter> : ControllerBase
        where TEntity : class, IEntityBase<TId>, IProjectionBase, new()
        where TModel : class, IModelBase<TId>, IProjectionBase, new()
        where TFilter : FilterSorterBase<TEntity>, new()
    {
        protected readonly EntityRestControllerHelper<TEntity, TId, TModel, TFilter, AppDbContext> helper;

        public EntityRestController()
        {
            helper = new EntityRestControllerHelper<TEntity, TId, TModel, TFilter, AppDbContext>();
        }

        /// <summary>
        /// Get entity by Id
        /// </summary>
        /// <param name="id">The target Id.</param>
        /// <returns>The found <typeparamref name="TEntity"/> by Id, transformed into <typeparamref name="TModel"/></returns>
        [NonInvokable]
        [HttpGet(RoutingConstants.Id)]
        public virtual async Task<ActionResult<ApiResponse<TModel>>> Get([FromRoute] TId id)
        {
            TModel data = await helper.GetAsync(id);
            var result = ApiResponse<TModel>.SuccessResponse(data);
            return result;
        }

        /// <summary>
        /// Get entities, optionally by a filter, sortings and paging, and transforms each entity to a given model.
        /// </summary>
        /// <param name="filter">The <see cref="FilterSorterBase{TEntity}"/> instance used for filtering.</param>
        /// <param name="sorts">The collection of sorts used for sorting.</param>
        /// <param name="page">The page object used for paging.</param>
        /// <returns>The found entities, transformed into <see cref="IEnumerable{TModel}"/>, and their total count (excluding the paging), wrapped in <see cref="PageResult{TModel}"/> object.</returns>
        /// <exception cref="SecurityException"></exception>
        [NonInvokable]
        [HttpGet]
        [ExecuteInTransactionAsync]
        public virtual async Task<ActionResult<ApiResponse<PageResult<TModel>>>> Read([FromQuery] TFilter filter, [FromQuery] List<Sort> sorts, [FromQuery] Page page)
        {
            var data = await helper.ReadAsync(filter, sorts, page);
            var result = ApiResponse<PageResult<TModel>>.SuccessResponse(data);
            return result;
        }

        /// <summary>
        /// Counts entities, optionally by a filter.
        /// </summary>
        /// <param name="filter">The <see cref="FilterSorterBase{TEntity}"/> instance used for filtering.</param>
        /// <returns>The number of the found entities.</returns>
        [NonInvokable]
        [HttpGet(RoutingConstants.Action)]
        public virtual async Task<ActionResult<ApiResponse<int>>> Count([FromQuery] TFilter filter)
        {
            int data = await helper.CountAsync(filter);
            var result = ApiResponse<int>.SuccessResponse(data);
            return result;
        }

        /// <summary>
        /// Checks if entities exist, optionally by a filter.
        /// </summary>
        /// <param name="filter">The <see cref="FilterSorterBase{TEntity}"/> instance used for filtering.</param>
        /// <returns><see langword="True"/> if the entities exist, otherwise <see langword="false"/>.</returns>
        [NonInvokable]
        [HttpGet(RoutingConstants.Action)]
        public virtual async Task<ActionResult<ApiResponse<bool>>> Exist([FromQuery] TFilter filter)
        {
            bool data = await helper.ExistAsync(filter);
            var result = ApiResponse<bool>.SuccessResponse(data);
            return result;
        }

        /// <summary>
        /// Creates and saves an entity, based on a <typeparamref name="TModel"/>.
        /// </summary>
        /// <param name="model">The model containing the input data of the entity, that will be created.</param>
        /// <returns>The model containing the input data. The model might be modified by the logic.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [NonInvokable]
        [HttpPost]
        [ExecuteInTransactionAsync]
        public virtual async Task<ActionResult<TModel>> Post([FromBody] TModel model)
        {
            var data = await helper.CreateAsync(model);
            var result = ApiResponse<TModel>.SuccessResponse(data);
            return CreatedAtAction(nameof(Get), new { id = data.Id }, result);
        }

        /// <summary>
        /// Updates and saves an entity, based on a <typeparamref name="TModel"/>.
        /// </summary>
        /// <param name="id">The Id of the entity that will be updated.</param>
        /// <param name="model">The model containing the input data of the entity, that will be updated.</param>
        /// <returns>The model containing the input data. The model might be modified by the logic.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidModelStateException"></exception>
        [NonInvokable]
        [HttpPut(RoutingConstants.Id)]
        [ExecuteInTransactionAsync]
        public virtual async Task<ActionResult<ApiResponse<TModel>>> Put([FromRoute] TId id, [FromBody] TModel model)
        {
            TModel data = await helper.UpdateAsync(id, model);
            var result = ApiResponse<TModel>.SuccessResponse(data);
            return result;
        }

        /// <summary>
        /// Deletes an entity, based on an Id.
        /// </summary>
        /// <param name="id">The target Id.</param>
        /// <returns>A model of the found entity in a state before the deletion.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [NonInvokable]
        [HttpDelete(RoutingConstants.Id)]
        [ExecuteInTransactionAsync]
        public virtual async Task<ActionResult<ApiResponse<TModel>>> Delete([FromRoute] TId id)
        {
            TModel data = await helper.DeleteAsync(id);
            var result = ApiResponse<TModel>.SuccessResponse(data);
            return result;
        }
    }
}