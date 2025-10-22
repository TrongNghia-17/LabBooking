global using System.Text;
global using System.Security.Claims;
global using LabBooking.API.Extensions;
global using LabBooking.Application.Common;
global using LabBooking.Application.Extensions;
global using LabBooking.Application.Features.DoorRequests.Dtos;
global using LabBooking.Application.Features.DoorRequests.Queries.GetAllDoorRequests;
global using LabBooking.Application.Features.Incidents.Commands.CreateIncident;
global using LabBooking.Application.Features.Incidents.Dtos;
global using LabBooking.Application.Features.Incidents.Queries.GetAllIncidents;
global using LabBooking.Application.Features.Auths.Commands;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.OpenApi.Models;
global using MediatR;
global using Serilog;








