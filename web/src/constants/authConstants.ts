// The key that will indicate the date of the user's last activity in localStorage
export const USER_LAST_ACTIVITY_DATE = 'user-last-activity-date';

export const BROADCAST_CHANNEL_NAME = 'bc-session-storage';

// This key in local storage contains the reason for the logout in localStorage
export const LOGOUT_REASON = 'logout-reason';

export const LOGOUT_MESSAGE = 'logout-message';
// This key in local storage contains the timestamp until the end of the current session
export const SESSION_EXPIRES = 'session-expires';
export const SESSION_EXPIRES_TEXT = 'Your session will expire in';
// This key in local storage contains status of log out after user inactivity
export const LOG_OUT_STATUS = 'log-out-status';
// Time after which popup will be shown if the user is inactive
export const INACTIVE_LIMIT = 1800000;
// Time after which logout will occur
export const SESSION_DURATION = 7200000;
// Time of showing popup after which log out will occur
export const SHOWING_LOG_OUT_POPUP_DURATION = 30000;

export const REDIRECT_AFTER_LOGIN_URL = 'redirectAfterLogin';

export enum LogoutReasons {
	VIA_BUTTON = 'logout-via-btn',
	SESSION_EXPIRED = 'session-expired',
	INACTIVITY = 'user-inactivity',
	VIA_MESSAGE = 'logout-via-message',
}

export enum LogoutStatuses {
	STAY_LOG_IN = 'stay-log-in',
	LOG_OUT = 'log-out',
}

// Array of events that will be considered user activity
export const USER_ACTIVITY_EVENTS = [
	'load',
	'mousemove',
	'click',
	'contextmenu',
	'scroll',
	'drag',
	'copy',
	'cut',
	'paste',
	'touchstart',
	'touchend',
	'touchcancel',
	'touchmove',
];
