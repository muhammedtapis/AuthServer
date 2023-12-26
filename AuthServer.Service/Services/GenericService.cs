using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWorks;
using AuthServer.Service.Mapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class GenericService<TEntity, TDto> : IGenericService<TEntity, TDto> where TEntity : class where TDto : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<TEntity> _genericRepository;

        public GenericService(IUnitOfWork unitOfWork, IGenericRepository<TEntity> genericRepository)
        {
            _unitOfWork = unitOfWork;
            _genericRepository = genericRepository;
        }

        //mapperi DI nesnesi olarka çağırmıyoruz lazy tanımladık ve statik metot olarak belirttik direkt sınıf üzerinden çağırcaz
        public async Task<ResponseDTO<TDto>> AddAsync(TDto dto)
        {
            var newEntity = ObjectMapper.Mapper.Map<TEntity>(dto);
            await _genericRepository.AddAsync(newEntity);
            await _unitOfWork.CommitAsync();
            var newDto = ObjectMapper.Mapper.Map<TDto>(newEntity);
            return ResponseDTO<TDto>.Success(StatusCodes.Status201Created, newDto);
        }

        public async Task<ResponseDTO<IEnumerable<TDto>>> GetAllAsync()
        {
            var entityList = await _genericRepository.GetAllAsync();
            var dtoList = ObjectMapper.Mapper.Map<IEnumerable<TDto>>(entityList);
            return ResponseDTO<IEnumerable<TDto>>.Success(StatusCodes.Status200OK, dtoList);
        }

        public async Task<ResponseDTO<TDto>> GetByIdAsync(int id)
        {
            var entity = await _genericRepository.GetByIdAsync(id);

            if (entity == null)
            {
                return ResponseDTO<TDto>.Fail(StatusCodes.Status404NotFound, $"{id} not found!", true);
            }
            var dto = ObjectMapper.Mapper.Map<TDto>(entity);
            return ResponseDTO<TDto>.Success(StatusCodes.Status200OK, dto);
        }

        public async Task<ResponseDTO<NoContentDTO>> RemoveAsync(int id)
        {
            var entity = await _genericRepository.GetByIdAsync(id);

            if (entity == null)
            {
                return ResponseDTO<NoContentDTO>.Fail(StatusCodes.Status404NotFound, $"{id} not foud!", true);
            }
            _genericRepository.Remove(entity);
            await _unitOfWork.CommitAsync();
            return ResponseDTO<NoContentDTO>.Success(StatusCodes.Status200OK);
        }

        public async Task<ResponseDTO<NoContentDTO>> UpdateAsync(TDto dto, int id)
        {
            //getbyId metodunda memoryde track edilmiyor eğer edilseydi gelen dto ve bu metodun sonucu iki aynı idye sahip
            //entity modified edilmeye çalışılacaktı
            var entity = await _genericRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return ResponseDTO<NoContentDTO>.Fail(StatusCodes.Status404NotFound, $"{id} not found!", true);
            }
            //Map.<source type ,destination type>.(source,destination)
            ObjectMapper.Mapper.Map<TDto, TEntity>(dto, entity);
            //yeni instance oluşturmadan mevcut entity üzerine yani gtbyid metodundan gelen entity üzerinde değişiklik yaptığımzda saveleyince
            //update edilmiş oluyor.

            //eskiden böyle yazıyoduk ama burada tracking sorunu var yukarıdaki gibi yapıp
            //gelen veriyi getbyid metoduyla çektiğin veriyle direkt mapliyorsun böylelikle track sorunu ortadan kalkıyor
            //await _genericRepository.Update(updatedEntity);

            await _unitOfWork.CommitAsync();
            return ResponseDTO<NoContentDTO>.Success(StatusCodes.Status204NoContent);
        }

        public async Task<ResponseDTO<IEnumerable<TDto>>> WhereAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entityList = await _genericRepository.Where(predicate).ToListAsync(); //dönüş tipimiz Ienumerable olduğ için tolist yaptık
            var dtoList = ObjectMapper.Mapper.Map<IEnumerable<TDto>>(entityList);
            return ResponseDTO<IEnumerable<TDto>>.Success(StatusCodes.Status200OK, dtoList);
        }
    }
}