import { Theme } from '@emotion/react';

export interface CellWithExpandRendererOptions<T> {
	fieldToDisplay?: keyof T;
	theme?: Theme;
}

export interface GridRowProps {
	hasStages?: boolean;
	expanded?: boolean;
	isMainInterview?: boolean;
}
