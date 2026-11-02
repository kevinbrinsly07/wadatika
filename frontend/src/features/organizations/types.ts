export type Organization = {
     id: string;
     name: string;
     slug: string;
     createdAt: string;
   };
   
   export type CreateOrganizationRequest = {
     name: string;
     slug: string;
   };