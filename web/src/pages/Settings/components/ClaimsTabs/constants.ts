import { ThemeMode } from '@/theme';

const NAME_COLUMN_WIDTH = 200;
export const DEPARTMENT_COLUMN_WIDTH = 88;
export const ADMINISTRATION_DEPARTMENT = 'Administration';
export const SYSTEM_ADMINISTRATION_DEPARTMENT = 'System Administrators';
export const SYSTEM_ADMINISTRATION_DEPARTMENT_SHORT = 'Sys. Admin.';
export const ADMINISTRATION_DEPARTMENT_WIDTH = 97;

export const ROLE_COLUMN_WIDTH = 267;

export const FIRST_COLUMN_SETUP = {
	name: '',
	key: 'name',
	width: NAME_COLUMN_WIDTH,
};

export const NAME_CELL_BACKGROUND: Record<ThemeMode, string> = {
	light: '#f6f7f9',
	dark: '#383C49',
};
