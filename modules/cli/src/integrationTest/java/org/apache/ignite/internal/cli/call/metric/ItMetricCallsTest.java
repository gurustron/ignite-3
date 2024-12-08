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

package org.apache.ignite.internal.cli.call.metric;

import static org.assertj.core.api.Assertions.assertThat;

import jakarta.inject.Inject;
import java.util.List;
import java.util.stream.Collectors;
import org.apache.ignite.internal.cli.CliIntegrationTest;
import org.apache.ignite.internal.cli.call.cluster.metric.ClusterMetricSourceEnableCall;
import org.apache.ignite.internal.cli.call.cluster.metric.ClusterMetricSourceListCall;
import org.apache.ignite.internal.cli.call.node.metric.NodeMetricSetListCall;
import org.apache.ignite.internal.cli.call.node.metric.NodeMetricSourceEnableCall;
import org.apache.ignite.internal.cli.call.node.metric.NodeMetricSourceListCall;
import org.apache.ignite.internal.cli.core.call.CallOutput;
import org.apache.ignite.internal.cli.core.call.UrlCallInput;
import org.apache.ignite.rest.client.model.MetricSet;
import org.apache.ignite.rest.client.model.MetricSource;
import org.apache.ignite.rest.client.model.NodeMetricSources;
import org.assertj.core.api.ThrowingConsumer;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.ValueSource;

/** Tests for metrics calls. */
class ItMetricCallsTest extends CliIntegrationTest {
    private final UrlCallInput urlInput = new UrlCallInput(NODE_URL);

    @Inject
    NodeMetricSourceListCall nodeMetricSourceListCall;

    @Inject
    ClusterMetricSourceListCall clusterMetricSourceListCall;

    @Inject
    NodeMetricSetListCall nodeMetricSetListCall;

    @Inject
    NodeMetricSourceEnableCall nodeMetricSourceEnableCall;

    @Inject
    ClusterMetricSourceEnableCall clusterMetricSourceEnableCall;

    @Test
    @DisplayName("Should display metric sources when cluster is up and running")
    void nodeMetricSourcesList() {
        // When
        CallOutput<List<MetricSource>> output = nodeMetricSourceListCall.execute(urlInput);

        // Then
        assertThat(output.hasError()).isFalse();

        // And
        assertThat(output.body()).contains(ALL_METRIC_SOURCES);
        assertThat(output.body()).hasSize(ALL_METRIC_SOURCES.length);
    }

    @Test
    @DisplayName("Should display all metric sources when cluster is up and running")
    void clusterMetricSourcesList() {
        // When
        CallOutput<List<NodeMetricSources>> output = clusterMetricSourceListCall.execute(urlInput);

        // Then
        assertThat(output.hasError()).isFalse();

        // And
        //noinspection unchecked
        ThrowingConsumer<NodeMetricSources>[] assertions = CLUSTER.runningNodes()
                .map(ignite -> (ThrowingConsumer<NodeMetricSources>) input -> {
                    assertThat(input.getNode()).isEqualTo(ignite.name());
                    assertThat(input.getSources()).containsExactlyInAnyOrder(ALL_METRIC_SOURCES);
                })
                .toArray(ThrowingConsumer[]::new);

        assertThat(output.body()).satisfiesExactlyInAnyOrder(assertions);
    }

    @Test
    @DisplayName("Should display metric sets when cluster is up and running")
    void nodeMetricSetsListContainsAllMetrics() {
        // When
        CallOutput<List<MetricSet>> metricSetsOutput = nodeMetricSetListCall.execute(urlInput);
        CallOutput<List<MetricSource>> metricSourcesOutput = nodeMetricSourceListCall.execute(urlInput);

        // Then
        assertThat(metricSetsOutput.hasError()).isFalse();
        assertThat(metricSourcesOutput.hasError()).isFalse();

        // And
        List<String> allMetricsSource =
                metricSourcesOutput.body().stream().map(MetricSource::getName).collect(Collectors.toList());
        List<String> enabledMetrics =
                metricSetsOutput.body().stream().map(MetricSet::getName).collect(Collectors.toList());

        assertThat(allMetricsSource).isNotEmpty();
        assertThat(enabledMetrics).isNotEmpty();

        // Since all metrics are enabled by default, we must observe metric sets from all metric sources.
        assertThat(enabledMetrics).containsAll(allMetricsSource);
    }

    @ParameterizedTest
    @ValueSource(booleans = {false, true})
    void nodeMetricEnable(boolean enabled) {
        // Given
        var input = MetricSourceEnableCallInput.builder()
                .endpointUrl(NODE_URL)
                .srcName("no.such.metric")
                .enable(enabled)
                .build();

        // When
        CallOutput<String> output = nodeMetricSourceEnableCall.execute(input);

        // Then
        assertThat(output.hasError()).isTrue();
    }

    @ParameterizedTest
    @ValueSource(booleans = {false, true})
    void clusterMetricEnable(boolean enabled) {
        // Given
        var input = MetricSourceEnableCallInput.builder()
                .endpointUrl(NODE_URL)
                .srcName("no.such.metric")
                .enable(enabled)
                .build();

        // When
        CallOutput<String> output = clusterMetricSourceEnableCall.execute(input);

        // Then
        assertThat(output.hasError()).isTrue();
    }
}
