import { Theme } from '@emotion/react';

export const KPI_COLOR_KEYS: Record<
	'Red' | 'Orange' | 'Green',
	keyof Theme['palette']
> = {
	Red: 'feedback-danger',
	Orange: 'text-accent',
	Green: 'feedback-success',
};

export const MONTH_NAME_OFFSET_Y = 16;
export const PERCENT_OFFSET_Y = 32;
export const PERCENT_SUFFIX = '%';
