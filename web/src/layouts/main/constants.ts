import { Routes } from '@/constants/routes';
import { zIndexes } from '@/constants/zIndexes';

export const TOP_POSITION_LOAD_WRAPPER = 54;

export const bothScrollLayoutRoutes = [
	Routes.home,
	Routes.reports,
	Routes.booking,
	Routes.timeSheet,
];

export const SCROLL_BOTTOM_MARGIN = 16;
export const TIMER_VISIBLE_BREAKPOINT = 665;

export const HEADER_HEIGHT = 54;
export const HEADER_STYLES = { height: HEADER_HEIGHT, zIndex: zIndexes.header };
export const HEADER_ACTION_BUTTON_STYLES = { padding: 0 };
