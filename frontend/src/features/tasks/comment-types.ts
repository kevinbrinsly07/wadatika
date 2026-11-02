export type CreateTaskCommentRequest = {
     content: string;
   };
   
   export type TaskComment = {
     id: string;
     projectTaskId: string;
     authorUserId: string;
     content: string;
     createdAt: string;
     updatedAt: string | null;
   };