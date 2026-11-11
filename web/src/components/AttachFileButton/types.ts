import { ChangeEvent } from 'react';

export interface AttachFileButtonProps {
	validExtensions: string[];
	title: string;
	attachedFileName?: string;
	onAttachedFileRemove: () => void;
	onAttachedFileChange: (
		fileType?: string
	) => (e: ChangeEvent<HTMLInputElement>) => File | undefined;
}
