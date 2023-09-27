﻿/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/


using Finos.Fdc3.Backplane.Client.Extensions;
using Finos.Fdc3.Backplane.DTO;
using Finos.Fdc3.Backplane.DTO.Envelope;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Client.Transport
{
    internal class SignalRBackplaneTransport : IBackplaneTransport
    {
        private readonly ILogger<IBackplaneTransport> _logger;
        private const string MSG_CONNECTION_CLOSED = "Underlying connection is closed!";
        private readonly HubConnection _hubConnection;
        private readonly AppIdentifier _appIdentifier;

        public SignalRBackplaneTransport(IServiceProvider serviceProvider, InitializeParams initializeParams, Func<Uri> urlProvider)
        {
            Uri backplaneUrl = urlProvider();
            _logger = serviceProvider.GetRequiredService<ILogger<IBackplaneTransport>>();
            _appIdentifier = initializeParams.AppIdentifier;
            _hubConnection = new HubConnectionBuilder().WithUrl(backplaneUrl)
                 .AddNewtonsoftJsonProtocol()
                  .ConfigureLogging(logging =>
                  {
                      logging.AddProvider(_logger.AsLoggerProvider());
                  }).WithAutomaticReconnect().Build();
            _logger.LogInformation($"Creating connection object with url:{backplaneUrl}");
        }

        public async Task<AppIdentifier> ConnectAsync(Action<MessageEnvelope> onMessage, Func<Exception, Task> onDisconnect, CancellationToken ct = default)
        {
            _hubConnection.On("OnMessage", onMessage);
            _hubConnection.Closed += onDisconnect;
            await _hubConnection.StartAsync();
            return _appIdentifier;
        }



        public async Task<IEnumerable<Channel>> GetUserChannelsAsync(CancellationToken ct = default)
        {
            if (_hubConnection.State != HubConnectionState.Connected)
            {
                throw new InvalidOperationException(MSG_CONNECTION_CLOSED);
            }
            return await _hubConnection.InvokeAsync<IEnumerable<Channel>>("GetUserChannels", ct);
        }

        public async Task BroadcastAsync(MessageEnvelope message, CancellationToken ct = default)
        {
            if (_hubConnection.State != HubConnectionState.Connected)
            {
                throw new InvalidOperationException(MSG_CONNECTION_CLOSED);
            }
            await _hubConnection.InvokeAsync("Broadcast", message, ct);
            _logger.LogInformation($"Broadcast successfull for message: {JsonSerializer.Serialize(message)}");
        }




        public async ValueTask DisposeAsync()
        {
            await _hubConnection.DisposeAsync();
        }

    }
}

