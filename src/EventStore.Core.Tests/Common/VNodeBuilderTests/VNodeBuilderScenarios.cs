using EventStore.Core.Cluster.Settings;
using EventStore.Core.TransactionLog.Chunks;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using EventStore.Core.Tests.Services.Transport.Tcp;

namespace EventStore.Core.Tests.Common.VNodeBuilderTests {
	[TestFixture]
	public abstract class SingleNodeScenario {
		protected VNodeBuilder _builder;
		protected ClusterVNode _node;
		protected ClusterVNodeSettings _settings;
		protected TFChunkDbConfig _dbConfig;

		[OneTimeSetUp]
		public virtual void TestFixtureSetUp() {
			_builder = TestVNodeBuilder.AsSingleNode()
				.WithServerCertificate(ssl_connections.GetServerCertificate())
				.RunInMemory();
			Given();
			_node = _builder.Build();
			_settings = ((TestVNodeBuilder)_builder).GetSettings();
			_dbConfig = ((TestVNodeBuilder)_builder).GetDbConfig();
			_node.Start();
		}

		[OneTimeTearDown]
		public virtual async Task TestFixtureTearDown() {
			try {
				await _node.StopAsync();
			} catch (OperationCanceledException) {

			}
		}

		public abstract void Given();
	}

	[TestFixture, Category("LongRunning")]
	public abstract class ClusterMemberScenario {
		protected VNodeBuilder _builder;
		protected ClusterVNode _node;
		protected ClusterVNodeSettings _settings;
		protected TFChunkDbConfig _dbConfig;
		protected int _clusterSize = 3;
		protected int _quorumSize;

		[OneTimeSetUp]
		public virtual void TestFixtureSetUp() {
			_builder = TestVNodeBuilder.AsClusterMember(_clusterSize)
				.WithServerCertificate(ssl_connections.GetServerCertificate())
				.RunInMemory();
			_quorumSize = _clusterSize / 2 + 1;
			Given();
			_node = _builder.Build();
			_settings = ((TestVNodeBuilder)_builder).GetSettings();
			_dbConfig = ((TestVNodeBuilder)_builder).GetDbConfig();
			_node.Start();
		}

		[OneTimeTearDown]
		public virtual async Task TestFixtureTearDown() {
			await _node.StopAsync();
		}

		public abstract void Given();
	}
}
