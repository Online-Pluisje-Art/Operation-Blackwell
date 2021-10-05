using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestFramework {
	// A Test behaves as an ordinary method.
	[Test]
	public void SimpleExampleTest() {
		// Use the Assert class to test conditions.
		Assert.AreEqual(1, 1);
	}
}
