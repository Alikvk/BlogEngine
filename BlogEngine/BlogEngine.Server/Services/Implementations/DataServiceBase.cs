﻿using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlogEngine.Core.Services.Abstractions;
using AutoMapper;
using BlogEngine.Shared.Helpers;
using BlogEngine.Core.Data.Entities.Common;
using BlogEngine.Server.Services.Abstractions;

namespace BlogEngine.Server.Services.Implementations
{
    public class DataServiceBase<TEntity, TDTO, TCreationDTO, TUpdateDTO>
        : IDataServiceBase<TDTO, TCreationDTO, TUpdateDTO>
        where TEntity : BaseEntity
        where TDTO : class
        where TCreationDTO : class
        where TUpdateDTO : class
    {
        private readonly IAsyncRepository<TEntity> _repository;
        private readonly IMapper _mapper;

        public DataServiceBase(IAsyncRepository<TEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public virtual async Task<TDTO> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity is null) return null;

            return ToDTO(entity);
        }

        public virtual async Task<TUpdateDTO> GetUpdateDTOAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity is null) return null;

            return ToUpdateDTO(entity);
        }

        public virtual async Task<List<TDTO>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();

            return _mapper.Map<List<TDTO>>(entities.ToList());
        }

        public virtual async Task<TDTO> InsertAsync(TCreationDTO creationDTO)
        {
            Preconditions.NotNull(creationDTO, nameof(creationDTO));

            var entity = ToEntity(creationDTO);

            var insertedEntity = await _repository.InsertAsync(entity);

            return ToDTO(insertedEntity);
        }

        public virtual async Task<TDTO> UpdateAsync(int id, TUpdateDTO updateDTO)
        {
            Preconditions.NotNull(updateDTO, nameof(updateDTO));

            var entity = await _repository.GetByIdAsync(id);

            if (entity is null) return null;

            _mapper.Map(updateDTO, entity);

            var updatedEntity = await _repository.UpdateAsync(entity);

            return ToDTO(updatedEntity);
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity is null) return false;

            return await _repository.DeleteAsync(entity.ID);
        }

        protected TEntity ToEntity(TCreationDTO creationDTO) => _mapper.Map<TEntity>(creationDTO);
        protected TDTO ToDTO(TEntity entity) => _mapper.Map<TDTO>(entity);
        protected TUpdateDTO ToUpdateDTO(TEntity entity) => _mapper.Map<TUpdateDTO>(entity);
    }
}