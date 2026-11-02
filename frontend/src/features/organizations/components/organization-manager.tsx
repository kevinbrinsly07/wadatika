
"use client";

import { useState } from "react";

import { createOrganization } from "../organization-service";
import { useOrganization } from "../organization-context";

export function OrganizationManager() {
  const [name, setName] = useState("");
  const [slug, setSlug] = useState("");

  const [isCreating, setIsCreating] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const {
    organizations,
    isLoading,
    error: organizationError,
    addOrganization,
  } = useOrganization();

  function handleNameChange(value: string) {
    setName(value);

    const generatedSlug = value
      .toLowerCase()
      .trim()
      .replace(/[^a-z0-9]+/g, "-")
      .replace(/^-|-$/g, "");

    setSlug(generatedSlug);
  }

  async function handleCreateOrganization(
    event: React.FormEvent<HTMLFormElement>,
  ) {
    event.preventDefault();

    setError(null);
    setSuccess(null);
    setIsCreating(true);

    try {
      const organization = await createOrganization({
        name: name.trim(),
        slug: slug.trim(),
      });

      addOrganization(organization);

      setName("");
      setSlug("");
      setSuccess("Organization created successfully.");
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : "Unable to create organization.",
      );
    } finally {
      setIsCreating(false);
    }
  }

  return (
    <section className="mt-10">
      <div className="flex flex-col gap-2 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <p className="text-sm font-medium">
            YOUR WORKSPACES
          </p>

          <h2 className="mt-3 text-xl font-semibold text-[#8BAE66]">
            Organizations
          </h2>

          <p className="mt-1 text-sm">
            Manage the organizations where you collaborate.
          </p>
        </div>

        <span className="text-sm text-[#628141]">
          {organizations.length}{" "}
          {organizations.length === 1
            ? "organization"
            : "organizations"}
        </span>
      </div>

      <div className="mt-6 grid gap-6 lg:grid-cols-[1fr_340px]">
        <div className="rounded-xl border border-slate-800 bg-[#628141] p-5">
          <h3 className="font-bold text-black/70">
            Your organizations
          </h3>

          {isLoading ? (
            <p className="mt-5 text-sm">
              Loading organizations...
            </p>
          ) : organizations.length === 0 ? (
            <div className="mt-5 rounded-lg border border-dashed border-[#8BAE66] p-6 text-center">
              <p className="text-sm font-medium">
                No organizations yet
              </p>

              <p className="mt-2 text-sm">
                Create your first organization to get started.
              </p>
            </div>
          ) : (
            <div className="mt-4 space-y-3">
              {organizations.map((organization) => (
                <div
                  key={organization.id}
                  className="flex items-center gap-3 rounded-lg bg-[#1B211A] p-4"
                >
                  <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-[#628141] font-semibold text-black">
                    {organization.name.charAt(0).toUpperCase()}
                  </div>

                  <div className="min-w-0 flex-1">
                    <p className="truncate font-Semibold">
                      {organization.name}
                    </p>

                    <p className="mt-1 truncate text-xs">
                      {organization.slug}
                    </p>
                  </div>

                  <span className="rounded-full bg-[#628141] px-2.5 py-1 font-bold text-xs text-black/70">
                    Member
                  </span>
                </div>
              ))}
            </div>
          )}

          {(error || organizationError) && (
            <p role="alert" className="mt-4 text-sm text-red-400">
              {error || organizationError}
            </p>
          )}
        </div>

        <div className="rounded-xl bg-[#628141] p-6">
          <h3 className="font-bold text-black/70">
            Create organization
          </h3>

          <p className="mt-1 text-sm text-black/70">
            Start a new workspace for your team.
          </p>

          <form
            onSubmit={handleCreateOrganization}
            className="mt-5 space-y-4"
          >
            <div>
              <label
                htmlFor="organization-name"
                className="mb-2 block font-semibold text-sm text-black/70"
              >
                Organization name
              </label>

              <input
                id="organization-name"
                type="text"
                required
                maxLength={100}
                value={name}
                onChange={(event) =>
                  handleNameChange(event.target.value)
                }
                placeholder="Acme Technologies"
                className="w-full rounded-lg bg-[#1B211A] px-3 py-2.5 text-sm text-[#628141] outline-none transition focus:border-[#1B211A] placeholder:text-[#628141]"
              />
            </div>

            <div>
              <label
                htmlFor="organization-slug"
                className="mb-2 block text-sm text-black/70"
              >
                Organization URL slug
              </label>

              <input
                id="organization-slug"
                type="text"
                required
                maxLength={100}
                pattern="[a-z0-9]+(?:-[a-z0-9]+)*"
                value={slug}
                onChange={(event) => setSlug(event.target.value)}
                placeholder="acme-technologies"
                className="w-full rounded-lg bg-[#1B211A] text-sm  px-3 py-2.5 text-[#628141] outline-none transition focus:border-[#1B211A] placeholder:text-[#628141]"
              />

              <p className="mt-1 text-xs text-black/70">
                Lowercase letters, numbers, and hyphens.
              </p>
            </div>

            {success && (
              <p role="status" className="text-sm text-emerald-400">
                {success}
              </p>
            )}

            <button
              type="submit"
              disabled={
                isCreating || !name.trim() || !slug.trim()
              }
              className="w-full rounded-lg bg-[#8BAE66] px-4 py-2.5 text-sm font-semibold text-black/80 transition hover:bg-[#EBD5AB] disabled:cursor-not-allowed disabled:opacity-50 cursor-pointer"
            >
              {isCreating ? "Creating..." : "Create organization"}
            </button>
          </form>
        </div>
      </div>
    </section>
  );
}
