import { JSX } from 'react';

export interface BaseFileAttachedProps {
	fileName: string;
	fileMetaInfo: string[];

	maxWidth?: number;
	minWidth?: number;

	controls?: () => JSX.Element | undefined;
	onClick?: VoidFunction;
}
