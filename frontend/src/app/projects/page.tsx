"use client";

import { useCallback, useEffect, useState, type FormEvent } from "react";
import {
  createProject,
  getProjects,
} from "@/features/projects/project-service";
import type { CreateProjectRequest, Project } from "@/features/projects/types";
import { useOrganization } from "@/features/organizations/organization-context";

export default function ProjectsPage() {
  const { selectedOrganization } = useOrganization();
  const selectedOrganizationId = selectedOrganization?.id ?? null;

  const [projects, setProjects] = useState<Project[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isCreating, setIsCreating] = useState(false);
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [name, setName] = useState("");
  const [key, setKey] = useState("");
  const [description, setDescription] = useState("");

  const loadProjects = useCallback(async () => {
    if (!selectedOrganizationId) {
      setProjects([]);
      setIsLoading(false);
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const result = await getProjects(selectedOrganizationId);
      setProjects(result);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unable to load projects.");
    } finally {
      setIsLoading(false);
    }
  }, [selectedOrganizationId]);

  useEffect(() => {
    let active = true;

    async function fetchProjects() {
      if (!selectedOrganizationId) {
        setProjects([]);
        setError(null);
        setIsLoading(false);
        return;
      }

      setIsLoading(true);
      setError(null);

      try {
        const result = await getProjects(selectedOrganizationId);

        if (active) {
          setProjects(result);
        }
      } catch (err) {
        if (active) {
          setError(
            err instanceof Error ? err.message : "Unable to load projects.",
          );
        }
      } finally {
        if (active) {
          setIsLoading(false);
        }
      }
    }

    void fetchProjects();

    return () => {
      active = false;
    };
  }, [selectedOrganizationId]);

  async function handleCreateProject(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!selectedOrganizationId) return;

    setIsCreating(true);
    setError(null);

    const request: CreateProjectRequest = {
      name: name.trim(),
      key: key.trim().toUpperCase(),
      description: description.trim() || null,
    };

    try {
      const createdProject = await createProject(
        selectedOrganizationId,
        request,
      );

      setProjects((current) =>
        [...current, createdProject].sort((a, b) =>
          a.name.localeCompare(b.name),
        ),
      );

      setName("");
      setKey("");
      setDescription("");
      setShowCreateForm(false);
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Unable to create project.",
      );
    } finally {
      setIsCreating(false);
    }
  }

  if (!selectedOrganizationId) {
    return (
      <main className="p-8">
        <h1 className="text-2xl font-semibold text-white">Projects</h1>
        <p className="mt-2 text-sm text-gray-300">
          Select an organization to view its projects.
        </p>
      </main>
    );
  }

  return (
    <main className="mx-auto max-w-6xl p-8">
      <div className="flex flex-wrap items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-semibold text-white">Projects</h1>
          <p className="mt-1 text-sm text-gray-300">
            Organize and track work across your organization.
          </p>
        </div>

        <button
          type="button"
          onClick={() => setShowCreateForm((current) => !current)}
          className="rounded-lg bg-[#628141] px-4 py-2.5 text-sm font-bold text-[#1B211A] hover:bg-[#8BAE66] cursor-pointer"
        >
          {showCreateForm ? "Cancel" : "New project"}
        </button>
      </div>

      {error && (
        <div
          role="alert"
          className="mt-6 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700"
        >
          {error}
        </div>
      )}

      {showCreateForm && (
        <form
          onSubmit={handleCreateProject}
          className="mt-6 max-w-xl space-y-4 rounded-xl bg-[#628141] p-6 shadow-sm"
        >
          <h2 className="text-lg font-semibold text-[#1B211A]">
            Create a project
          </h2>

          <div>
            <label
              htmlFor="project-name"
              className="mb-1.5 block text-sm font-medium text-[#1B211A]"
            >
              Project name
            </label>
            <input
              id="project-name"
              value={name}
              onChange={(event) => setName(event.target.value)}
              required
              maxLength={100}
              placeholder="Website Redesign"
              className="w-full rounded-lg text-[#628141] bg-[#1B211A] px-3 py-2 text-sm outline-none focus:border-[#628141] focus:ring-2 focus:ring-[#EBD5AB]"
            />
          </div>

          <div>
            <label
              htmlFor="project-key"
              className="mb-1.5 block text-sm font-medium text-[#1B211A]"
            >
              Project key
            </label>
            <input
              id="project-key"
              value={key}
              onChange={(event) => setKey(event.target.value.toUpperCase())}
              required
              minLength={2}
              maxLength={10}
              pattern="[A-Za-z][A-Za-z0-9]{1,9}"
              placeholder="WEB"
              className="w-full rounded-lg text-[#628141] bg-[#1B211A] px-3 py-2 text-sm outline-none focus:border-[#628141] focus:ring-2 focus:ring-[#EBD5AB]"
            />
            <p className="mt-1 text-xs text-[#1B211A]">
              2–10 letters or numbers, starting with a letter.
            </p>
          </div>

          <div>
            <label
              htmlFor="project-description"
              className="mb-1.5 block text-sm font-medium text-[#1B211A]"
            >
              Description{" "}
              <span className="font-normal text-[#1B211A]">(optional)</span>
            </label>
            <textarea
              id="project-description"
              value={description}
              onChange={(event) => setDescription(event.target.value)}
              maxLength={500}
              rows={3}
              placeholder="What is this project about?"
              className="w-full resize-y rounded-lg px-3 py-2 text-[#628141] bg-[#1B211A]  text-sm outline-none focus:border-[#628141] focus:ring-2 focus:ring-[#EBD5AB]"
            />
          </div>

          <button
            type="submit"
            disabled={isCreating}
            className="rounded-lg bg-[#1B211A] px-4 py-2.5 text-sm font-medium text-[#628141] hover:bg-[#8BAE66] hover:text-[#1B211A] disabled:cursor-not-allowed disabled:opacity-60 cursor-pointer"
          >
            {isCreating ? "Creating..." : "Create project"}
          </button>
        </form>
      )}

      <section className="mt-8">
        {isLoading ? (
          <p className="py-12 text-center text-sm text-slate-500">
            Loading projects...
          </p>
        ) : projects.length === 0 ? (
          <div className="rounded-xl border border-dashed border-slate-300 bg-white px-6 py-16 text-center">
            <h2 className="text-lg font-medium text-slate-900">
              No projects yet
            </h2>
            <p className="mt-2 text-sm text-slate-500">
              Create your first project to start organizing work.
            </p>
          </div>
        ) : (
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {projects.map((project) => (
              <article
                key={project.id}
                className="rounded-xl border border-[#1B211A] bg-[#628141] p-5 shadow-sm"
              >
                <div className="flex items-start justify-between gap-3">
                  <h2 className="font-semibold text-[#1B211A]">
                    {project.name}
                  </h2>
                  <span className="rounded-md bg-[#EBD5AB] px-2 py-1 text-xs font-semibold text-[#1B211A]">
                    {project.key}
                  </span>
                </div>

                <p className="mt-3 min-h-10 text-sm text-[#1B211A]">
                  {project.description || "No description provided."}
                </p>

                <p className="mt-5 text-xs text-[#1B211A]">
                  Created {new Date(project.createdAt).toLocaleDateString()}
                </p>
              </article>
            ))}
          </div>
        )}
      </section>
    </main>
  );
}
