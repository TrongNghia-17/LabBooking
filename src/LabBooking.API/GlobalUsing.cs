global using Google.Apis.Auth.OAuth2.Requests;
global using LabBooking.API.Extensions;
global using LabBooking.API.Middlewares;
global using LabBooking.Application.Common;
global using LabBooking.Application.Extensions;
global using LabBooking.Application.Features.Authentication.Commands.GoogleLogin;
global using LabBooking.Application.Features.Authentication.Commands.RefreshTokens;
global using LabBooking.Application.Features.Incidents.Commands.CreateIncident;
global using LabBooking.Application.Features.Incidents.Dtos;
global using LabBooking.Application.Features.Incidents.Queries.GetAllIncidents;
global using LabBooking.Application.Services.Caching;
global using LabBooking.Domain.Entities;
global using LabBooking.Domain.Exceptions;
global using LabBooking.Infrastructure.Extensions;
global using LabBooking.Infrastructure.Seeders;
global using MediatR;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.OpenApi.Models;
global using Serilog;
global using System.Diagnostics;
global using System.Security.Claims;
global using System.Text;
global using LabBooking.Application.Features.Supports.Commands.CreateSupport;
global using LabBooking.Application.Features.Supports.Commands.UpdateSupport;
global using LabBooking.Application.Features.Supports.Dtos;
global using LabBooking.Application.Features.Supports.Queries.GetAllSupports;
global using LabBooking.Application.Features.Supports.Queries.GetByIdSupport;








