import { Theme } from '@emotion/react';

export interface DividerProps {
	color?: keyof Theme['palette'];
	margin?: string;
}
