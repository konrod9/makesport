export type UserRole = "User" | "Moderator" | "Admin";

export type AuthUser = {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  userName: string;
};
