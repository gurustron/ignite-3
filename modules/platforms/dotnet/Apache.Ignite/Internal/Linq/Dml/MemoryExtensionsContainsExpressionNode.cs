/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements. See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace Apache.Ignite.Internal.Linq.Dml;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

/// <summary>
/// System.MemoryExtensions.Contains().
/// </summary>
public class MemoryExtensionsContainsExpressionNode : ResultOperatorExpressionNodeBase
{
    /// <summary>
    /// ExecuteUpdate method.
    /// </summary>
    public static readonly MethodInfo MethodInfo = typeof(MemoryExtensions)
        .GetMethod(nameof(MemoryExtensions.Contains), [
            typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
            Type.MakeGenericMethodParameter(0)
        ])!;

    /// <summary>
    /// All supported methods.
    /// </summary>
    public static readonly IReadOnlyList<MethodInfo> MethodInfos = [MethodInfo];

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryExtensionsContainsExpressionNode" /> class.
    /// </summary>
    /// <param name="parseInfo">Parse information.</param>
    /// <param name="optionalPredicate">TODO.</param>
    /// <param name="optionalSelector">TODO 1.</param>
    public MemoryExtensionsContainsExpressionNode(
        MethodCallExpressionParseInfo parseInfo,
        LambdaExpression optionalPredicate,
        LambdaExpression optionalSelector)
        : base(parseInfo, optionalPredicate, optionalSelector)
    {
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override Expression Resolve(
        ParameterExpression inputParameter,
        Expression expressionToBeResolved,
        ClauseGenerationContext clauseGenerationContext)
    {
        throw this.CreateResolveNotSupportedException();
    }

    /// <inheritdoc />
    protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
    {
        throw new System.NotImplementedException();
    }
}
