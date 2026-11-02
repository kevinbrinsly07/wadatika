import { apiRequest } from "@/lib/api";
import type {
     CreateTaskRequest,
     ProjectTask,
     TaskPriority,
     TaskStatus,
} from "./types";

import type {
     CreateTaskCommentRequest,
     TaskComment,
} from "./comment-types";


export function getTasks(
     organizationId: string,
     projectId: string,
): Promise<ProjectTask[]> {
     return apiRequest<ProjectTask[]>(
          `/api/organizations/${organizationId}/projects/${projectId}/tasks`,
     );
}

export function createTask(
     organizationId: string,
     projectId: string,
     request: CreateTaskRequest,
): Promise<ProjectTask> {
     return apiRequest<ProjectTask>(
          `/api/organizations/${organizationId}/projects/${projectId}/tasks`,
          {
               method: "POST",
               body: request,
          },
     );
}

export function updateTaskStatus(
     organizationId: string,
     projectId: string,
     taskId: string,
     status: TaskStatus,
): Promise<ProjectTask> {
     return apiRequest<ProjectTask>(
          `/api/organizations/${organizationId}/projects/${projectId}/tasks/${taskId}/status`,
          {
               method: "PATCH",
               body: { status },
          },
     );
}

export function updateTaskPriority(
     organizationId: string,
     projectId: string,
     taskId: string,
     priority: TaskPriority,
): Promise<ProjectTask> {
     return apiRequest<ProjectTask>(
          `/api/organizations/${organizationId}/projects/${projectId}/tasks/${taskId}/priority`,
          {
               method: "PATCH",
               body: { priority },
          },
     );
}

export function getTaskComments(
     organizationId: string,
     projectId: string,
     taskId: string,
): Promise<TaskComment[]> {
     return apiRequest<TaskComment[]>(
          `/api/organizations/${organizationId}/projects/${projectId}/tasks/${taskId}/comments`,
     );
}

export function createTaskComment(
     organizationId: string,
     projectId: string,
     taskId: string,
     request: CreateTaskCommentRequest,
): Promise<TaskComment> {
     return apiRequest<TaskComment>(
          `/api/organizations/${organizationId}/projects/${projectId}/tasks/${taskId}/comments`,
          {
               method: "POST",
               body: request,
          },
     );
}