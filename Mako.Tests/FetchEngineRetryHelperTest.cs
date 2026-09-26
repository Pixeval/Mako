// Copyright (c) Mako.Tests.
// Licensed under the GPL-3.0 License.

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mako.Model;
using Mako.Utilities;
using Mako.Net.Responses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApiClientCore.Exceptions;

namespace Mako.Tests;

[TestClass]
public sealed class FetchEngineRetryHelperTest
{
    [TestMethod]
    [DataRow(HttpStatusCode.TooManyRequests)]
    [DataRow(HttpStatusCode.ServiceUnavailable)]
    public async Task RetriesTransientResponse(HttpStatusCode status)
    {
        using var response = new HttpResponseMessage(status);
        var expected = new UgoiraMetadata { ZipUrls = new() { Medium = "https://example.com/ugoira.zip" }, Frames = [] };
        var attempts = 0;
        var result = await FetchEngineRetryHelper.ExecuteAsync(_ => ++attempts is 1
            ? Task.FromException<UgoiraMetadataResponse>(new ApiResponseStatusException(response))
            : Task.FromResult(new UgoiraMetadataResponse { Content = expected }), static _ => TimeSpan.Zero);

        Assert.AreSame(expected, result.Content);
        Assert.AreEqual(2, attempts);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task RetriesTransportFailure(bool timeout)
    {
        Exception failure = timeout
            ? new TaskCanceledException("HTTP timeout", new TimeoutException())
            : new HttpRequestException("Disconnected");
        var expected = new UgoiraMetadata { ZipUrls = new() { Medium = "https://example.com/ugoira.zip" }, Frames = [] };
        var attempts = 0;
        var result = await FetchEngineRetryHelper.ExecuteAsync(_ => ++attempts is 1
            ? Task.FromException<UgoiraMetadataResponse>(failure)
            : Task.FromResult(new UgoiraMetadataResponse { Content = expected }), static _ => TimeSpan.Zero);

        Assert.AreSame(expected, result.Content);
        Assert.AreEqual(2, attempts);
    }

    [TestMethod]
    public async Task PreservesPermanentFailure()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        var expected = new ApiResponseStatusException(response);
        var attempts = 0;
        var actual = await Assert.ThrowsExactlyAsync<ApiResponseStatusException>(() =>
            FetchEngineRetryHelper.ExecuteAsync(_ =>
            {
                attempts++;
                return Task.FromException<UgoiraMetadataResponse>(expected);
            }));

        Assert.AreSame(expected, actual);
        Assert.AreEqual(1, attempts);
    }

    [TestMethod]
    public async Task CancellationInterruptsRetryDelay()
    {
        using var cancellation = new CancellationTokenSource();
        var failed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var attempts = 0;
        var task = FetchEngineRetryHelper.ExecuteAsync(token =>
        {
            Assert.AreEqual(cancellation.Token, token);
            attempts++;
            failed.SetResult();
            return Task.FromException<UgoiraMetadataResponse>(new HttpRequestException("Disconnected"));
        }, token: cancellation.Token);

        await failed.Task;
        await cancellation.CancelAsync();
        _ = await Assert.ThrowsAsync<OperationCanceledException>(() => task.WaitAsync(TimeSpan.FromSeconds(5)));
        Assert.AreEqual(1, attempts);
    }

    [TestMethod]
    public async Task CancelledRequestDoesNotStart()
    {
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();
        var attempts = 0;
        _ = await Assert.ThrowsAsync<OperationCanceledException>(() => FetchEngineRetryHelper.ExecuteAsync(_ =>
        {
            attempts++;
            return Task.FromResult(new UgoiraMetadataResponse { Content = null! });
        }, token: cancellation.Token));
        Assert.AreEqual(0, attempts);
    }
}
