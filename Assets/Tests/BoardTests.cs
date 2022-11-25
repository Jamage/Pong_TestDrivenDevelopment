using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

public class BoardTests
{
    TestSetup _setup = new TestSetup();

    [Test]
    public void AtLeastOnePaddleIsSuccesfullyCreated()
    {
        GameObject[] paddles = _setup.CreatePaddlesForTest();

        Assert.IsNotNull(paddles);
    }
    
    [Test]
    public void TwoPaddlesAreSuccesfullyCreated()
    {
        GameObject[] paddles = _setup.CreatePaddlesForTest();

        //assert if number of paddles does not equal 2
        Assert.AreEqual(2, paddles.Length);
    }

    [Test]
    public void CreatedPaddlesHaveCorrectNames()
    {
        GameObject[] paddles = _setup.CreatePaddlesForTest();

        for (int i = 0; i < paddles.Length; i++)
        {
            Assert.AreEqual("Paddle" + (i + 1).ToString(), paddles[i].name);
        }
    }

    [Test]
    public void Paddle1HasCorrectName()
    {
        GameObject[] paddles = _setup.CreatePaddlesForTest();

        Assert.AreEqual("Paddle1", paddles[0].name);
    }

    [Test]
    public void Paddle2HasCorrectName()
    {
        GameObject[] paddles = _setup.CreatePaddlesForTest();

        Assert.AreEqual("Paddle2", paddles[1].name);
    }

    [Test]
    public void Paddle1HasCorrectPosition()
    {
        GameObject[] paddles = _setup.CreatePaddlesForTest();

        Assert.AreEqual(new Vector3(-6, 0, 0), paddles[0].transform.position);
    }

    [Test]
    public void Paddle2HasCorrectPosition()
    {
        GameObject[] paddles = _setup.CreatePaddlesForTest();

        Assert.AreEqual(new Vector3(6, 0, 0), paddles[1].transform.position);
    }

    [Test]
    public void BallIsSuccesfullyCreated()
    {
        BoardManager board = new BoardManager();
        GameObject ball = board.CreateBall();

        Assert.IsNotNull(ball);
    }

    [UnitySetUp]
    public IEnumerator TestSetup()
    {
        _setup.DestroyAll();
        Camera cam = _setup.CreateCameraForTest();
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TestTeardown()
    {
        _setup.DestroyAll();
        yield return null;
    }

    [UnityTest]
    public IEnumerator Paddle1StaysInUpperCameraBounds()
    {
        GameObject[] paddles = _setup.CreatePaddlesForTest();
        Paddle firstPaddle = paddles[0].GetComponent<Paddle>();

        float time = 0;
        while (time < 5)
        {
            firstPaddle.MoveUpY("Paddle1");
            time += Time.deltaTime;
        }
        yield return new WaitForFixedUpdate();

        Assert.LessOrEqual(paddles[0].transform.position.y, 4.15);

        //edge of paddle should not leave edge of screen
    }

    [UnityTest]
    public IEnumerator Paddle1StaysInLowerCameraBounds()
    {
        GameObject[] paddles = _setup.CreatePaddlesForTest();
        Paddle firstPaddle = paddles[0].GetComponent<Paddle>();
        float time = 0;
        while (time < 5)
        {
            firstPaddle.MoveDownY("Paddle1");
            time += Time.deltaTime;
        }
        yield return new WaitForFixedUpdate();

        // 5 is Camera.main.othographicsize
        Assert.GreaterOrEqual(paddles[0].transform.position.y, -4.15);
    }

    [UnityTest]
    public IEnumerator Paddle2StaysInUpperCameraBounds()
    {
        GameObject[] paddles = _setup.CreatePaddlesForTest();
        Paddle secondPaddle = paddles[1].GetComponent<Paddle>();

        float time = 0;
        while (time < 5)
        {
            secondPaddle.MoveUpY("Paddle2");
            time += Time.deltaTime;
        }
        yield return new WaitForFixedUpdate();

        // 4 is (Camera.main.orthographicSize - transform.localScale.y /2)
        Assert.LessOrEqual(paddles[1].transform.position.y, 4.15);

        //edge of paddle should not leave edge of screen
    }

    [UnityTest]
    public IEnumerator Paddle2StaysInLowerCameraBounds()
    {
        GameObject[] paddles = _setup.CreatePaddlesForTest();
        Paddle secondPaddle = paddles[1].GetComponent<Paddle>();

        float time = 0;
        while (time < 5)
        {
            secondPaddle.MoveDownY("Paddle2");
            time += Time.deltaTime;
        }
        yield return new WaitForFixedUpdate();

        // 4 is (Camera.main.orthographicSize - transform.localScale.y /2)
        Assert.GreaterOrEqual(paddles[1].transform.position.y, -4.15);
    }

    [UnityTest]
    public IEnumerator BallBouncesFromBoardBounds()
    {
        GameObject ball = _setup.CreateBallForTest();
        ball.GetComponent<Ball>().SetSpeed(0, 1);
        float time = 0;
        while (time < 5)
        {
            ball.GetComponent<Ball>().Move();
            time += Time.deltaTime;
        }
        yield return new WaitForFixedUpdate();
        Assert.Less(ball.transform.position.y, 5);
    }

    [UnityTest]
    public IEnumerator BallCollidesWithPaddle1()
    {
        GameObject[] paddles = _setup.CreatePaddlesForTest();
        GameObject ball = _setup.CreateBallForTest();
        Ball ballComp = ball.GetComponent<Ball>();
        ballComp.SetSpeed(-1, 0);
        ballComp.transform.position = paddles[0].transform.position;
        yield return new WaitForFixedUpdate();
        Assert.IsTrue(ballComp.GetSpeed().x == 1, $"Speed was {ballComp.GetSpeed()}");
    }

    [UnityTest]
    public IEnumerator BallCollidesWithPaddle2()
    {
        GameObject[] paddles = _setup.CreatePaddlesForTest();
        GameObject ball = _setup.CreateBallForTest();
        Ball ballComp = ball.GetComponent<Ball>();
        ballComp.SetSpeed(1, 0);
        ballComp.transform.position = paddles[1].transform.position;
        yield return new WaitForFixedUpdate();
        Assert.IsTrue(ballComp.GetSpeed().x == -1, $"Speed was {ballComp.GetSpeed()}");
    }

    [Test]
    public void ScoreIsInitiallyZeroed()
    {
        BoardManager board = new BoardManager();
        Assert.AreEqual(0, board.playerOneScore);
        Assert.AreEqual(0, board.playerTwoScore);
    }

    [Test]
    public void PlayerOneCanScore()
    {
        BoardManager board = new BoardManager();
        GameObject ball = board.CreateBall();
        ball.GetComponent<Ball>().SetSpeed(0, 0);
        ball.transform.position = new Vector2(0, 7);
        board.CheckForScore();
        Assert.AreEqual(1, board.playerOneScore);
    }
    
    [Test]
    public void PlayerTwoCanScore()
    {
        BoardManager board = new BoardManager();
        GameObject ball = board.CreateBall();
        ball.GetComponent<Ball>().SetSpeed(0, 0);
        ball.transform.position = new Vector2(0, -7);
        board.CheckForScore();
        Assert.AreEqual(1, board.playerTwoScore);
    }

    [Test]
    public void BallIsResetAfterScoring()
    {
        BoardManager board = new BoardManager();
        GameObject ball = board.CreateBall();
        ball.GetComponent<Ball>().SetSpeed(0, 0);
        ball.transform.position = new Vector2(0, -7);
        board.CheckForScore();
        Assert.AreEqual(new Vector3(0, 0, 0), ball.transform.position);
    }

}
