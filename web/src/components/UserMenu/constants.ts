export const HEADER_AVATAR_SIZE = 38;
export const USER_MENU_WIDTH = 350;
export const USER_MENU_TOP_MARGIN = 52;
export const USER_MENU_SCROLL_MARGIN = 2;

const placement = {
	vertical: 'top',
	horizontal: 'right',
} as const;

export const CUSTOM_USER_MENU_PROPS = {
	id: 'menu-appbar',
	anchorOrigin: placement,
	transformOrigin: placement,
	keepMounted: true,
	elevation: 0,
};
