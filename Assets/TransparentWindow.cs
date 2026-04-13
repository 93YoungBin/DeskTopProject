using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class TransparentWindow : MonoBehaviour
{
    [DllImport("user32.dll")] static extern IntPtr GetActiveWindow();
    [DllImport("user32.dll")] static extern uint GetWindowLong(IntPtr hWnd, int nIndex);
    [DllImport("user32.dll")] static extern int  SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
    [DllImport("user32.dll")] static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    [DllImport("user32.dll")] static extern int  SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

    [DllImport("Dwmapi.dll")]
    static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    private struct MARGINS
    {
        public int cxLeftWidth, cxRightWidth, cyTopHeight, cyBottomHeight;
    }

    const int  GWL_STYLE      = -16;
    const int  GWL_EXSTYLE    = -20;

    const uint WS_CAPTION     = 0x00C00000;
    const uint WS_THICKFRAME  = 0x00040000;
    const uint WS_MINIMIZE    = 0x00020000;
    const uint WS_MAXIMIZE    = 0x01000000;
    const uint WS_SYSMENU     = 0x00080000;

    const uint WS_EX_LAYERED  = 0x00080000;
    const uint LWA_COLORKEY   = 0x00000001;

    const uint SWP_NOMOVE       = 0x0002;
    const uint SWP_NOSIZE       = 0x0001;
    const uint SWP_FRAMECHANGED = 0x0020;
    const uint SWP_SHOWWINDOW   = 0x0040;

    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

    void Start()
    {
#if !UNITY_EDITOR
        IntPtr hWnd = GetActiveWindow();

        // 1) 타이틀바 / 테두리 완전 제거
        uint style = GetWindowLong(hWnd, GWL_STYLE);
        style &= ~(WS_CAPTION | WS_THICKFRAME | WS_MINIMIZE | WS_MAXIMIZE | WS_SYSMENU);
        SetWindowLong(hWnd, GWL_STYLE, style);

        // 2) Layered 확장 스타일 적용
        uint exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
        exStyle |= WS_EX_LAYERED;
        SetWindowLong(hWnd, GWL_EXSTYLE, exStyle);

        // 3) DWM 투명 합성 확장
        MARGINS margins = new MARGINS { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(hWnd, ref margins);

        // 4) 검정(0x000000) = 투명 색상키
        SetLayeredWindowAttributes(hWnd, 0x000000, 0, LWA_COLORKEY);

        // 5) 스타일 변경 반영 + TopMost (위치/크기 유지)
        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0,
            SWP_NOMOVE | SWP_NOSIZE | SWP_FRAMECHANGED | SWP_SHOWWINDOW);
#endif
    }
}
