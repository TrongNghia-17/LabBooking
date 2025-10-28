global using System.Security.Claims;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using LabBooking.Application.Common;
global using LabBooking.Application.Features.DoorRequests.Dtos;
global using LabBooking.Application.Features.Incidents.Commands.CreateIncident;
global using LabBooking.Application.Features.Incidents.Dtos;
global using LabBooking.Application.Services.Authentication.External;
global using LabBooking.Application.Services.Authentication.Token;
global using LabBooking.Application.Services.Authentication.Users;
global using LabBooking.Domain.Constants;
global using LabBooking.Domain.Entities;
global using LabBooking.Domain.Exceptions;
global using LabBooking.Domain.Repositories;
global using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
global using MediatR;
global using AutoMapper;
global using FluentValidation;
global using Google.Apis.Auth;





