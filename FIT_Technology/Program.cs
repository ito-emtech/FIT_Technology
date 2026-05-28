using FIT_Technology.Models.Services;

/**
 * <summary>
 * アプリケーションの起動構成クラス。
 * サービスの登録、ミドルウェアパイプラインの設定、ルート定義を行います。
 * </summary>
 */
var builder = WebApplication.CreateBuilder(args);

// --- サービスの登録 (Dependency Injection) ---

// MVC コントローラーとビューのサポートを追加
builder.Services.AddControllersWithViews();

// セッション状態を管理するためのメモリキャッシュ（分散キャッシュ）を登録
builder.Services.AddDistributedMemoryCache();

// セッションオプションの設定
builder.Services.AddSession(options => {
    // セッションの有効期限（60分）
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    // クライアントサイドのスクリプトからクッキーへのアクセスを禁止（セキュリティ対策）
    options.Cookie.HttpOnly = true;
    // ユーザーの同意に関わらず、アプリの動作に必須のクッキーとして扱う
    options.Cookie.IsEssential = true;
});

// アプリケーション固有のサービス（AccountService）をスコープ単位で登録
// リクエストごとに新しいインスタンスが作成されます
builder.Services.AddScoped<AccountService>();

var app = builder.Build();

// --- HTTP リクエストパイプライン（ミドルウェア）の設定 ---

// 開発環境以外（本番環境など）のエラーハンドリング
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// wwwroot フォルダ内の静的ファイル（CSS, JS, 画像など）を配信可能にする
app.UseStaticFiles();

// ルーティングを有効化
app.UseRouting();

// 認可（Authorization）を有効化
app.UseAuthorization();

// セッションミドルウェアを有効化
// ※重要: app.UseRouting() と app.MapControllerRoute() の間に配置する必要があります
app.UseSession();

// デフォルトのルート定義
// 起動時に Account コントローラーの Login アクションを呼び出す設定
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// アプリケーションの実行
app.Run();
