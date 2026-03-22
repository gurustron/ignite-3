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

namespace Apache.Ignite.Tests.NET10.Linq;

using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Common.Table;
using Ignite.Table;
using NodaTime;
using Sql;

using static Common.Table.TestTables;

/// <summary>
/// Basic LINQ provider tests.
/// </summary>
[SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Tests")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Tests")]
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "Tests")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "Tests")]
public class FakeServerLinqTests
{
    private IIgniteClient _client = null!;
    private FakeServer _server = null!;
    private ITable _table = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _server = new FakeServer(false);
        _client = await _server.ConnectClientAsync();
        _table = (await _client.Tables.GetTableAsync(FakeServer.ExistingTableName))!;
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _client.Dispose();
        _server.Dispose();

        TestUtils.CheckByteArrayPoolLeak();
    }

    [Test]
    public void TestSelectOneColumn() =>
        AssertSql("select _T0.KEY from PUBLIC.TBL1 as _T0", q => q.Select(x => x.Key).ToList());

    [Test]
    public void TestContains() =>
        AssertSql("select (_T0.KEY IN (?, ?)) from PUBLIC.TBL1 as _T0", q =>
        {
            var keys = new long[] { 4, 2 };
            return q.Select(x => keys.Contains(x.Key)).ToList();
        });

    // [Test]
    // public void TestContainsComparer() =>
    //     AssertSql("select (_T0.KEY IN (?, ?)) from PUBLIC.TBL1 as _T0", q =>
    //     {
    //         IEnumerable<string> vals = new[] { "42" };
    //         return q.Select(x => vals.Contains(x.Val, StringComparer.OrdinalIgnoreCase)).ToList();
    // });
    private void AssertSql(string expectedSql, Func<IQueryable<Poco>, object?> query) =>
        AssertSql(expectedSql, t => query(t.GetRecordView<Poco>().AsQueryable()));

    private void AssertSql(string expectedSql, Func<ITable, object?> query)
    {
        _server.LastSql = string.Empty;
        Exception? ex = null;

        try
        {
            query(_table);
        }
        catch (Exception e)
        {
            // Ignore.
            // Result deserialization may fail because FakeServer returns one column always.
            // We are only interested in the generated SQL.
            ex = e;
        }

        Assert.AreEqual(expectedSql, _server.LastSql, string.IsNullOrEmpty(_server.LastSql) ? ex?.ToString() : _server.LastSql);
    }

    // ReSharper disable NotAccessedPositionalProperty.Local, ClassNeverInstantiated.Local
    private record OneColumnPoco(long Key);

    private record EmptyPoco;

    private record UnmappedPoco([property: NotMapped] long Key, [field: NotMapped] string Val);
}
