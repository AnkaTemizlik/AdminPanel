using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using DNA.API.Models;
using DNA.API.Resources;
using DNA.API.Resources.Auth;
//using DNA.Domain.Models.GIB;
using DNA.Domain.Models.Queries;
using DNA.Domain.Services.Communication;
using RestSharp;
using AutoMapper.Extensions.EnumMapping;
using DNA.Domain.Resources;
using DNA.Domain.Models;
using Newtonsoft.Json;
using DNA.API.Extensions;
using DNA.Domain.Exceptions;

namespace DNA.API.Mapping {
    public class AutoMapperProfile : Profile {
        public AutoMapperProfile() {

            CreateMap<Log, LogResource>();

            CreateMap(typeof(QueryResult<>), typeof(QueryResult<>));
            CreateMap(typeof(QueryResultResponse<>), typeof(QueryResultResponse<>));

            CreateMap<ApplicationUser, ApplicationUserResource>();
            CreateMap<ApplicationUserResource, ApplicationUser>();

            CreateMap<EntityQueryResource, EntityQuery>();
            CreateMap<QueryResource, EntityQuery>();
            CreateMap<CardQueryResource, CardsQuery>();
            CreateMap<UserQueryResource, UserQuery>();

            CreateMap<QueryResource, Query>()
                // .ForMember(d => d.Take, opt => opt.MapFrom(s => s.Take > 0 ? s.Take : 10000))
                .ForMember(d => d.Skip, opt => opt.MapFrom(s => s.Take > 0 && s.Page > 0 ? (s.Take / s.Page) : 0))
                .ForMember(d => d.Sort, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Sort) ? new List<Sort>() : JsonConvert.DeserializeObject<List<Sort>>(src.Sort)))
                .ForMember(d => d.Filter, o => o.MapFrom<WhereConditionResolver>())
                .IncludeAllDerived();

            CreateMap<SaveUserResource, ApplicationUser>();

            CreateMap(typeof(Response<>), typeof(Response<>)).ConvertUsing(typeof(ResponseConverter<,>));

            CreateMap(typeof(IRestResponse<>), typeof(Response<>)).ConvertUsing(typeof(RestResponseConverter<>));

            CreateMap<HttpMethod, Method>().ConvertUsing(typeof(HttpMethodConverter));

            CreateMap<Response, Notification>()
                .IncludeAllDerived()
                .ConvertUsing((s, d) => {
                    d ??= new Notification();
                    d.CreationTime = DateTime.Now;
                    d.Description = string.Join(Environment.NewLine, s.Details);
                    d.Title = s.Message;
                    d.Comment = $"{s.Comment}";
                    return d;
                });
        }

        class HttpMethodConverter : ITypeConverter<HttpMethod, Method> {
            public Method Convert(HttpMethod source, Method destination, ResolutionContext context) {
                if (source == HttpMethod.Get)
                    return Method.GET;
                if (source == HttpMethod.Post)
                    return Method.POST;
                if (source == HttpMethod.Delete)
                    return Method.DELETE;
                if (source == HttpMethod.Put)
                    return Method.PUT;
                if (source == HttpMethod.Options)
                    return Method.OPTIONS;
                if (source == HttpMethod.Head)
                    return Method.HEAD;
                if (source == HttpMethod.Patch)
                    return Method.PATCH;
                throw new NotImplementedException($"{source} is not implemented.");
            }
        }

        class ResponseConverter<T, K> : ITypeConverter<Response<T>, Response<K>> {
            public Response<K> Convert(Response<T> sourceMember, Response<K> destination, ResolutionContext context) {
                if (sourceMember.Success) {
                    return new Response<K>(context.Mapper.Map<K>(sourceMember.Resource)) {
                        Message = sourceMember.Message,
                        Comment = sourceMember.Comment
                    };
                }
                else {
                    return new Response<K>() {
                        Code = sourceMember.Code,
                        Comment = sourceMember.Comment,
                        Details = sourceMember.Details,
                        Message = sourceMember.Message,
                        Success = false,
                    };
                }
            }
        }

        class RestResponseConverter<T> : ITypeConverter<IRestResponse<T>, Response<T>> {
            public Response<T> Convert(IRestResponse<T> sourceMember, Response<T> destination, ResolutionContext context) {
                if (sourceMember.IsSuccessful) {
                    return new Response<T>(sourceMember.Data);
                }
                else {
                    if (sourceMember.ErrorException != null)
                        return new Response<T>(sourceMember.ErrorException);
                    else if (!string.IsNullOrWhiteSpace(sourceMember.ErrorMessage))
                        return new Response<T>(sourceMember.ErrorMessage);
                    else
                        return new Response<T>($"StatusCode: {sourceMember.StatusCode} {sourceMember.StatusDescription}");
                }
            }
        }
    }

}
