def clear():
    automation_context.ScriptingContext.Clear()

def exit():
    automation_context.Exit()

def version():
	return automation_context.Version()