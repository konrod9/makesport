import { cn } from "@/lib/utils"

type Todo = {
  id: number;
  text: string;
  completed: boolean;
};

export default function Todo() {
  const todos: Todo[] = [
    { id: 1, text: "Learn TS", completed: false },
    { id: 2, text: "Build a Next.js app", completed: true },
    { id: 3, text: "Write tests", completed: false },
  ];

  return (
    <div className="flex flex-col gap-4 p-6">
      {todos.map((todo) => (
        <div
          key={todo.id}
          className="flex items-center justify-between rounded-lg border bg-card p-4 text-card-foreground shadow-sm"
        >
          <div className="flex items-center gap-3">
            <div
              className={cn(
                "flex h-6 w-6 items-center justify-center rounded-full border-2",
                todo.completed
                  ? "border-primary bg-primary text-primary-foreground"
                  : "border-border"
              )}
            >
              {todo.completed && (
                <svg className="h-4 w-4" fill="currentColor" viewBox="0 0 20 20">
                  <path
                    fillRule="evenodd"
                    d="M16.707 5.293a1 1 0 010-1.414l-7-7a1 1 0 011.414-1.414L9.5 6.5l4.707-4.707a1 1 0 011.414 0z"
                    clipRule="evenodd"
                  />
                </svg>
              )}
            </div>
            <span className="font-medium">{todo.text}</span>
          </div>
          <span
            className={cn(
              "rounded-full px-3 py-1 text-xs font-medium",
              todo.completed
                ? "bg-secondary text-secondary-foreground"
                : "bg-muted text-muted-foreground"
            )}
          >
            {todo.completed ? "Completed" : "Pending"}
          </span>
        </div>
      ))}
    </div>
  );
}
