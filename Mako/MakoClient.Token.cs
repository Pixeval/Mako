// Copyright (c) Mako.
// Licensed under the GPL-3.0 License.

using System;
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
    public void SetToken(string? refreshToken)
    {
        ObjectDisposedException.ThrowIf(Status is ClientStatus.Disposed, this);
        if (Status is ClientStatus.Built)
            CancelAll();
        var tokenOption = Provider.GetRequiredService<RefreshTokenOption>();
        tokenOption.RefreshToken = refreshToken;
        var tokenProvider = GetTokenProvider();
        tokenProvider.ClearToken();
        SetTokenInternal(null);
        if (refreshToken is not null)
            Status = ClientStatus.Built;
    }

    /// <summary>
    /// 关闭所有正在运行的实例并删除 Token（切换账号）
    /// </summary>
    public void ClearToken()
    {
        if (Status is not ClientStatus.Created)
            SetToken(null);
    }

    /// <summary>
    /// 测试当前 Token 是否有效
    /// </summary>
    public async Task<bool> IdentifyTokenAsync()
    {
        EnsureBuilt();
        if (Status is ClientStatus.Created)
            throw new InvalidOperationException("Token not set");
        var tokenProvider = GetTokenProvider();
        try
        {
            _ = await tokenProvider.GetTokenAsync();
            return true;
        }
        catch (Exception e)
        {
            LogException(e);
            return false;
        }
    }

    internal void SetTokenInternal(TokenResponse? response)
    {
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
