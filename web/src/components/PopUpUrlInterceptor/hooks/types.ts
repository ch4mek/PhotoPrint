type AllowedPopUps = [string, string][];

export interface PopupAccessState {
	allowed: AllowedPopUps;
	denied: string[];
	valueDenied: string[];
}

export interface UseDeniedPopUpNotificationsProps {
	valueDeniedPopUps: string[];
	isUserInitialized: boolean;
}

export interface UseCleanDeniedParamsProps {
	deniedPopUps: string[];
	isUserInitialized: boolean;
}

export interface UsePopUpRendererProps {
	allowedPopUps: AllowedPopUps;
	isUserInitialized: boolean;
}

export interface UseRedirectHomeProps {
	isRedirect: boolean;
	allowedPopUps: AllowedPopUps;
	search: string;
}
