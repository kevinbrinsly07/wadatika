"use client";

import { useEffect, useState, type FormEvent } from "react";
import { useOrganization } from "@/features/organizations/organization-context";
import { WorkspaceLayout } from "@/components/layout/workspace-layout";
import { getProjects } from "@/features/projects/project-service";
import type { Project } from "@/features/projects/types";
import {
  createTask,
  getTasks,
  updateTaskPriority,
  updateTaskStatus,
  createTaskComment,
  getTaskComments,
} from "@/features/tasks/task-service";
import type {
  CreateTaskRequest,
  ProjectTask,
  TaskPriority,
  TaskStatus,
} from "@/features/tasks/types";

import type { TaskComment } from "@/features/tasks/comment-types";

export default function TasksPage() {
  const { selectedOrganization } = useOrganization();
  const organizationId = selectedOrganization?.id ?? null;

  const [projects, setProjects] = useState<Project[]>([]);
  const [selectedProjectId, setSelectedProjectId] = useState("");
  const [tasks, setTasks] = useState<ProjectTask[]>([]);

  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [dueDate, setDueDate] = useState("");

  const [isLoading, setIsLoading] = useState(true);
  const [isCreating, setIsCreating] = useState(false);
  const [showForm, setShowForm] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [expandedTaskId, setExpandedTaskId] = useState<string | null>(null);
  const [comments, setComments] = useState<Record<string, TaskComment[]>>({});
  const [commentContent, setCommentContent] = useState("");
  const [isLoadingComments, setIsLoadingComments] = useState(false);
  const [isSubmittingComment, setIsSubmittingComment] = useState(false);

  async function handleStatusChange(task: ProjectTask, status: TaskStatus) {
    if (!organizationId || !selectedProjectId) return;

    try {
      const updatedTask = await updateTaskStatus(
        organizationId,
        selectedProjectId,
        task.id,
        status,
      );

      setTasks((current) =>
        current.map((item) => (item.id === task.id ? updatedTask : item)),
      );
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Unable to update task status.",
      );
    }
  }

  async function handlePriorityChange(
    task: ProjectTask,
    priority: TaskPriority,
  ) {
    if (!organizationId || !selectedProjectId) return;

    try {
      const updatedTask = await updateTaskPriority(
        organizationId,
        selectedProjectId,
        task.id,
        priority,
      );

      setTasks((current) =>
        current.map((item) => (item.id === task.id ? updatedTask : item)),
      );
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Unable to update task priority.",
      );
    }
  }

  useEffect(() => {
    let active = true;

    async function loadProjects() {
      if (!organizationId) {
        setProjects([]);
        setSelectedProjectId("");
        setIsLoading(false);
        return;
      }

      setIsLoading(true);
      setError(null);

      try {
        const result = await getProjects(organizationId);

        if (!active) return;

        setProjects(result);
        setSelectedProjectId((current) =>
          result.some((project) => project.id === current)
            ? current
            : (result[0]?.id ?? ""),
        );
      } catch (err) {
        if (active) {
          setError(
            err instanceof Error ? err.message : "Unable to load projects.",
          );
        }
      } finally {
        if (active) setIsLoading(false);
      }
    }

    void loadProjects();

    return () => {
      active = false;
    };
  }, [organizationId]);

  useEffect(() => {
    let active = true;

    async function loadTasks() {
      if (!organizationId || !selectedProjectId) {
        setTasks([]);
        return;
      }

      setIsLoading(true);
      setError(null);

      try {
        const result = await getTasks(organizationId, selectedProjectId);
        if (active) setTasks(result);
      } catch (err) {
        if (active) {
          setError(
            err instanceof Error ? err.message : "Unable to load tasks.",
          );
        }
      } finally {
        if (active) setIsLoading(false);
      }
    }

    void loadTasks();

    return () => {
      active = false;
    };
  }, [organizationId, selectedProjectId]);

  async function handleCreateTask(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!organizationId || !selectedProjectId) return;

    const request: CreateTaskRequest = {
      title: title.trim(),
      description: description.trim() || null,
      dueDate: dueDate || null,
    };

    setIsCreating(true);
    setError(null);

    try {
      const createdTask = await createTask(
        organizationId,
        selectedProjectId,
        request,
      );

      setTasks((current) => [createdTask, ...current]);
      setTitle("");
      setDescription("");
      setDueDate("");
      setShowForm(false);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unable to create task.");
    } finally {
      setIsCreating(false);
    }
  }

  if (!organizationId) {
    return (
      <WorkspaceLayout>
        <main className="p-8">
          <h1 className="text-2xl font-semibold text-[#628141]">Tasks</h1>
          <p className="mt-2 text-sm text-[#628141]">
            Select an organization to view its tasks.
          </p>
        </main>
      </WorkspaceLayout>
    );
  }

  async function handleToggleComments(taskId: string) {
    if (!organizationId || !selectedProjectId) return;

    if (expandedTaskId === taskId) {
      setExpandedTaskId(null);
      return;
    }

    setExpandedTaskId(taskId);
    setIsLoadingComments(true);
    setError(null);

    try {
      const result = await getTaskComments(
        organizationId,
        selectedProjectId,
        taskId,
      );

      setComments((current) => ({
        ...current,
        [taskId]: result,
      }));
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unable to load comments.");
    } finally {
      setIsLoadingComments(false);
    }
  }

  async function handleCreateComment(
    event: FormEvent<HTMLFormElement>,
    taskId: string,
  ) {
    event.preventDefault();

    if (!organizationId || !selectedProjectId || !commentContent.trim()) {
      return;
    }

    setIsSubmittingComment(true);
    setError(null);

    try {
      const comment = await createTaskComment(
        organizationId,
        selectedProjectId,
        taskId,
        { content: commentContent.trim() },
      );

      setComments((current) => ({
        ...current,
        [taskId]: [...(current[taskId] ?? []), comment],
      }));

      setCommentContent("");
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Unable to create comment.",
      );
    } finally {
      setIsSubmittingComment(false);
    }
  }

  return (
    <WorkspaceLayout>
      <main className="mx-auto max-w-6xl p-8">
        <div className="flex flex-wrap items-center justify-between gap-4">
          <div>
            <h1 className="text-2xl font-semibold text-[#628141]">Tasks</h1>
            <p className="mt-1 text-xs text-[#628141]">
              Manage and track your project work.
            </p>
          </div>

          <button
            type="button"
            onClick={() => setShowForm((current) => !current)}
            disabled={!selectedProjectId}
            className="rounded-lg bg-[#628141] px-4 py-2.5 text-sm font-semibold text-black/80 hover:bg-[#506d35] cursor-pointer disabled:opacity-50"
          >
            {showForm ? "Cancel" : "New task"}
          </button>
        </div>

        <div className="mt-6 max-w-sm">
          <label
            htmlFor="project-select"
            className="mb-2 block text-sm font-medium text-[#628141]"
          >
            Project
          </label>

          <select
            id="project-select"
            value={selectedProjectId}
            onChange={(event) => setSelectedProjectId(event.target.value)}
            className="w-full rounded-lg bg-[#628141] px-3 py-2.5 text-sm text-black outline-none focus:ring-2 focus:ring-[#628141]"
          >
            {projects.length === 0 && (
              <option value="">No projects available</option>
            )}

            {projects.map((project) => (
              <option key={project.id} value={project.id}>
                {project.name} ({project.key})
              </option>
            ))}
          </select>
        </div>

        {error && (
          <div
            role="alert"
            className="mt-6 rounded-lg bg-red-50 p-4 text-sm text-red-700"
          >
            {error}
          </div>
        )}

        {showForm && (
          <form
            onSubmit={handleCreateTask}
            className="mt-6 max-w-xl space-y-4 rounded-xl bg-[#628141] p-6 shadow-sm"
          >
            <h2 className="text-lg font-semibold text-[#1B211A]">
              Create a task
            </h2>

            <div>
              <label
                htmlFor="task-title"
                className="mb-1 block text-sm font-medium text-[#1B211A]"
              >
                Title
              </label>
              <input
                id="task-title"
                value={title}
                onChange={(event) => setTitle(event.target.value)}
                required
                maxLength={200}
                placeholder="Implement user authentication"
                className="w-full rounded-lg bg-[#1B211A] px-3 py-3 text-sm text-[#628141] outline-none focus:ring-2 focus:ring-[#628141]"
              />
            </div>

            <div>
              <label
                htmlFor="task-description"
                className="mb-1 block text-sm font-medium text-black"
              >
                Description
              </label>
              <textarea
                id="task-description"
                value={description}
                onChange={(event) => setDescription(event.target.value)}
                maxLength={2000}
                rows={3}
                placeholder="Describe the work..."
                className="w-full rounded-lg px-3 py-2 text-sm bg-[#1B211A] text-[#628141] outline-none focus:ring-2 focus:ring-[#628141]"
              />
            </div>

            <div>
              <label
                htmlFor="task-due-date"
                className="mb-1 block text-sm font-medium text-black"
              >
                Due date
              </label>
              <input
                id="task-due-date"
                type="date"
                value={dueDate}
                onChange={(event) => setDueDate(event.target.value)}
                className="rounded-lg bg-[#1B211A] px-3 py-2 text-sm text-[#628141] outline-none focus:ring-2 focus:ring-[#628141]"
              />
            </div>

            <button
              type="submit"
              disabled={isCreating}
              className="rounded-lg cursor-pointer bg-[#1B211A] px-4 py-2.5 text-sm font-semibold  hover:bg-[#EBD5AB] hover:text-black disabled:opacity-50"
            >
              {isCreating ? "Creating..." : "Create task"}
            </button>
          </form>
        )}

        <section className="mt-8">
          {isLoading ? (
            <p className="py-12 text-center text-sm">Loading tasks...</p>
          ) : tasks.length === 0 ? (
            <div className="rounded-xl border border-dashed border-[#628141] px-6 py-16 text-center">
              <h2 className="text-lg font-medium">No tasks yet</h2>
              <p className="mt-2 text-sm">
                Create your first task for this project.
              </p>
            </div>
          ) : (
            <div className="overflow-hidden rounded-xl bg-[#628141]">
              {tasks.map((task) => (
                <article
                  key={task.id}
                  className="flex flex-wrap items-center justify-between gap-4 border-b border-slate-100 p-5 last:border-b-0"
                >
                  <div className="min-w-0 flex-1">
                    <h2 className="font-medium text-[#1B211A]">{task.title}</h2>
                    {task.description && (
                      <p className="mt-1 text-sm text-[#1B211A]">
                        {task.description}
                      </p>
                    )}
                    {task.dueDate && (
                      <p className="mt-2 text-xs text-[#1B211A]">
                        Due: {task.dueDate}
                      </p>
                    )}
                  </div>

                  <div className="flex items-center gap-2">
                    <select
                      aria-label={`Status for ${task.title}`}
                      value={task.status}
                      onChange={(event) =>
                        void handleStatusChange(
                          task,
                          Number(event.target.value) as TaskStatus,
                        )
                      }
                      className="rounded-full bg-[#1B211A] px-3 py-1 text-xs"
                    >
                      <option value={1}>Todo</option>
                      <option value={2}>In Progress</option>
                      <option value={3}>In Review</option>
                      <option value={4}>Done</option>
                    </select>

                    <select
                      aria-label={`Priority for ${task.title}`}
                      value={task.priority}
                      onChange={(event) =>
                        void handlePriorityChange(
                          task,
                          Number(event.target.value) as TaskPriority,
                        )
                      }
                      className="rounded-full bg-[#1B211A] px-5 py-1 text-xs"
                    >
                      <option value={1}>Low</option>
                      <option value={2}>Medium</option>
                      <option value={3}>High</option>
                      <option value={4}>Urgent</option>
                    </select>
                  </div>

                  <div className="w-full">
                    <button
                      type="button"
                      onClick={() => void handleToggleComments(task.id)}
                      className="mt-2 text-sm font-medium text-black cursor-pointer hover:underline"
                    >
                      {expandedTaskId === task.id
                        ? "Hide comments"
                        : "View comments"}
                    </button>

                    {expandedTaskId === task.id && (
                      <div className="mt-4 space-y-4 rounded-lg bg-[#1B211A] p-4">
                        {isLoadingComments ? (
                          <p className="text-sm">Loading comments...</p>
                        ) : (
                          <div className="space-y-3">
                            {(comments[task.id] ?? []).length === 0 ? (
                              <p className="text-sm">No comments yet.</p>
                            ) : (
                              comments[task.id].map((comment) => (
                                <div
                                  key={comment.id}
                                  className="rounded-lg bg-[#1B211A] p-3"
                                >
                                  <p className="text-sm">{comment.content}</p>
                                  <p className="mt-2 text-xs">
                                    {new Date(
                                      comment.createdAt,
                                    ).toLocaleString()}
                                  </p>
                                </div>
                              ))
                            )}
                          </div>
                        )}

                        <form
                          onSubmit={(event) =>
                            void handleCreateComment(event, task.id)
                          }
                          className="flex flex-col gap-2 sm:flex-row"
                        >
                          <input
                            value={commentContent}
                            onChange={(event) =>
                              setCommentContent(event.target.value)
                            }
                            maxLength={5000}
                            required
                            placeholder="Write a comment..."
                            className="min-w-0 flex-1 rounded-lg bg-[#628141] px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-[#628141]"
                          />

                          <button
                            type="submit"
                            disabled={
                              isSubmittingComment || !commentContent.trim()
                            }
                            className="rounded-lg bg-[#628141] px-4 py-2 text-sm font-medium text-white disabled:opacity-50"
                          >
                            {isSubmittingComment ? "Sending..." : "Comment"}
                          </button>
                        </form>
                      </div>
                    )}
                  </div>
                </article>
              ))}
            </div>
          )}
        </section>
      </main>
    </WorkspaceLayout>
  );
}
