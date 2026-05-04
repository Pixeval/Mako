// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

// 不支持源生成器的Runtime中，WebApiClient会降级到IL代理，此时网络接口不能使用internal。本项目面向.NET 10所以直接抑制
[assembly: SuppressMessage("Error", "WA1007:不支持的修饰符", Justification = "<挂起>")]
