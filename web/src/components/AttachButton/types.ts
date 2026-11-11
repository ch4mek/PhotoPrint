import { ChangeEvent, ReactNode, RefAttributes } from 'react';

export interface AttachButtonProps extends RefAttributes<HTMLInputElement> {
	onAttachedFileChange: (
		fileType?: string
	) => (e: ChangeEvent<HTMLInputElement>) => Array<File> | File | undefined;
	validExtensions: string[];
	title?: string;
	fileTypes?: string[];
	isAttachMultiple?: boolean;
	disabled?: boolean;
	icon?: ReactNode;
}
