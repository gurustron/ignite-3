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

using System.Diagnostics.CodeAnalysis;
using Common.Table;
using Ignite.Table;
using NodaTime;

using static Common.Table.TestTables;

/// <summary>
/// Basic LINQ provider tests.
/// </summary>
[SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Tests")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Tests")]
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "Tests")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "Tests")]
public class LinqTests : IgniteTestsBase
{
    // ReSharper disable once UnusedMember.Local
#pragma warning disable CA1823
    private const int Count = 10;
#pragma warning restore CA1823

    private IRecordView<PocoByte> PocoByteView { get; set; } = null!;

    private IRecordView<PocoShort> PocoShortView { get; set; } = null!;

    private IRecordView<PocoInt> PocoIntView { get; set; } = null!;

    private IRecordView<PocoIntEnum> PocoIntEnumView { get; set; } = null!;

    private IRecordView<PocoLong> PocoLongView { get; set; } = null!;

    private IRecordView<PocoFloat> PocoFloatView { get; set; } = null!;

    private IRecordView<PocoDouble> PocoDoubleView { get; set; } = null!;

    private IRecordView<PocoDecimal> PocoDecimalView { get; set; } = null!;

    private IRecordView<PocoBigDecimal> PocoBigDecimalView { get; set; } = null!;

    private IRecordView<PocoString> PocoStringView { get; set; } = null!;

    [OneTimeSetUp]
    public async Task InsertData()
    {
        await Task.Yield();

        // var tableNames = new[]
        // {
        //     TableName, TableDateTimeName, TableDoubleName, TableFloatName, TableDecimalName, TableInt8Name,
        //     TableInt16Name, TableInt32Name, TableInt64Name
        // };
        //
        // foreach (var tableName in tableNames)
        // {
        //     await Client.Sql.ExecuteAsync(null, "delete from " + tableName);
        // }
        //
        // PocoByteView = (await Client.Tables.GetTableAsync(TableInt8Name))!.GetRecordView<PocoByte>();
        // PocoShortView = (await Client.Tables.GetTableAsync(TableInt16Name))!.GetRecordView<PocoShort>();
        // PocoIntView = (await Client.Tables.GetTableAsync(TableInt32Name))!.GetRecordView<PocoInt>();
        // PocoIntEnumView = (await Client.Tables.GetTableAsync(TableInt32Name))!.GetRecordView<PocoIntEnum>();
        // PocoLongView = (await Client.Tables.GetTableAsync(TableInt64Name))!.GetRecordView<PocoLong>();
        // PocoFloatView = (await Client.Tables.GetTableAsync(TableFloatName))!.GetRecordView<PocoFloat>();
        // PocoDoubleView = (await Client.Tables.GetTableAsync(TableDoubleName))!.GetRecordView<PocoDouble>();
        // PocoStringView = (await Client.Tables.GetTableAsync(TableStringName))!.GetRecordView<PocoString>();
        //
        // var tableDecimal = await Client.Tables.GetTableAsync(TableDecimalName);
        // PocoDecimalView = tableDecimal!.GetRecordView<PocoDecimal>();
        // PocoBigDecimalView = tableDecimal.GetRecordView<PocoBigDecimal>();
        //
        // for (int i = 0; i < Count; i++)
        // {
        //     await PocoView.UpsertAsync(null, new Poco { Key = i, Val = "v-" + i });
        //
        //     await PocoByteView.UpsertAsync(null, new PocoByte((sbyte)i, (sbyte)(i / 3)));
        //     await PocoShortView.UpsertAsync(null, new PocoShort((short)(i * 2), (short)(i * 2)));
        //     await PocoIntView.UpsertAsync(null, new PocoInt(i, i * 100));
        //     await PocoLongView.UpsertAsync(null, new PocoLong(i, i * 2));
        //
        //     await PocoFloatView.UpsertAsync(null, new(i, i));
        //     await PocoDoubleView.UpsertAsync(null, new(i, i));
        //     await PocoDecimalView.UpsertAsync(null, new(i, i));
        //
        //     await PocoStringView.UpsertAsync(null, new("k-" + i, "v-" + i * 2));
        //
        //     var pocoAllColumns = new PocoAllColumnsSqlNullable(
        //         i,
        //         "v -" + i,
        //         (sbyte)(i + 1),
        //         (short)(i + 2),
        //         i + 3,
        //         i + 4,
        //         i + 5.5f,
        //         i + 6.5,
        //         new LocalDate(2022, 12, i + 1),
        //         new LocalTime(11, 38, i + 1),
        //         new LocalDateTime(2022, 12, 19, 11, i + 1),
        //         Instant.FromUnixTimeSeconds(i + 1),
        //         new byte[] { 1, 2 },
        //         i + 7.7m,
        //         new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, (byte)(i + 1)),
        //         i % 2 == 0);
        //
        //     await PocoAllColumnsSqlNullableView.UpsertAsync(null, pocoAllColumns);
        // }
        //
        // await PocoAllColumnsSqlNullableView.UpsertAsync(null, new PocoAllColumnsSqlNullable(100));
    }

    [OneTimeTearDown]
    public async Task CleanTables()
    {
        await Task.Yield();

        // await TupleView.DeleteAllAsync(null, Enumerable.Range(0, Count).Select(x => GetTuple(x)));
        // await PocoIntView.DeleteAllAsync(null, Enumerable.Range(0, Count).Select(x => new PocoInt(x, 0)));
    }

    [Test]
    public void TestSelectOneColumn()
    {
        var query = PocoView.AsQueryable()
            .Where(x => x.Key == 3)
            .Select(x => x.Val);

        string?[] res = query.ToArray();

        CollectionAssert.AreEqual(new[] { "v-3" }, res);
    }

    [Test]
    public void TestContains()
    {
        var keys = new long[] { 4, 2 };

        // var query = PocoView.AsQueryable()
        //     .Where(x => Enumerable.Contains(keys, x.Key))
        //     .Select(x => x.Val);

        // System.MemoryExtensions.Contains()
        var query = PocoView.AsQueryable()
            .Where(x => keys.Contains(x.Key))
            .Select(x => x.Val);

        List<string?> res = query.ToList();

        CollectionAssert.AreEquivalent(new[] { "v-2", "v-4" }, res);

        StringAssert.Contains(
            "select _T0.VAL from PUBLIC.TBL1 as _T0 where (_T0.KEY IN (?, ?)), Parameters = [ 4, 2 ]",
            query.ToString());
    }

#pragma warning disable CA1711
#pragma warning disable SA1201
    private enum TestEnum
#pragma warning restore SA1201
#pragma warning restore CA1711
    {
        None = 0,
        A = 100,
        B = 300
    }

    private record PocoByte(sbyte Key, sbyte? Val);

    private record PocoShort(short Key, short? Val);

    private record PocoInt(int Key, int? Val);

    private record PocoLong(long Key, long? Val);

    private record PocoFloat(float Key, float? Val);

    private record PocoDouble(double Key, double? Val);

    private record PocoDecimal(decimal Key, decimal? Val);

    private record PocoBigDecimal(BigDecimal Key, BigDecimal? Val);

    private record PocoString(string Key, string? Val);

    private record PocoDate(LocalDate Key, LocalDate? Val);

    private record PocoTime(LocalTime Key, LocalTime? Val);

    private record PocoDateTime(LocalDateTime Key, LocalDateTime? Val);

    private record PocoIntEnum(int Key, TestEnum? Val);
}
