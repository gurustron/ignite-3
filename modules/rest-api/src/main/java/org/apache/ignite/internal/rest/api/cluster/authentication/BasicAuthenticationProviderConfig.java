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

package org.apache.ignite.internal.rest.api.cluster.authentication;

import com.fasterxml.jackson.annotation.JsonCreator;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.annotation.JsonTypeName;
import io.swagger.v3.oas.annotations.media.Schema;
import org.apache.ignite.internal.util.StringUtils;
import org.apache.ignite.security.AuthenticationType;

/**
 * REST representation of {@link BasicAuthenticationProviderConfig}.
 */
@JsonTypeName("basic")
@Schema(name = "BasicAuthenticationProviderConfig", description = "Configuration for basic authentication.")
public class BasicAuthenticationProviderConfig implements AuthenticationProviderConfig {

    @Schema(description = "Provider name.")
    private final String name;

    @Schema(description = "Username.")
    private final String username;

    @Schema(description = "Password.")
    private final String password;

    /** Constructor. */
    @JsonCreator
    public BasicAuthenticationProviderConfig(
            @JsonProperty("name") String name,
            @JsonProperty("username") String username,
            @JsonProperty("password") String password) {
        if (StringUtils.nullOrBlank(name)) {
            throw new IllegalArgumentException("Name must not be empty.");
        }

        if (StringUtils.nullOrBlank(username)) {
            throw new IllegalArgumentException("Username must not be empty.");
        }

        if (StringUtils.nullOrBlank(password)) {
            throw new IllegalArgumentException("Password must not be empty.");
        }

        this.name = name;
        this.username = username;
        this.password = password;
    }

    @JsonProperty
    public String username() {
        return username;
    }

    @JsonProperty
    public String password() {
        return password;
    }

    @JsonProperty
    @Override
    @Schema(description = "Authentication type to use.")
    public AuthenticationType type() {
        return AuthenticationType.BASIC;
    }

    @JsonProperty
    @Override
    public String name() {
        return name;
    }
}