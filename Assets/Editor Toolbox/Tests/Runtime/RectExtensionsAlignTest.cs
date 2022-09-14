using NUnit.Framework;
using UnityEngine;

namespace Toolbox.Tests
{
    public class RectExtensionsAlignTest
    {
        public class AlignTestData
        {
            public float alignValue;
            public Rect sourceRect;
            public Rect actualRect;

            public override string ToString()
            {
                return sourceRect.ToString();
            }
        }

        private static readonly AlignTestData[] alignRightData = new AlignTestData[]
        {
            new AlignTestData()
            {
                alignValue = 20,
                sourceRect = new Rect(0, 0, 100, 500),
                actualRect = new Rect(80, 0, 20, 500)
            },
            new AlignTestData()
            {
                alignValue = 0,
                sourceRect = new Rect(0, 0, 100, 500),
                actualRect = new Rect(100, 0, 0, 500)
            },
            new AlignTestData()
            {
                alignValue = 50,
                sourceRect = new Rect(15, 100, 100, 500),
                actualRect = new Rect(65, 100, 50, 500)
            },
            new AlignTestData()
            {
                alignValue = 100,
                sourceRect = new Rect(15, 100, 100, 50),
                actualRect = new Rect(15, 100, 100, 50)
            },
            new AlignTestData()
            {
                alignValue = -50,
                sourceRect = new Rect(0, 0, 100, 500),
                actualRect = new Rect(150, 0, -50, 500)
            }
        };

        private static readonly AlignTestData[] alignLeftData = new AlignTestData[]
        {
            new AlignTestData()
            {
                alignValue = 20,
                sourceRect = new Rect(0, 0, 100, 500),
                actualRect = new Rect(0, 0, 20, 500)
            },
            new AlignTestData()
            {
                alignValue = 0,
                sourceRect = new Rect(0, 0, 100, 500),
                actualRect = new Rect(0, 0, 0, 500)
            },
            new AlignTestData()
            {
                alignValue = 50,
                sourceRect = new Rect(15, 100, 100, 500),
                actualRect = new Rect(15, 100, 50, 500)
            },
            new AlignTestData()
            {
                alignValue = 100,
                sourceRect = new Rect(15, 100, 100, 50),
                actualRect = new Rect(15, 100, 100, 50)
            },
            new AlignTestData()
            {
                alignValue = -50,
                sourceRect = new Rect(0, 0, 100, 500),
                actualRect = new Rect(0, 0, -50, 500)
            }
        };

        private static readonly AlignTestData[] alignBottomData = new AlignTestData[]
        {
            new AlignTestData()
            {
                alignValue = 20,
                sourceRect = new Rect(0, 0, 100, 500),
                actualRect = new Rect(0, 480, 100, 20)
            },
            new AlignTestData()
            {
                alignValue = 0,
                sourceRect = new Rect(0, 0, 100, 500),
                actualRect = new Rect(0, 500, 100, 0)
            },
            new AlignTestData()
            {
                alignValue = 50,
                sourceRect = new Rect(15, 100, 100, 500),
                actualRect = new Rect(15, 550, 100, 50)
            },
            new AlignTestData()
            {
                alignValue = 50,
                sourceRect = new Rect(15, 100, 100, 50),
                actualRect = new Rect(15, 100, 100, 50)
            },
            new AlignTestData()
            {
                alignValue = -50,
                sourceRect = new Rect(0, 0, 100, 500),
                actualRect = new Rect(0, 550, 100, -50)
            }
        };

        private static readonly AlignTestData[] alignTopData = new AlignTestData[]
        {
            new AlignTestData()
            {
                alignValue = 20,
                sourceRect = new Rect(0, 0, 100, 500),
                actualRect = new Rect(0, 0, 100, 20)
            },
            new AlignTestData()
            {
                alignValue = 0,
                sourceRect = new Rect(0, 0, 100, 500),
                actualRect = new Rect(0, 0, 100, 0)
            },
            new AlignTestData()
            {
                alignValue = 50,
                sourceRect = new Rect(15, 100, 100, 500),
                actualRect = new Rect(15, 100, 100, 50)
            },
            new AlignTestData()
            {
                alignValue = 50,
                sourceRect = new Rect(15, 100, 100, 50),
                actualRect = new Rect(15, 100, 100, 50)
            },
            new AlignTestData()
            {
                alignValue = -50,
                sourceRect = new Rect(0, 0, 100, 500),
                actualRect = new Rect(0, 0, 100, -50)
            }
        };

        private static readonly AlignTestData[] alignCenterXData = new AlignTestData[]
        {
            new AlignTestData()
            {
                alignValue = 20,
                sourceRect = new Rect(0, 0, 100, 500),
                actualRect = new Rect(40, 0, 20, 500)
            },
            new AlignTestData()
            {
                alignValue = 0,
                sourceRect = new Rect(0, 0, 100, 500),
                actualRect = new Rect(50, 0, 0, 500)
            },
            new AlignTestData()
            {
                alignValue = 50,
                sourceRect = new Rect(15, 100, 100, 500),
                actualRect = new Rect(40, 100, 50, 500)
            },
            new AlignTestData()
            {
                alignValue = 50,
                sourceRect = new Rect(15, 100, 100, 50),
                actualRect = new Rect(40, 100, 50, 50)
            },
            new AlignTestData()
            {
                alignValue = -50,
                sourceRect = new Rect(0, 0, 100, 500),
                actualRect = new Rect(75, 0, -50, 500)
            }
        };

        private static readonly AlignTestData[] alignCenterYData = new AlignTestData[]
        {
            new AlignTestData()
            {
                alignValue = 20,
                sourceRect = new Rect(0, 0, 100, 500),
                actualRect = new Rect(0, 240, 100, 20)
            },
            new AlignTestData()
            {
                alignValue = 0,
                sourceRect = new Rect(0, 0, 100, 500),
                actualRect = new Rect(0, 250, 100, 0)
            },
            new AlignTestData()
            {
                alignValue = 50,
                sourceRect = new Rect(15, 100, 100, 500),
                actualRect = new Rect(15, 325, 100, 50)
            },
            new AlignTestData()
            {
                alignValue = 50,
                sourceRect = new Rect(15, 100, 100, 50),
                actualRect = new Rect(15, 100, 100, 50)
            },
            new AlignTestData()
            {
                alignValue = -50,
                sourceRect = new Rect(0, 0, 100, 500),
                actualRect = new Rect(0, 275, 100, -50)
            }
        };

        [Test]
        public void TestAlignRightPass([ValueSource(nameof(alignRightData))] AlignTestData data)
        {
            var alignValue = data.alignValue;
            var sourceRect = data.sourceRect;
            var actualRect = sourceRect.AlignRight(alignValue);
            Assert.AreEqual(data.actualRect, actualRect);
        }

        [Test]
        public void TestAlignLeftPass([ValueSource(nameof(alignLeftData))] AlignTestData data)
        {
            var alignValue = data.alignValue;
            var sourceRect = data.sourceRect;
            var actualRect = sourceRect.AlignLeft(alignValue);
            Assert.AreEqual(data.actualRect, actualRect);
        }

        [Test]
        public void TestAlignBottomPass([ValueSource(nameof(alignBottomData))] AlignTestData data)
        {
            var alignValue = data.alignValue;
            var sourceRect = data.sourceRect;
            var actualRect = sourceRect.AlignBottom(alignValue);
            Assert.AreEqual(data.actualRect, actualRect);
        }

        [Test]
        public void TestAlignTopPass([ValueSource(nameof(alignTopData))] AlignTestData data)
        {
            var alignValue = data.alignValue;
            var sourceRect = data.sourceRect;
            var actualRect = sourceRect.AlignTop(alignValue);
            Assert.AreEqual(data.actualRect, actualRect);
        }

        [Test]
        public void TestAlignCenterXPass([ValueSource(nameof(alignCenterXData))] AlignTestData data)
        {
            var alignValue = data.alignValue;
            var sourceRect = data.sourceRect;
            var actualRect = sourceRect.AlignCenterX(alignValue);
            Assert.AreEqual(data.actualRect, actualRect);
        }

        [Test]
        public void TestAlignCenterYPass([ValueSource(nameof(alignCenterYData))] AlignTestData data)
        {
            var alignValue = data.alignValue;
            var sourceRect = data.sourceRect;
            var actualRect = sourceRect.AlignCenterY(alignValue);
            Assert.AreEqual(data.actualRect, actualRect);
        }
    }
}