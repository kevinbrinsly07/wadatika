export type CreateProjectRequest = {
     name: string;
     key: string;
     description?: string | null;
   };
   
   export type Project = {
     id: string;
     organizationId: string;
     name: string;
     key: string;
     description: string | null;
     createdAt: string;
   };