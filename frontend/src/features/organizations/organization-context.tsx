"use client";

import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useState,
  type ReactNode,
} from "react";

import { useAuth } from "@/features/auth/auth-context";

import { getMyOrganizations } from "./organization-service";
import type { Organization } from "./types";


type OrganizationContextValue = {
  organizations: Organization[];
  selectedOrganization: Organization | null;
  selectOrganization: (id: string) => void;
  isLoading: boolean;
  error: string | null;
  addOrganization: (organization: Organization) => void;
};

const OrganizationContext = createContext<OrganizationContextValue | undefined>(
  undefined,
);

const STORAGE_KEY = "wadatika-selected-organization";

export function OrganizationProvider({ children }: { children: ReactNode }) {
  const { status } = useAuth();
  const [organizations, setOrganizations] = useState<Organization[]>([]);
  const [selectedOrganizationId, setSelectedOrganizationId] = useState<
    string | null
  >(() => {
    if (typeof window === "undefined") return null;

    return window.localStorage.getItem(STORAGE_KEY);
  });
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (status !== "authenticated") {
      return;
    }

    let active = true;

    async function loadOrganizations() {
      try {
        const result = await getMyOrganizations();

        if (!active) return;

        setOrganizations(result);
        setError(null);

        setSelectedOrganizationId((currentId) => {
          const stillExists = result.some(
            (organization) => organization.id === currentId,
          );

          if (stillExists) return currentId;

          return result[0]?.id ?? null;
        });
      } catch (err) {
        if (!active) return;

        setError(
          err instanceof Error ? err.message : "Unable to load organizations.",
        );
      } finally {
        if (active) setIsLoading(false);
      }
    }

    void loadOrganizations();

    return () => {
      active = false;
    };
  }, [status]);

  const selectOrganization = useCallback((id: string) => {
    setSelectedOrganizationId(id);
    window.localStorage.setItem(STORAGE_KEY, id);
  }, []);

  useEffect(() => {
    if (selectedOrganizationId) {
      window.localStorage.setItem(STORAGE_KEY, selectedOrganizationId);
    } else if (!isLoading) {
      window.localStorage.removeItem(STORAGE_KEY);
    }
  }, [selectedOrganizationId, isLoading]);

  const selectedOrganization =
    organizations.find(
      (organization) => organization.id === selectedOrganizationId,
    ) ?? null;

  const addOrganization = useCallback((organization: Organization) => {
    setOrganizations((current) =>
      [...current, organization].sort((a, b) => a.name.localeCompare(b.name)),
    );

    setSelectedOrganizationId(organization.id);
  }, []);

  return (
    <OrganizationContext.Provider
      value={{
        organizations,
        selectedOrganization,
        selectOrganization,
        addOrganization,
        isLoading,
        error,
      }}
    >
      {children}
    </OrganizationContext.Provider>
  );
}

export function useOrganization() {
  const context = useContext(OrganizationContext);

  if (!context) {
    throw new Error(
      "useOrganization must be used inside an OrganizationProvider.",
    );
  }

  return context;
}
