namespace NHibernate.DependencyInjection
{
    /// <summary>
    /// Helper class for initializing the custom bytecode provider
    /// </summary>
    public static class Initializer
    {
        /// <summary>
        /// Plugs the bytecode provider into NHibernate
        /// </summary>
        /// <param name="injector">IEntityInjector implementation for dependency injection with NHibernate</param>
        public static void RegisterBytecodeProvider(IEntityInjector injector)
        {
            Cfg.Environment.BytecodeProvider = new BytecodeProvider(injector);
        }

        /// <summary>
        /// Plugs the bytecode provider into NHibernate
        /// </summary>
        public static void RegisterBytecodeProvider()
        {
            Cfg.Environment.BytecodeProvider = new BytecodeProvider();
        }
    }
}