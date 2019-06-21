using CleverCrow.Fluid.Dialogues.Builders;
using NSubstitute;
using NUnit.Framework;

namespace CleverCrow.Fluid.Dialogues.Nodes {
    public class NodeLinkTest {
        private INode _child;
        private NodeLink _link;

        [SetUp]
        public void BeforeEach () {
            _child = A.Node.Build();
            _link = new NodeLink(_child);
        }

        public class IsValidProperty : NodeLinkTest {
            [Test]
            public void It_should_return_the_child_IsValid_status () {
                _child.IsValid.Returns(true);

                Assert.IsTrue(_link.IsValid);
            }
        }

        public class NextMethod : NodeLinkTest {
            [Test]
            public void It_should_return_its_child () {
                _child.IsValid.Returns(true);

                Assert.AreEqual(_child, _link.Next());
            }

            [Test]
            public void It_should_not_return_its_child_if_invalid () {
                _child.IsValid.Returns(false);

                Assert.IsNull(_link.Next());
            }
        }

        public class PlayMethod : NodeLinkTest {
            [Test]
            public void It_should_trigger_next_on_playback () {
                var playback = Substitute.For<IDialoguePlayback>();

                _link.Play(playback);

                playback.Received(1).Next();
            }
        }
    }
}
