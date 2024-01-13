import { Navigate, RouteObject, createBrowserRouter } from "react-router-dom";
import App from "../layout/App";
import ActivityDashboard from "../../components/activities/dashboard/ActivityDashboard";
import ActivityForm from "../../components/activities/form/ActivityForm";
import ActivityDetails from "../../components/activities/details/ActivityDetails";
// import TestErrors from "../../components/errors/TestError";
import NotFound from "../../components/errors/NotFound";
import ServerError from "../../components/errors/ServerError";
// import LoginForm from "../../components/users/LoginForm";
import ProfilePage from "../../components/profiles/ProfilePage";
import RequireAuth from "./RequireAuth";

export const routes: RouteObject[] = [
  {
    path: "/",
    element: <App />,
    children: [
      {
        element: <RequireAuth />,
        children: [
          { path: "activities", element: <ActivityDashboard /> },
          { path: "activities/:id", element: <ActivityDetails /> },
          { path: "createActivity", element: <ActivityForm key="create" /> },
          { path: "manage/:id", element: <ActivityForm key="manage" /> },
          { path: "profiles/:username", element: <ProfilePage /> },
          // { path: "errors", element: <TestErrors /> },
        ],
      },

      // { path: "login", element: <LoginForm /> },
      { path: "not-found", element: <NotFound /> },
      { path: "server-error", element: <ServerError /> },
      { path: "*", element: <Navigate replace to="/not-found" /> },
    ],
  },
];

export const router = createBrowserRouter(routes);
