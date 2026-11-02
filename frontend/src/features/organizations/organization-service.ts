import { apiRequest } from "@/lib/api";

import type {
     CreateOrganizationRequest,
     Organization,
} from "./types";

export function getMyOrganizations(): Promise<Organization[]> {
     return apiRequest<Organization[]>("/api/organizations");
}

export function getOrganization(id: string): Promise<Organization> {
     return apiRequest<Organization>(`/api/organizations/${id}`);
}

export function createOrganization(
     request: CreateOrganizationRequest,
): Promise<Organization> {
     return apiRequest<Organization>("/api/organizations", {
          method: "POST",
          body: request,
     });
}