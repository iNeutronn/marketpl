using Business.Interfaces;
using Data.Entities;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Data.Interfaces;
using AutoMapper;
using System.ComponentModel.DataAnnotations;
using Business.Validation;
using System;

namespace Business.Services
{
    public abstract class GenericService<TEntity, TModel> : ICrud<TModel>
         where TEntity : BaseEntity
         where TModel : class
    {
        protected abstract IRepository<TEntity> _repository 
        {
            get;
        }
        protected readonly IUnitOfWork _uof;
        protected readonly IMapper _mapper;

        protected GenericService(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;
        }
        public virtual Task AddAsync(TModel model)
        {
            Validate(model);
            return _repository.AddAsync(_mapper.Map<TEntity>(model))
                  .ContinueWith(t => _uof.SaveAsync());
        }

        protected void Validate(object model)
        {
            if (model is null)
                throw new MarketException("Model is null", new NullReferenceException());
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(model, context, results,true)) 
            {
                var message = new StringBuilder();
                message.AppendLine($"Validation error. Can`t validate {model.GetType().Name}. Validation problems:");
                foreach (var error in results)
                {
                    message.AppendLine(error.ErrorMessage);
                }
                throw new MarketException(message.ToString(),new ValidationException());
            }
              
        }

        public virtual Task DeleteAsync(int modelId)
        {
            return _repository.DeleteByIdAsync(modelId)
                .ContinueWith(t => _uof.SaveAsync());
        }

        public virtual Task<IEnumerable<TModel>> GetAllAsync()
        {
            return _repository.GetAllAsync()
                .ContinueWith(t => _mapper.Map<IEnumerable<TModel>>(t.Result));
        }

        public virtual Task<TModel> GetByIdAsync(int id)
        {
            return _repository.GetByIdAsync(id)
                .ContinueWith(t => _mapper.Map<TModel>(t.Result));
        }

        public virtual Task UpdateAsync(TModel model)
        {
            Validate(model);
            var entity = _mapper.Map<TEntity>(model);
            _repository.Update(entity);
            return _uof.SaveAsync();
        }
    }
}
