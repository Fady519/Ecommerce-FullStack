using E_commerce.Domain.Contracts;
using E_commerce.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Persistence
{
    internal static class SpecificationEvaluator
    {

        public static IQueryable<TEntity> CreateQuery<TEntity , TKey>(IQueryable<TEntity> EntryPoint , 
            ISpecifications<TEntity , TKey> specifications) where TEntity : BaseEntitey<TKey>
        {

            var Query = EntryPoint;

            if (specifications is not null)
            {


                if (specifications.Criteria is not null)
                {
                    Query = Query.Where(specifications.Criteria);
                }

                if (specifications.IncludeExpressions is not null && specifications.IncludeExpressions.Any())
                {

                    Query = specifications.IncludeExpressions.Aggregate(Query
                        , (CurrentQuery, IncludeExp) => CurrentQuery.Include(IncludeExp));

                }

                if(specifications.OrderBy is not null)
                {
                    Query = Query.OrderBy(specifications.OrderBy);
                }

                if(specifications.OrderByDescending is not null)
                {
                    Query = Query.OrderByDescending(specifications.OrderByDescending);
                }

                if(specifications.IsPaginated)
                {
                    Query = Query.Skip(specifications.Skip).Take(specifications.Take);
                }

            }

            return Query;
        }

    }
}
