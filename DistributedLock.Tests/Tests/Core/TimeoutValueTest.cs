﻿using Medallion.Threading.Internal;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading.Tests.Tests.Core
{
    public class TimeoutValueTest
    {
        [Test]
        public void TestArgumentValidation()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimeoutValue(TimeSpan.FromMilliseconds(-2)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimeoutValue(TimeSpan.FromMilliseconds((long)int.MaxValue + 1)));
        }

        [Test]
        public void TestProperties()
        {
            Assert.IsTrue(default(TimeoutValue).IsZero);
            Assert.IsFalse(default(TimeoutValue).IsInfinite);
            Assert.AreEqual(0, default(TimeoutValue).InMilliseconds);
            Assert.AreEqual(0, default(TimeoutValue).InSeconds);

            TimeoutValue infinite = Timeout.InfiniteTimeSpan;
            Assert.IsFalse(infinite.IsZero);
            Assert.IsTrue(infinite.IsInfinite);
            Assert.AreEqual(-1, infinite.InMilliseconds);
            Assert.Throws<InvalidOperationException>(() => infinite.InSeconds.ToString());

            TimeoutValue normal = TimeSpan.FromSeconds(10.4);
            Assert.IsFalse(normal.IsZero);
            Assert.IsFalse(normal.IsInfinite);
            Assert.AreEqual(10400, normal.InMilliseconds);
            Assert.AreEqual(10, normal.InSeconds);
        }

        [Test]
        public void TestConversion()
        {
            Assert.AreEqual((TimeoutValue)default(TimeSpan?), new TimeoutValue(Timeout.InfiniteTimeSpan));

            CheckEquality(Timeout.InfiniteTimeSpan);
            CheckEquality(TimeSpan.FromSeconds(101.3));
            CheckEquality(TimeSpan.FromTicks(1));
            CheckEquality(TimeSpan.Zero);

            void CheckEquality(TimeSpan value) => Assert.AreEqual((int)value.TotalMilliseconds, ((TimeoutValue)value).InMilliseconds);
        }

        [Test]
        public void TestEquality()
        {
            var timeSpans = new double[] { Timeout.Infinite, 0, 1, 1000, 10101 }.Select(TimeSpan.FromMilliseconds)
                .ToArray();

            foreach (var a in timeSpans)
            {
                foreach (var b in timeSpans)
                {
                    TimeoutValue aValue = a, bValue = b;

                    if (a == b)
                    {
                        Assert.IsTrue(aValue == bValue);
                        Assert.IsFalse(aValue != bValue);
                        Assert.IsTrue(aValue.Equals(bValue));
                        Assert.IsTrue(aValue.Equals((object)bValue));
                        Assert.IsTrue(Equals(aValue, bValue));
                        Assert.AreEqual(aValue.GetHashCode(), bValue.GetHashCode());
                    }
                    else
                    {
                        Assert.IsFalse(aValue == bValue);
                        Assert.IsTrue(aValue != bValue);
                        Assert.IsFalse(aValue.Equals(bValue));
                        Assert.IsFalse(aValue.Equals((object)bValue));
                        Assert.IsFalse(Equals(aValue, bValue));
                        Assert.AreNotEqual(aValue.GetHashCode(), bValue.GetHashCode());
                    }
                }
            }
        }
    }
}