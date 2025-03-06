using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "AddSoliloquy ", menuName = "EventData/AddSoliloquy")]
public class AddSoliloquy : BaseEventData
{
    [Label("セリフ")]
    public List<string> messageList;
    SoliloquyManager solM;
    CancellationTokenSource cts2;
    public override BaseEventData deepCopy()
    {
        AddSoliloquy copy = CreateInstance<AddSoliloquy>();
        copy.messageList = new List<string>();
        if (messageList != null) copy.messageList = messageList;
        return copy;
    }

    public override void init()
    {
        solM = GameManager.solM;
    }

    public override void next()
    {
        cts2.Cancel();
    }

    protected override async UniTask doEvent(CancellationToken token)
    {
        try 
        {
            foreach (string message in messageList) 
            {
                cts2 = new CancellationTokenSource();
                await solM.setSoliloquy(message, cts2);
                token.ThrowIfCancellationRequested();
            }
        }
        catch (Exception)
        {
            
        }
    }
}
