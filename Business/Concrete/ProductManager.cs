using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Validation;
using Core.Aspects.Autofac.Transaction;
using Core.CrossCuttingConcerns.Validation;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Aspects.Autofac.Performance;
using System.Threading;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.log4Net.Loggers;

namespace Business.Concrete
{
    public class ProductManager : IProductService
    {
        //Cross Cutting Concerns - Validation, Cache, Log, Performance, Aut, Transaction
        //AOP - Aspect Oriented Programming

        private IProductDal _productDal;        

        public ProductManager(IProductDal productDal)
        {
            _productDal = productDal;            
        }
                
        [ValidationAspect(typeof(ProductValidator),Priority =1)]        
        [CacheRemoveAspect("IProductService.Get")]
        public IResult Add(Product product)
        {            
            _productDal.Add(product);
            return new SuccessResult(Messages.ProductAdded);
        }

        public IResult Delete(Product product)
        {
            _productDal.Delete(product);
            return new SuccessResult(Messages.ProductDeleted);
        }

        public IDataResult<Product> GetById(int productId)
        {
            return new SuccessDataResult<Product>(_productDal.Get(p => p.ProductId == productId));
        }

        [PerformanceAspect(5)]
        [LogAspect(typeof(DatabaseLogger))]
        public IDataResult<List<Product>> GetList()
        {
            //Thread.Sleep(5000);
            return new SuccessDataResult<List<Product>>(_productDal.GetList().ToList());
        }

        //[SecureOperation("Product.List,Admin")]
        [LogAspect(typeof(DatabaseLogger))]
        [CacheAspect(duration:10)]
        public IDataResult<List<Product>> GetListByCategory(int categoryId)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetList(p=> p.CategoryId == categoryId).ToList());
        }

        [TransactionScopeAspect]
        public IResult TranscaptionalOperation(Product product)
        {
            _productDal.Update(product);
            //_productDal.Add(product);
            return new SuccessResult(Messages.ProductUpdated);
        }

        public IResult Update(Product product)
        {
           _productDal.Update(product);
            return new SuccessResult(Messages.ProductUpdated);
        }
    }
}
