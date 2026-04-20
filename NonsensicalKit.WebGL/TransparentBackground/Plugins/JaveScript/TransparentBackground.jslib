var LibraryGLClear = {
    glClear: function(mask)
    {
        if (mask == 0x00004000)
        {
            var v = GLctx.getParameter(GLctx.COLOR_WRITEMASK);
            if (!v[0] && !v[1] && !v[2] && v[3])
                // We are trying to clear alpha only -- skip.
                return;
        }
        GLctx.clear(mask);
    }
};
mergeInto(LibraryManager.library, LibraryGLClear); 

//当需要透明背景时启用这个方法
//打包路径的TemplateData/style.css需要修改
//#unity-canvas { background: transparent !important;}
//色彩空间为liner时无法生效，必须为gamma
//摄像机渲染路径（RenderingPath）不能为延迟渲染（Deferred）,可以为前向渲染（Forward）
//当摄像机中不存在材质为Standard/Opaque物体时会闪烁，可以使用一个跟随相机移动且一直背对相机的面片解决
//unity2021.3.8f1