﻿using System;
using Autofac;
using Autofac.Features.OwnedInstances;
using SQLAzureDemo.App_Start.Autofac;
using SQLAzureDemo.Controllers.Services;
using SQLAzureDemo.Database.Repositories;

namespace SQLAzureDemo.Controllers
{
    public class TransientEntityFrameworkController : SearchController
    {
        public TransientEntityFrameworkController(IComponentContext scope, Func<Owned<IControllerOperationLogger>> operationLoggerFactory)
            : base(scope.ResolveKeyed<IMovieRepository>(EntityFrameworkModule.TransientConnection), operationLoggerFactory)
        {}
    }
}
