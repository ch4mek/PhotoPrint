import { JSX } from 'react';

export interface AttachedFileWithTypesProps {
	fileName: string;
	file?: File;
	fileSize?: number;
	handleRemoveClick?: VoidFunction;
	fileControl?: JSX.Element;
	hasMaxWidth?: boolean;
	maxWidth?: number;
	minWidth?: number;
	onClick?: VoidFunction;
}
