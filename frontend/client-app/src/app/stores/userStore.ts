import { makeAutoObservable, runInAction } from "mobx";
import { User, UserFormValues } from "../models/user";
import agent from "../api/agent";
import { store } from "./store";
import { router } from "../router/Routes";

export default class UserStore {
  user: User | null = null;
  fbLoading = false;
  refreshTokenTimeout?: NodeJS.Timeout;

  constructor() {
    makeAutoObservable(this);
  }

  get isLoggedIn() {
    return !!this.user;
  }

  login = async (creds: UserFormValues) => {
    try {
      // console.log("Attempting to log in with credentials:", creds);
      const user = await agent.Account.login(creds);
      console.log("Login successful, user:", user);
      store.commonStore.setToken(user.token);
      this.startRefreshTokenTimer(user);
      runInAction(() => (this.user = user));
      router.navigate("/activities");
      store.modalStore?.closeModal();
    } catch (error) {
      console.error("Login failed", error);
      throw error;
    }
  };

  register = async (creds: UserFormValues) => {
    try {
      await agent.Account.register(creds);

      router.navigate(`/account/registerSuccess?email=${creds.email}`);
      store.modalStore?.closeModal();
    } catch (error) {
      console.log(error);
      throw error;
    }
  };

  logout = () => {
    store.commonStore.setToken(null);
    this.user = null;
    router.navigate("/");
  };

  getUser = async () => {
    try {
      const user = await agent.Account.current();
      store.commonStore.setToken(user.token);
      this.startRefreshTokenTimer(user);
      runInAction(() => (this.user = user));
    } catch (error) {
      console.log(error);
    }
  };

  setImage = (image: string) => {
    if (this.user) this.user.image = image;
  };

  setDisplayName = (name: string) => {
    if (this.user) this.user.displayName = name;
  };

  setUsername = (username: string) => {
    if (this.user) this.user.username = username;
  };

  facebookLogin = async (accessToken: string) => {
    this.fbLoading = true;
    try {
      const user = await agent.Account.fblogin(accessToken);
      store.commonStore.setToken(user.token);
      this.startRefreshTokenTimer(user);
      runInAction(() => {
        this.user = user;
        this.fbLoading = false;
      });
      router.navigate("/activities");
      store.modalStore?.closeModal();
    } catch (error) {
      console.log(error);
      runInAction(() => (this.fbLoading = false));
    } finally {
      runInAction(() => (this.fbLoading = false));
    }
  };

  refreshToken = async () => {
    this.stopRefreshTokenTimer();
    try {
      const user = await agent.Account.refreshToken();
      runInAction(() => (this.user = user));
      store.commonStore.setToken(user.token);
      this.startRefreshTokenTimer(user);
    } catch (error) {
      console.log(error);
    }
  };

  private startRefreshTokenTimer = (user: User) => {
    // atob() decodes a base-64 encoded string (token) to a new string (jwtToken) with the decoded data (header, payload, signature) in it (JSON format)
    const jwtToken = JSON.parse(atob(user.token.split(".")[1]));
    // jwtToken.exp is the expiration time of the token in seconds since 1970-01-01T00:00:00Z (UTC)
    const expires = new Date(jwtToken.exp * 1000);
    // subtract 60 seconds from the expiration time to give the token some time to refresh before it expires
    // (the token will be refreshed 60 seconds before it expires)
    const timeout = expires.getTime() - Date.now() - 60 * 1000;
    this.refreshTokenTimeout = setTimeout(this.refreshToken, timeout);
    console.log("Refresh token timeout:", this.refreshTokenTimeout);
  };

  private stopRefreshTokenTimer = () => {
    clearTimeout(this.refreshTokenTimeout);
  };
}
