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

package org.apache.ignite.internal.metastorage.impl;

import java.util.concurrent.CompletableFuture;
import java.util.concurrent.locks.ReentrantLock;
import org.apache.ignite.internal.lang.NodeStoppingException;
import org.apache.ignite.internal.metastorage.Revisions;
import org.apache.ignite.internal.metastorage.server.RecoveryRevisionsListener;
import org.apache.ignite.internal.util.IgniteSpinBusyLock;

/** Implementation of {@link RecoveryRevisionsListener}. */
class RecoveryRevisionsListenerImpl implements RecoveryRevisionsListener {
    private final IgniteSpinBusyLock busyLock;

    private final CompletableFuture<Revisions> recoveryFinishFuture;

    private final ReentrantLock lock = new ReentrantLock();

    /** Guarded by {@link #lock}. */
    private Revisions targetRevisions;

    /** Guarded by {@link #lock}. */
    private Revisions currentRevisions;

    RecoveryRevisionsListenerImpl(
            IgniteSpinBusyLock busyLock,
            CompletableFuture<Revisions> recoveryFinishFuture
    ) {
        this.busyLock = busyLock;
        this.recoveryFinishFuture = recoveryFinishFuture;
    }

    @Override
    public void onUpdate(Revisions currentRevisions) {
        lock.lock();

        try {
            this.currentRevisions = currentRevisions;

            completeRecoveryFinishFutureIfPossible();
        } finally {
            lock.unlock();
        }
    }

    void setTargetRevisions(Revisions targetRevisions) {
        lock.lock();

        try {
            this.targetRevisions = targetRevisions;

            completeRecoveryFinishFutureIfPossible();
        } finally {
            lock.unlock();
        }
    }

    private void completeRecoveryFinishFutureIfPossible() {
        if (!busyLock.enterBusy()) {
            recoveryFinishFuture.completeExceptionally(new NodeStoppingException());
        }

        try {
            if (targetRevisions == null
                    || currentRevisions == null
                    || currentRevisions.revision() < targetRevisions.revision()
                    || currentRevisions.compactionRevision() < targetRevisions.compactionRevision()) {
                return;
            }

            recoveryFinishFuture.complete(currentRevisions);
        } catch (Throwable t) {
            recoveryFinishFuture.completeExceptionally(t);
        } finally {
            busyLock.leaveBusy();
        }
    }
}
