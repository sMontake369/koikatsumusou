using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine.UIElements;

public class SoliloquyManager : BaseWindowManager
{
    Label textLabel; // 独り言のテキスト
    AudioManager audM;
    public bool doSoliloquy { get; private set; } = false; // 独り言を表示中かどうか
    CancellationTokenSource cts;

    public override void init()
    {
        rootElement = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("SoliloquyElement");
        textLabel = rootElement.Q<VisualElement>("Soliloquy").Q<Label>("Text");
        textLabel.text = "";
        doSoliloquy = false;

        rootElement.RegisterCallback<ClickEvent>((e) => { onClick(); });

        hideWindow();
        audM = GameManager.audM;

        if (cts != null) cts.Cancel();
    }

    public override void startGame()
    {
        showWindow();
    }

    void onClick()
    {
        if (doSoliloquy) cts.Cancel();
    }

    public async UniTask setSoliloquy(string text, CancellationTokenSource cts = null)
    {
        textLabel.text = text;
        doSoliloquy = true;
        audM.PlayNormalSound(NormalSound.soliloquy);
        if (cts == null) this.cts = new CancellationTokenSource();
        else this.cts = cts;

        try { await UniTask.Delay((int)math.lerp(0, 3000, math.clamp(text.Length, 10, 20)) / 20, cancellationToken: this.cts.Token); }
        catch (Exception) { }
        
        textLabel.text = "";
        doSoliloquy = false;
        this.cts = null;
    }
}
