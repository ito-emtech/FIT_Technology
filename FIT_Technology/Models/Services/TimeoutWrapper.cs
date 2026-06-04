namespace FIT_Technology.Models.Services
{
    /// <summary>
    /// 指定された処理を実行し、制限時間を超えた場合に強制終了させるための共通クラスです。
    /// </summary>
    public static class TimeoutWrapper
    {
        // デフォルトのタイムアウト時間を3秒（3000ミリ秒）に設定
        private const int DefaultTimeoutMs = 3000;

        /// <summary>
        /// 戻り値がある処理（boolなど）をタイムアウト付きで実行します。
        /// </summary>
        /// <typeparam name="T">戻り値の型</typeparam>
        /// <param name="action">実行したいDB処理などのデリゲート</param>
        /// <param name="timeoutMs">タイムアウト時間（ミリ秒）※省略時は3000ms</param>
        public static T Execute<T>(Func<T> action, int timeoutMs = DefaultTimeoutMs)
        {
            try
            {
                // 処理を別タスクとして実行
                var dbTask = Task.Run(action);

                // 実際の処理と、タイムアウトタイマーの早い者勝ち競争
                if (Task.WhenAny(dbTask, Task.Delay(timeoutMs)).Result == dbTask)
                {
                    // 3秒以内に終われば、その結果（true/falseなど）を返す
                    return dbTask.Result;
                }
                else
                {
                    // 3秒経っても終わらなければ例外をスロー
                    throw new TimeoutException($"C#側で制限時間（{timeoutMs}ms）超過を検知しました。");
                }
            }
            catch (Exception)
            {
                // タイムアウト例外、または内部で起きたDBエラーはすべてここに集約される
                throw;
            }
        }
    }
}