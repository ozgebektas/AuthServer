﻿using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class ServiceGeneric<TEntity, TDto> : IServiceGeneric<TEntity, TDto>
        where TEntity : class where TDto : class
    {
        //IUnitOfWork'ü çağıracağımız yer burası.savechanges işlemlerini yapacapımız yer(unitOfWork classının referansını tutan IUnitOfWork'den nesne türettik(losely coupled)
        //Veritabanına da ulaşmamız gerekli burada GenericRepository'in referansını tutan(onu implemente eden) IgenericRepository interface den nesne üretilir.
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<TEntity> _genericRepository;

        public ServiceGeneric(IUnitOfWork unitOfWork, IGenericRepository<TEntity> genericRepository)
        {
            _unitOfWork = unitOfWork;
            _genericRepository = genericRepository;

        }
        public async Task<Response<TDto>> AddAsync(TDto entity)
        {
            //Mapper.Map<TEntity>(entity) burada; eşleştir:Tentity ile entity yapısını.
            //not:Tentity bir entity model olurken entity ise TDtodan gelen clienta gidecek TDto nesnesi olacak.
            var newEntity = ObjectMapper.Mapper.Map<TEntity>(entity);//bu maplenen yapı newEntity de tutulacak.
            await _genericRepository.AddAsync(newEntity);//newEntity eklenecek.

            await _unitOfWork.CommitAsync();//savechanges ile veritabanına değişiklikler yansıyacak.

            //yeni data eklendi.newEnity'nin id'si yüklendi.geriye döneceğimiz Dto'nun id'sini de doldurmamız gerekiyor
            //yani TDto 'yu maple ne ile:newEntity ile.
            var newDto = ObjectMapper.Mapper.Map<TDto>(newEntity);

            return Response<TDto>.Success(newDto, 200);

        }


        public async Task<Response<IEnumerable<TDto>>> GetAllAsync()
        {
            var products = ObjectMapper.Mapper.Map<List<TDto>>(await _genericRepository.GetAllAsync());
            return Response<IEnumerable<TDto>>.Success(products, 200);
        }

        public async Task<Response<TDto>> GetByIdAsync(int id)
        {
            var product = await _genericRepository.GetByIdAsync(id);
            if (product == null)
            {
                return Response<TDto>.Fail("id not found", 404, true);
            }
            return Response<TDto>.Success(ObjectMapper.Mapper.Map<TDto>(product), 200);
        }

        public async Task<Response<NoDataDto>> Remove(int id)
        {
            var isExistEntity = await _genericRepository.GetByIdAsync(id);
            if (isExistEntity == null)
            {
                return Response<NoDataDto>.Fail("id not found", 404, true);
            }
            _genericRepository.Remove(isExistEntity);
            await _unitOfWork.CommitAsync();

            return Response<NoDataDto>.Success(204);
        }

        public async Task<Response<NoDataDto>> Update(TDto entity, int id)
        {
            var isExistEntity = await _genericRepository.GetByIdAsync(id);
            if (isExistEntity == null)
            {
                return Response<NoDataDto>.Fail("is not found", 404, true);
            }

            var updateEntity = ObjectMapper.Mapper.Map<TEntity>(entity);
            _genericRepository.Update(updateEntity);
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(204);
            //204 durum kodu=> no content=> Response body'sinde hiç bir data olmayacak.

        }

        public async Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate)
        {
            var list= _genericRepository.Where(predicate);
            //toList yada ToListAsync yazana kadar veri tabanına yansımayacak.
            //list.Skip(4).Take(5);//hala veri tabanına yansımadı(IQuerable dan dolayı)

            return Response<IEnumerable<TDto>>.Success(ObjectMapper.Mapper.Map<IEnumerable<TDto>>(await list.ToListAsync()),200);
          
        }
    }
}
