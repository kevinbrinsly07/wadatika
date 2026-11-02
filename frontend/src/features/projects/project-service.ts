import { apiRequest } from "@/lib/api";
import type { CreateProjectRequest, Project } from "./types";

export function getProjects(
     organizationId: string,
): Promise<Project[]> {
     return apiRequest<Project[]>(
          `/api/organizations/${organizationId}/projects`,
     );
}

export function createProject(
     organizationId: string,
     request: CreateProjectRequest,
): Promise<Project> {
     return apiRequest<Project>(
          `/api/organizations/${organizationId}/projects`,
          {
               method: "POST",
               body: request,
          },
     );
}