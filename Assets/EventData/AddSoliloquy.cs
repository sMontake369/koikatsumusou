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
    public override BaseEventData Copy()
    {
        AddSoliloquy copy = CreateInstance<AddSoliloquy>();
        copy.messageList = new List<string>();
        if (messageList != null) copy.messageList = messageList;
        return copy;
    }

    public override void Init()
    {
        solM = GameManager.solM;
    }

    public override void DoNext()
    {
        cts2.Cancel();
    }

    protected override async UniTask DoEvent(CancellationToken token)
    {
        try 
        {
            foreach (string message in messageList) 
            {
                cts2 = new CancellationTokenSource();
                await solM.SetSoliloquy(message, cts2);
                token.ThrowIfCancellationRequested();
            }
        }
        catch (Exception)
        {
            
        }
    }
}
