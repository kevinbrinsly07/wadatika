export type TaskStatus = 1 | 2 | 3 | 4;

export type TaskPriority = 1 | 2 | 3 | 4;

export type CreateTaskRequest = {
  title: string;
  description?: string | null;
  dueDate?: string | null;
};

export type ProjectTask = {
  id: string;
  projectId: string;
  createdByUserId: string;
  assigneeUserId: string | null;
  title: string;
  description: string | null;
  status: TaskStatus;
  priority: TaskPriority;
  dueDate: string | null;
  createdAt: string;
  updatedAt: string | null;
};