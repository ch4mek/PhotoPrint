import { ThemeMode } from '@/theme';

export const LEADERBOARD_COLUMN_SIZE = 5;
export const LEADERBOARD_TITLE = 'Лучшие результаты';
export const LEADERBOARD_WIDGET_MAX_HEIGHT = 430;

export const topResultBackgroundColors = {
	[ThemeMode.LIGHT]: '#F179001A',
	[ThemeMode.DARK]: '#FEC3871A',
} as const;
