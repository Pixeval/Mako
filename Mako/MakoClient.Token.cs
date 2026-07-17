// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Mako.Model;
using Mako.Net;
using Mako.Net.EndPoints;
using Microsoft.Extensions.DependencyInjection;
using WebApiClientCore.Extensions.OAuths;

namespace Mako;

public partial class MakoClient
{
    /// <summary>
    /// 关闭所有正在运行的实例并更换 Token（切换账号）
    /// </summary>
    /// <remarks>
    /// 仅设置，不验证 Token 是否有效或刷新 Token，可以手动调用 <see cref="IdentifyTokenAsync"/> 来验证 Token 是否有效
    /// </remarks>
    public async Task SetTokenAsync(string? refreshToken, CancellationToken token = default)
    {
        ObjectDisposedException.ThrowIf(Status is ClientStatus.Disposed, this);
        if (Status is ClientStatus.Built)
            CancelAll();
        var tokenOption = Provider.GetRequiredService<RefreshTokenOption>();
        tokenOption.RefreshToken = refreshToken;
        await ClearTokenInternalAsync(token);
        if (refreshToken is not null)
            Status = ClientStatus.Built;
    }

    /// <inheritdoc cref="SetTokenAsync" />
    public async Task SetCodeAsync(string code, string codeVerifier, CancellationToken token = default)
    {
        ObjectDisposedException.ThrowIf(Status is ClientStatus.Disposed, this);
        if (Status is ClientStatus.Built)
            CancelAll();
        var tokenOption = Provider.GetRequiredService<RefreshTokenOption>();
        tokenOption.RefreshToken = null;
        tokenOption.Code = code;
        tokenOption.CodeVerifier = codeVerifier;
        await ClearTokenInternalAsync(token);
        Status = ClientStatus.Built;
    }

    /// <summary>
    /// 关闭所有正在运行的实例并删除 Token
    /// </summary>
    public async Task ClearTokenAsync(CancellationToken token = default)
    {
        if (Status is not ClientStatus.Created)
            await SetTokenAsync(null, token);
    }

    /// <summary>
    /// 测试当前 Token 是否有效
    /// </summary>
    public async Task<bool> IdentifyTokenAsync(CancellationToken token = default)
    {
        EnsureBuilt();
        if (Status is ClientStatus.Created)
            throw new InvalidOperationException("Token not set");
        var tokenProvider = GetTokenProvider();
        try
        {
            _ = await tokenProvider.GetTokenAsync(token);
            return true;
        }
        catch (Exception e)
        {
            LogException(e);
            return false;
        }
    }

    private async Task ClearTokenInternalAsync(CancellationToken token = default)
    {
        var tokenProvider = GetTokenProvider();
        await tokenProvider.ClearTokenAsync(token);
        SetTokenInternal(null);
    }

    internal void SetTokenInternal(TokenResponse? response)
    {
        if (Me is null && response is null)
            return;
        Me = response?.User;
        OnTokenRefreshed(response);
    }

    internal ITokenProvider GetTokenProvider()
    {
        var tokenProviderFactory = Provider.GetRequiredService<ITokenProviderFactory>();
        var tokenProvider = tokenProviderFactory.Create(typeof(IAppApiEndPoint), TypeMatchMode.TypeOrBaseTypes);
        return tokenProvider;
    }
}
