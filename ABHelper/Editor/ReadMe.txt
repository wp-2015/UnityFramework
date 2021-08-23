TODO:
	1 打包时记录所有打包的资源的记录
	2 ConfigRuntime中很多路径的拼接使用+,但是又不想用Path.Combine，因为使用Path.Combine结合符为\而不是/
	3 UnityWebRequest GET post new 区别
	4 使用md5还是hashcode？
	5 更新时断点续传
		想不到断点续传的使用场景。他的使用场景是启动应用，下载资源，下载了一个资源的一部分时退出，下次再重新下载时继续下载上次下载了一部分的文件。
		但是正常流程不应该是下载文件全部内容然后再写入文件，难道需要监听程序退出的生命周期？或者文件分段下载？
	6 editor与运行时常量重复定义
		public const string VersionFileName         = "Version.txt";                // 记录各个bundle的hash，方便对比差异，确定下载
        public const string AssetFileInfo           = "AssetFileInfo.txt";          // version以及 Asset名字:HashCode
        public const string AssetRelevanceBundle    = "AssetRelevanceBundle.txt";   // 记录asset与bundle的包含关系，哪个bundle里面有哪个bundle
        public const string BuildedFolderFileName   = "BuildedFolder.txt";          // 记录所有的打包的文件夹路径
        public const string BundleNameFileName      = "BundleName.txt";             // 记录所有的打包的bundle的名称
	7 压缩包下载解压
	8 资源依赖分析器需要添加一个
	9 profile分析资源卸载情况

支持更新方式:
	1 更改已经生成的AB中的某个asset
	2 在某个已经生成的AB中添加asset
	3 添加新的AB